using ReadySetSurgical.Models;

namespace ReadySetSurgical.ErrorRepository
{
    public interface IErrorLogRepository
    {
        IEnumerable<ErrorLog> GetAllErrorLog();
    }
}
