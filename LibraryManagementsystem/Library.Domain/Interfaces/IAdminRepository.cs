public interface IAdminRepository
{
	Task<Admin?> GetByUsernameAsync(string username);
}
Repositories