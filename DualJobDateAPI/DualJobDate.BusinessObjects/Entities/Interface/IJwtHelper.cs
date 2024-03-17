namespace DualJobDate.BusinessObjects.Entities.Interface
{
    public interface IJwtHelper
    {
        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="userType">The type of the user.</param>
        /// <param name="expiresInMinutes">Optional. The expiration time of the token in minutes. 
        /// If null, the default expiration time (24 hours) will be used.</param>
        /// <returns>The generated JWT token.</returns>
        public string GenerateJwtToken(string userId, string userType, int? expiresInMinutes = null);
    }
}
