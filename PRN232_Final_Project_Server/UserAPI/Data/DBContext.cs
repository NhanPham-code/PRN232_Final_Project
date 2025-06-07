using Microsoft.EntityFrameworkCore;
using System;
using UserAPI.Models;

namespace UserAPI.Data
{
    public class DBContext : DbContext
    {
        public DbSet<Feedback> Feedbacks { get; set; }

        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
    }
}
