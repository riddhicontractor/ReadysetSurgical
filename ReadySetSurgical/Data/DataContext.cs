using Microsoft.EntityFrameworkCore;
using ReadySetSurgical.Models;

namespace ReadySetSurgical.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
        {

        }
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<InvoiceDetail> InvoiceDetail { get; set; }

        public DbSet<ErrorLog> Errorlog { get; set; }
    }
}
