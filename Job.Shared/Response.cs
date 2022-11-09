namespace Job.Shared
{
    public record class Response
    {
        public Response(bool isValid,string message)
        {
            IsValid = isValid;
            Message = message;
        }
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}
