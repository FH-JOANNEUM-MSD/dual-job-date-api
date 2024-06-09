using Microsoft.IdentityModel.Tokens;

namespace DualJobDate.BusinessLogic.Exceptions
{
    public sealed class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(string? userId = null)
            : base(GetMessage(userId))
        {
        }

        private static string GetMessage(string userId)
        {
            return userId.IsNullOrEmpty()
                ? $"The user with the identifier {userId} was not found."
                : "User identifier was not provided.";
        }
    }
}
