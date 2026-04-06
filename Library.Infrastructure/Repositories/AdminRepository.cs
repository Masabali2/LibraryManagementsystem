using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly LibraryDbContext _context;

    public AdminRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<Admin?> GetAdminByCredentialsAsync(string username, string password)
    {

        return await _context.Admins
            .Where(u => u.Username == username && u.PasswordHash == password)
            .FirstOrDefaultAsync();
    }
}
