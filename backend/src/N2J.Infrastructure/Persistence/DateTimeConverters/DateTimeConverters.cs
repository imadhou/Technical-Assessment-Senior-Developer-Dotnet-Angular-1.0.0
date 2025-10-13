using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace N2J.Infrastructure.Persistence.DateTimeConverters;


public class UtcConverter : ValueConverter<DateTime, DateTime>
{
    public UtcConverter() : base(
        v => ConvertToUtc(v),   // When writing to DB
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // When reading from DB
    )
    { }

    private static DateTime ConvertToUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
            return dateTime;

        if (dateTime.Kind == DateTimeKind.Local)
            return dateTime.ToUniversalTime();

        // Unspecified: treat as local and convert to UTC
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime();
    }
}

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
