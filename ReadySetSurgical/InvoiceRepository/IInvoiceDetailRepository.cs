using ReadySetSurgical.Models;

namespace ReadySetSurgical.InvoiceRepository
{
    public interface IInvoiceDetailRepository
    {
        IEnumerable<InvoiceDetail> GetAllInvoiceDetail();
    }
}
