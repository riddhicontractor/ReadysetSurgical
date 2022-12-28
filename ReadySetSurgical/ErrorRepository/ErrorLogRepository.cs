using ReadySetSurgical.Data;
using ReadySetSurgical.Models;

namespace ReadySetSurgical.ErrorRepository
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly DataContext _context;

        /// <summary>
        /// Constructs an instance with a preconfigured datacontext.
        /// </summary>
        /// <param name="context"></param>
        public ErrorLogRepository(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// It returns list of error logs from RDS.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ErrorLog> GetAllErrorLog()
        {
            return _context.Errorlog.ToList();
        }
    }
}
