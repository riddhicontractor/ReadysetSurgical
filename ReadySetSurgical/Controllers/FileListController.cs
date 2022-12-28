using Microsoft.AspNetCore.Mvc;
using ReadySetSurgical.ErrorRepository;
using ReadySetSurgical.InvoiceRepository;

namespace ReadySetSurgical.Controllers
{
    public class FileListController : Controller
    {
        private readonly IInvoiceDetailRepository _invoiceDetailRepository;

        private readonly IErrorLogRepository _errorLogRepository;

        /// <summary>
        /// Constructs an instance with a preconfigured InvoiceDetail and ErrorLog Repository.
        /// </summary>
        /// <param name="invoiceDetailRepository"></param>
        /// <param name="errorLogRepository"></param>
        public FileListController(IInvoiceDetailRepository invoiceDetailRepository, IErrorLogRepository errorLogRepository)
        {
            _invoiceDetailRepository = invoiceDetailRepository;
            _errorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// This method is called for every FileList Controller invocation.
        /// </summary>
        /// <returns></returns>
        public IActionResult FileList()
        {
            return View();
        }

        /// <summary>
        /// This method is called for Success Files invocation and returns the invoice details to the UI from RDS.
        /// </summary>
        /// <returns></returns>
        public IActionResult GetInvoiceDetail()
        {
            var details = _invoiceDetailRepository.GetAllInvoiceDetail();
            return View(details);
        }

        /// <summary>
        /// This method is called for Error Files invocation and returns the error logs to the UI from RDS.
        /// </summary>
        /// <returns></returns>
        public IActionResult GetErrorLog()
        {
            var logs = _errorLogRepository.GetAllErrorLog();
            return View(logs);
        }
    }
}