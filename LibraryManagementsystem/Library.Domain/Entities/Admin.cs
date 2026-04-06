using System;
using System.Collections.Generic;

namespace Library.Domain.Entities;

public class Admin
{
    public int AdminId { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Email { get; set; }
}
