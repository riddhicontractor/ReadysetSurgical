using Microsoft.AspNetCore.Mvc;
using ReadySetSurgical.Data;
using System.Data.SqlClient;

namespace ReadySetSurgical.Controllers
{
    public class FileListController : Controller
    {
        //private readonly SqlConnection _connection;
        //DataContext dataContext;
        //readonly DataContext _dataContext = dataContext;
        public string connectionString = "server=sample-instance.c53wji5mnp4g.ap-south-1.rds.amazonaws.com;User Id=Admin;Password=Jz7XXc8iqCHjJTL;database=sample;Trusted_Connection=True;TrustServerCertificate=Yes;Integrated Security=false;";
        List<InvoiceDetails> details = new List<InvoiceDetails>();
        List<ErrorLog> logs = new List<ErrorLog>();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SuccessIndex()
        {
            FetchSuccessData();
            return View(details);
        }
        public IActionResult ErrorIndex()
        {
            FetchErrorData();
            return View(logs);
        }

        public void FetchSuccessData()
        {
            try
            {
                //using (var ctx = new DataContext())
                //{
                //    var details = (from s in ctx.invoiceDetails
                //                   where s.Id == 1
                //                   select s).FirstOrDefault<InvoiceDetails>();
                //}
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT * FROM [sample].[dbo].[invoiceDetails]";

                    SqlDataReader dr;

                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        details.Add(new InvoiceDetails()
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            FileName = dr["FileName"].ToString(),
                            InvoiceNumber = dr["InvoiceNumber"].ToString(),
                            VendorName = dr["VendorName"].ToString(),
                            ReceiverName = dr["ReceiverName"].ToString(),
                            CreatedAt = Convert.ToDateTime(dr["CreatedAt"])
                        });
                    }
                    conn.Close();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }  
        }

        public void FetchErrorData()
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT * FROM [sample].[dbo].[ErrorLog]";

                    SqlDataReader dr;

                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        logs.Add(new ErrorLog()
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            FileName = dr["FileName"].ToString(),
                            CreatedAt = Convert.ToDateTime(dr["CreatedAt"])
                        });
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}