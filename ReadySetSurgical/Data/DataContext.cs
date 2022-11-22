using Amazon.S3.Model;
using AWS_S3_Demo.Model;
using Microsoft.EntityFrameworkCore;

namespace AWS_S3_Demo.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
        {

        }
        public DataContext(DbContextOptions options) :base(options)
        {
           
        }
        public DbSet<InvoiceDetails> invoiceDetails { get; set; }
    }
}
