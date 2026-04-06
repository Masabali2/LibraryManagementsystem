public class AuthService
{
    private readonly IAdminRepository _adminRepo;

    public AuthService(IAdminRepository adminRepo)
    {
        _adminRepo = adminRepo;
    }

    public async Task<bool> ValidateLogin(string username, string password)
    {
        var admin = await _adminRepo.GetByUsernameAsync(username);

        if (admin == null) return false;

        // Simple check for now; in production use: BCrypt.Verify(password, admin.PasswordHash)
        return admin.PasswordHash == password;
    }
}