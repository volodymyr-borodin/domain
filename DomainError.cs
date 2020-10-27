namespace Domain
{
    public sealed class DomainError
    {
        public DomainError(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}