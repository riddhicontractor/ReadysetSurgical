using ReadySetSurgical.Models;

namespace ReadySetSurgical.Repository
{
    public interface IInvoiceDetailRepository
    {
        IEnumerable<InvoiceDetail> GetAllInvoiceDetail();
    }
}
