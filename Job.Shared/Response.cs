namespace Job.Shared
{
    /**
     * this class will be used to track response from the underlying
     * repository and services layers as most time i dont like rethrowing exceptions
     * as they hurt application overtime
     */
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
