using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ShortCircuit.Server.Model
{
    public class FailedResponse(ResponseType type, string? message) : IResponse<object>
    {
        public ResponseType Type { get; set; } = type;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Message { get; set; } = message;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object? Value => null;
    }
}