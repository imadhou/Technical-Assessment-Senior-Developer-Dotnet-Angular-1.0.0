using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using N2J.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace N2J.Application.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly string _keySetPrefix = "keyset:";

        // Configure Newtonsoft.Json settings
        private static readonly JsonSerializerSettings _jsonSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        };

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var cachedData = await _cache.GetStringAsync(key);
                if (string.IsNullOrEmpty(cachedData))
                    return default;

                return JsonConvert.DeserializeObject<T>(cachedData, _jsonSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache key {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                if (value == null)
                {
                    _logger.LogWarning("Attempted to cache null value for key: {Key}", key);
                    return;
                }

                var options = new DistributedCacheEntryOptions();

                if (expiry.HasValue)
                {
                    options.SetAbsoluteExpiration(expiry.Value);
                }
                else
                {
                    // Default cache expiration
                    options.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                }

                // Use Newtonsoft.Json for serialization
                var serializedData = JsonConvert.SerializeObject(value, _jsonSettings);

                await _cache.SetStringAsync(key, serializedData, options);
                _logger.LogDebug("Successfully cached data for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache key {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                _logger.LogDebug("Removed cache key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache key {Key}", key);
            }
        }

        public Task RemoveByPatternAsync(string pattern)
        {
            _logger.LogWarning("Pattern-based cache removal not supported with IDistributedCache. Pattern: {Pattern}", pattern);
            return Task.CompletedTask;
        }

        public async Task AddKeyToGroupAsync(string groupKey, string cacheKey)
        {
            try
            {
                var groupCacheKey = $"{_keySetPrefix}{groupKey}";
                var existingKeys = await GetAsync<HashSet<string>>(groupCacheKey) ?? new HashSet<string>();
                existingKeys.Add(cacheKey);

                // Store the set of keys with a longer expiration
                await SetAsync(groupCacheKey, existingKeys, TimeSpan.FromDays(1));
                _logger.LogDebug("Added key {CacheKey} to group {GroupKey}", cacheKey, groupKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding key {CacheKey} to group {GroupKey}", cacheKey, groupKey);
            }
        }

        public async Task RemoveGroupAsync(string groupKey)
        {
            try
            {
                var groupCacheKey = $"{_keySetPrefix}{groupKey}";
                var keys = await GetAsync<HashSet<string>>(groupCacheKey);

                if (keys != null)
                {
                    foreach (var key in keys)
                    {
                        await RemoveAsync(key);
                    }
                    await RemoveAsync(groupCacheKey);
                    _logger.LogDebug("Removed cache group: {GroupKey}", groupKey);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache group {GroupKey}", groupKey);
            }
        }

        // Helper method for debugging serialization
        public async Task<bool> TestSerializationAsync<T>(string key, T value)
        {
            try
            {
                _logger.LogInformation("Testing serialization for type: {Type}", typeof(T).Name);

                var serialized = JsonConvert.SerializeObject(value, _jsonSettings);
                _logger.LogInformation("Serialized data: {SerializedData}", serialized);

                await SetAsync(key, value, TimeSpan.FromMinutes(1));
                var retrieved = await GetAsync<T>(key);

                var success = retrieved != null;
                _logger.LogInformation("Serialization test result: {Result}", success ? "SUCCESS" : "FAILED");

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Serialization test failed");
                return false;
            }
        }
    }
}