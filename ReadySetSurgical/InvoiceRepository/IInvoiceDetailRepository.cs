using ReadySetSurgical.Model;

namespace ReadySetSurgical.InvoiceRepository
{
    public interface IInvoiceDetailRepository
    {
        IEnumerable<InvoiceDetail> GetAllInvoiceDetail();
    }
}
