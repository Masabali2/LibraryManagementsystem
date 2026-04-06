using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Application.Interfaces;
using Library.Domain.Interfaces;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Identity; 

namespace Library.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAdminRepository _adminRepo;
    private readonly IStudentRepository _studentRepo; // Injected for student operations
    private readonly PasswordHasher<Student> _hasher; // For secure hashing

    public AuthService(IAdminRepository adminRepo, IStudentRepository studentRepo)
    {
        _adminRepo = adminRepo;
        _studentRepo = studentRepo;
        _hasher = new PasswordHasher<Student>();
    }

    // 1. Authenticate Admin
    public async Task<bool> AuthenticateAdminAsync(string username, string password)
    {
        var admin = await _adminRepo.GetAdminByCredentialsAsync(username, password);
        return admin != null;
    }

    // 2. Authenticate Student
    public async Task<bool> AuthenticateStudentAsync(string username, string password)
    {
        var student = await _studentRepo.GetStudentByUsernameAsync(username);

        if (student == null)
        {
            return false;
        }

        // Verify the stored hash against the incoming plain text password
        var result = _hasher.VerifyHashedPassword(student, student.PasswordHash, password);

        return result == PasswordVerificationResult.Success;
    }

    // 3. Register Student
    public async Task<bool> RegisterStudentAsync(Student student)
    {
        // Check if username or RollNo already exists to prevent duplicates
        var existingUser = await _studentRepo.GetStudentByUsernameAsync(student.Username);
        if (existingUser != null) return false;

        var existingRoll = await _studentRepo.GetStudentByRollNoAsync(student.RollNo);
        if (existingRoll != null) return false;

        // Securely hash the password before it touches the database
        student.PasswordHash = _hasher.HashPassword(student, student.PasswordHash);

        // Explicitly set the role
        student.Role = "Student";

        return await _studentRepo.AddStudentAsync(student);
    }
}