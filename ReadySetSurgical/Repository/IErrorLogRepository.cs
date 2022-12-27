using ReadySetSurgical.Models;

namespace ReadySetSurgical.Repository
{
    public interface IErrorLogRepository
    {
        IEnumerable<ErrorLog> GetAllErrorLog();
    }
}
