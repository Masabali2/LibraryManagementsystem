public class AdminRepository : IAdminRepository
{
    private readonly LibraryDbContext _context;

    public AdminRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<Admin?> GetByUsernameAsync(string username)
    {
        // Finding the admin by username in the DB
        return await _context.Admins
            .FirstOrDefaultAsync(a => a.Username == username);
    }
}