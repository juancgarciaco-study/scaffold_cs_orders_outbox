using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace app.api.Infrastructure.Converters;

public class UlidToBytesConverter(ConverterMappingHints? mappingHints) : ValueConverter<Ulid, byte[]>(x => x.ToByteArray(),
    x => new Ulid(x),
    mappingHints: DefaultHints.With(mappingHints))
{
    private static readonly ConverterMappingHints DefaultHints = new(16);

    public UlidToBytesConverter() : this(null)
    {
    }
}

public class UlidToGuidConverter(ConverterMappingHints? mappingHints) : ValueConverter<Ulid, Guid>(x => x.ToGuid(),
    x => new Ulid(x),
    mappingHints: DefaultHints.With(mappingHints))
{
    private static readonly ConverterMappingHints DefaultHints = new(16);

    public UlidToGuidConverter() : this(null)
    {
    }
}

public class UlidToStringConverter(ConverterMappingHints? mappingHints) : ValueConverter<Ulid, string>(x => x.ToString(),
    x => Ulid.Parse(x, CultureInfo.InvariantCulture),
    mappingHints: DefaultHints.With(mappingHints))
{
    private static readonly ConverterMappingHints DefaultHints = new(26);

    public UlidToStringConverter() : this(null)
    {
    }
}
