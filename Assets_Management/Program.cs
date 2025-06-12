using AssetManagement_DataAccess;
using Assets_Management.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

//-------------------- Register SQL_DB -----------------------------
builder.Services.AddScoped<SQL_DB>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new SQL_DB(configuration.GetConnectionString("DefaultConnectionBeta"));
});

//-------------------- Dynamic DI via IServiceRegister Interface -----------------------------
//builder.Services.Scan(scan => scan
//    .FromAssembliesOf(typeof(IServiceRegister))
//    .AddClasses(classes => classes.AssignableTo<IServiceRegister>())
//    .AsSelfWithInterfaces()
//    .WithScopedLifetime()
//);

//------------------- Injecting Dependency for all Data Access Classes Manually

builder.Services.AddScoped<DashboardReports>();
builder.Services.AddScoped<CallLogs>();
builder.Services.AddScoped<AssetDetailsAndReports>();

//-------------------- Dependency injection for ApiConnection service Services -----------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ApiConnect>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

//-------------------- Middleware -----------------------------
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
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
