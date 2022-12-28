using ReadySetSurgical.Data;
using ReadySetSurgical.Model;

namespace ReadySetSurgical.InvoiceRepository
{
    public class InvoiceDetailRepository : IInvoiceDetailRepository
    {
        private readonly DataContext _context;

        /// <summary>
        /// Constructs an instance with a preconfigured datacontext.
        /// </summary>
        /// <param name="context"></param>
        public InvoiceDetailRepository(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// It returns list of invoice details from RDS.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<InvoiceDetail> GetAllInvoiceDetail()
        {
            return _context.InvoiceDetail.ToList();
        }

    }
}
