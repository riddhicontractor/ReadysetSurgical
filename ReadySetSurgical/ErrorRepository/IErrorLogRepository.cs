using ReadySetSurgical.Model;

namespace ReadySetSurgical.ErrorRepository
{
    public interface IErrorLogRepository
    {
        IEnumerable<ErrorLog> GetAllErrorLog();
    }
}
