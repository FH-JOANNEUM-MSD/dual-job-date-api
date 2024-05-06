namespace DualJobDate.BusinessLogic.Exceptions;

public abstract class NotFoundException : Exception
{
    protected NotFoundException(string message)
        : base(message)
    {
    }
}