using System.Data;
using System.Data.SqlClient;
using WebEmployeesDPepelyaev.Data;
using WebEmployeesDPepelyaev.Entitys;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IDbConnection>(provider =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<IDataWork ,DataWork>();
builder.Services.AddTransient<CompanyRepository>();
builder.Services.AddTransient<DepartmentRepository>();
builder.Services.AddTransient<EmployeeRepository>();
builder.Services.AddTransient<PassportRepository>();
builder.Services.AddTransient<PassportTypeRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");    
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


