using Microsoft.EntityFrameworkCore;

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
        public DbSet<InvoiceDetails> invoiceDetails { get; set; }
    }
}
