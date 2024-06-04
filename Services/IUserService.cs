using Core_CustAuthHandler.Models;

namespace Core_CustAuthHandler.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<bool> AuthenticateUserAsync(User user);

        Task<User> GetUserAsync(string userName);
    }
}
