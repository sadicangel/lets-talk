using System.Text.Json;
using System.Text.Json.Serialization;

namespace LetsTalk;

[JsonConverter(typeof(ContentTypeJsonConverter))]
public enum ContentType
{
    TextPlain
}

file sealed class ContentTypeJsonConverter : JsonConverter<ContentType>
{
    public override ContentType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (reader.GetString() ?? throw new JsonException()) switch
        {
            "text/plain" => ContentType.TextPlain,
            _ => throw new JsonException($"Unmapped {nameof(ContentType)} '{reader.GetString()}'")
        };
    }

    public override void Write(Utf8JsonWriter writer, ContentType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            ContentType.TextPlain => "text/plain",
            _ => throw new JsonException($"Unmapped {nameof(ContentType)} '{value}'")
        });
    }
}