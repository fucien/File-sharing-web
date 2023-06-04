using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using web_ver_2.Models;
using System.Reflection.Metadata;

namespace web_ver_2.Data
{
    //Set up DbContext
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<web_ver_2.Models.File> File { get; set; }
        public DbSet<web_ver_2.Models.User> User { get; set; }
    }
}