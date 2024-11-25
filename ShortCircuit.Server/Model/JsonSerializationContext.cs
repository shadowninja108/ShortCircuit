using FirstStrike.Thunder.Save;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShortCircuit.Server.Model
{
    [JsonSourceGenerationOptions]

    /* All types for responses. */
    [JsonSerializable(typeof(ResponseType))]
    [JsonSerializable(typeof(FailedResponse))]
    [JsonSerializable(typeof(SuccessResponse<string>))]
    [JsonSerializable(typeof(SuccessResponse<Save.Raw>))]
    [JsonSerializable(typeof(SuccessResponse<List<string>>))]
    /* FirstStrike types. */
    [JsonSerializable(typeof(Save.Raw))]
    /* Primitives. */
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(object[]))]
    [JsonSerializable(typeof(string[]))]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(Dictionary<string, object>))]
    public partial class JsonSerializationContext : JsonSerializerContext
    {
        public static async Task Serialize<T>(Stream stream, T value)
        {
            await JsonSerializer.SerializeAsync(stream, value, typeof(T), Default);
        }   
    }
}