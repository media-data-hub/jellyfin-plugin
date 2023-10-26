using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaDataHub.Plugin.Json;

public class DateTimeFormatConverter : JsonConverter<DateTime>
{

  public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var value = reader.GetString() ?? string.Empty;
    return DateTime.ParseExact(reader.GetString() ?? string.Empty, "yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'FFFK", CultureInfo.InvariantCulture);
  }

  public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value.ToString());
  }
}
