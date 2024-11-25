using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ShortCircuit.Server.Model
{
    public interface IResponse<T>
    {
        public ResponseType Type { get; }

        public T? Value { get; }
    }
}