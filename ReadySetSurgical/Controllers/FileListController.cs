using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace ReadySetSurgical.Controllers
{
    public class FileListController : Controller
    {
        public string connectionString = "server=sample-instance.c53wji5mnp4g.ap-south-1.rds.amazonaws.com;User Id=Admin;Password=Jz7XXc8iqCHjJTL;database=sample;Trusted_Connection=True;TrustServerCertificate=Yes;Integrated Security=false;";
        List<InvoiceDetails> details = new List<InvoiceDetails>();
        public IActionResult Index()
        {
            FetchData();
            return View(details);
        }

        public void FetchData()
        {
            try
            {
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
    }
}
