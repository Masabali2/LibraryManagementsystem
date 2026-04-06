using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Interfaces;

public interface IAuthService
{
    Task<bool> AuthenticateAdminAsync(string username, string password);
    Task<bool> AuthenticateStudentAsync(string username, string password);
    Task<bool> RegisterStudentAsync(Student student);
}
