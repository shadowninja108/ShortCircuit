namespace ShortCircuit.Server.Model
{
    public class SuccessResponse<T>(T value) : IResponse<T>
    {
        public ResponseType Type => ResponseType.Success;
        public T Value { get; } = value;
    }
}