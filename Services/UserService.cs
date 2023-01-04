using User = BorClone.Entity.User;

namespace BorClone.Services;

public class UserService
{
    public UserService()
    {

    }
    public async Task<User?> GetUserAsync(long? accountId)
    {
        return new User() { LanguageCode = "en" };

    }
}

