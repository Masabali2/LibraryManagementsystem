using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore;
using Library.Domain.Entities;

namespace Library.Infrastructure.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<Admin> Admins { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Borrowingrecord> BorrowingRecords { get; set; }

    // Add these two:
    public DbSet<Journal> Journals { get; set; }
    public DbSet<Thesis> Theses { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Fine> Fines { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Borrowingrecord>().HasKey(b => b.RecordId);
        modelBuilder.Entity<Journal>().HasKey(j => j.JournalId);
        modelBuilder.Entity<Thesis>().HasKey(t => t.ThesisId);
        
        modelBuilder.Entity<Reservation>().HasKey(r => r.ReservationId);
        modelBuilder.Entity<Fine>().HasKey(f => f.FineId);
    
        modelBuilder.Entity<Admin>().HasData(
            new Admin
            {
                AdminId = 1,
                Username = "admin",
                PasswordHash = "admin123"
            }
        );
    }

}

