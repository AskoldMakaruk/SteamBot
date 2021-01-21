using BFTemplate.Model;
using Microsoft.EntityFrameworkCore;

namespace BFTemplate.Database
{
    public class TelegramContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //todo add optionsBuilder.UseSomeDb(ConnectionString);
        }
    }
}