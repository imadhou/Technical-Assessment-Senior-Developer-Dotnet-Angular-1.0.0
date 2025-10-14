using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace N2J.Infrastructure.Persistence.DateTimeConverters;


/// <summary>
/// Provides a value converter for DateTime values that ensures all dates are stored in UTC format.
/// </summary>
/// <remarks>
/// This converter is typically used in Entity Framework Core to ensure that DateTime
/// values are written to the Postgres database in UTC format and read back as UTC.
/// </remarks>
public class UtcConverter : ValueConverter<DateTime, DateTime>
{
    public UtcConverter() : base(
        v => ConvertToUtc(v),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
    )
    { }

    private static DateTime ConvertToUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
            return dateTime;

        if (dateTime.Kind == DateTimeKind.Local)
            return dateTime.ToUniversalTime();

        return DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime();
    }
}

/// <summary>
/// Provides a value converter for nullable DateTime values that ensures all nullable values are converted to UTC.
/// </summary>
/// <remarks>
public class NullableUtcConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableUtcConverter() : base(
        v => v.HasValue ? ConvertToUtc(v.Value) : (DateTime?)null,
        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null
    )
    { }

    private static DateTime ConvertToUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
            return dateTime;

        if (dateTime.Kind == DateTimeKind.Local)
            return dateTime.ToUniversalTime();

        return DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime();
    }
}
