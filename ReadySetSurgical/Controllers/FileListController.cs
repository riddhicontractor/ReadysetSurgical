using Microsoft.AspNetCore.Mvc;
using ReadySetSurgical.Data;
using System.Data.SqlClient;

namespace ReadySetSurgical.Controllers
{
    public class FileListController : Controller
    {
        private DataContext _dataContext;
        public FileListController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SuccessIndex()
        {
            var details = _dataContext.invoiceDetails.ToList();
            return View(details);
        }
        public IActionResult ErrorIndex()
        {
            var logs = _dataContext.errorLogs.ToList();
            return View(logs);
        }
    }
}