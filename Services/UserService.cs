using Core_CustAuthHandler.Models;

namespace Core_CustAuthHandler.Services
{
    public class UserService : IUserService
    {

        AppSecurityContext _context;

        public UserService(AppSecurityContext context)
        {
            _context = context;
        }

        public async Task<bool> AuthenticateUserAsync(User user)
        {
            bool IsAutenticated = false;
            try
            {
                // 1. Check the UserName exist or not
                var findUser =  _context.Users.Where(u => u.UserName.Trim() == user.UserName.Trim()).FirstOrDefault();
                if (findUser == null)
                    throw new Exception($"User Name: {user.UserName} isnot found");
                // 2. Check for the Password Match
                if (String.Equals(findUser.Password.Trim(), user.Password.Trim()))
                {
                    IsAutenticated = true;
                }

                return await Task.FromResult(IsAutenticated);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                var result = await _context.AddAsync(user);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> GetUserAsync(string userName)
        {
            var findUser = _context.Users.Where(u => u.UserName.Trim() == userName.Trim()).FirstOrDefault();
            return await Task.FromResult(findUser) ;
        }
    }
}
