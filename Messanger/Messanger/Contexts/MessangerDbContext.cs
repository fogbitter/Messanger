using Messanger.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messanger.Contexts
{
    public class MessangerDbContext : DbContext
    {
        public MessangerDbContext(DbContextOptions<MessangerDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Dialog> Dialogs { get; set; }
        public DbSet<DialogUserLink> DialogUserLinks { get; set; }
    }
}
