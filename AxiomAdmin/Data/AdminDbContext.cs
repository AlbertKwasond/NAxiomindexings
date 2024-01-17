using AxiomAdmin.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AdminDbContext : IdentityDbContext
{
    public AdminDbContext(DbContextOptions<AdminDbContext> options)
           : base(options)
    {
    }

    public DbSet<UsersAccount> UsersAccounts { get; set; }
}