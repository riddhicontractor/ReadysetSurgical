using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using ReadySetSurgical.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAWSService<IAmazonS3>();
string connectionString = "server=sample-instance.c53wji5mnp4g.ap-south-1.rds.amazonaws.com;User Id=Admin;Password=Jz7XXc8iqCHjJTL;database=sample;Trusted_Connection=True;TrustServerCertificate=Yes;Integrated Security=false;";
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
