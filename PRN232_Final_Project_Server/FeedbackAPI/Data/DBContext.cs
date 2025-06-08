using Microsoft.EntityFrameworkCore;
using System;
using FeedbackAPI.Models;

namespace FeedbackAPI.Data
{
    public class DBContext : DbContext
    {
        public DbSet<Feedback> Feedbacks { get; set; }

        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
    }
}
