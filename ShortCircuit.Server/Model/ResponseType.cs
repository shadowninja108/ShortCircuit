using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ShortCircuit.Server.Model
{
    [JsonConverter(typeof(JsonStringEnumConverter<ResponseType>))]
    public enum ResponseType
    {
        Success,
        Cancelled,
        SaveBad,
        SaveTooBig,
        UnknownFailure,
    }
}