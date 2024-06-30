using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaDataHub.Plugin.Json;

public class NullableDateTimeFormatConverter : JsonConverter<DateTime?>
{

  public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var value = reader.GetString() ?? string.Empty;
    if (string.IsNullOrEmpty(value))
    {
      return null;
    }
    return DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss.FFFK", CultureInfo.InvariantCulture);
  }

  public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value?.ToString());
  }
}
