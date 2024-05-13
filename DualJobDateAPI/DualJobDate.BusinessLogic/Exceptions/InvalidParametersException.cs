namespace DualJobDate.BusinessLogic.Exceptions
{
    public sealed class InvalidParametersException : BadRequestException
    {
        public InvalidParametersException()
            : base("Invalid request parameters")
        {
        }
    }
}
