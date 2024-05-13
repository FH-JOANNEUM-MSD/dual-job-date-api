namespace DualJobDate.BusinessLogic.Exceptions;

public sealed class AppointmentNotFoundException(string message) : NotFoundException(GetMessage(message))
{
    private static string GetMessage(string location)
    {
        return $"No appointments found for {location}!";
    }
}