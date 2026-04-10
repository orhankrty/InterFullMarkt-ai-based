using InterFullMarkt.Application;
using InterFullMarkt.Infrastructure;
using InterFullMarkt.WebUI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Configuration from appsettings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' bulunamadı.");

// 📦 Register Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

// 🎨 Add MVC support
builder.Services.AddControllersWithViews();

// 📝 Logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// 🔍 Global Exception Handling Middleware (FIRST MIDDLEWARE)
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// 📋 Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Development'ta daha detaylı hata sayfası
    app.UseDeveloperExceptionPage();
}

// 🗄️ Initialize database and apply migrations
app.UseInfrastructure();

// 🌐 HTTP Middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();

// 🔐 Authorization
app.UseAuthorization();

// 🛣️ Route configurations
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Players}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "error",
    pattern: "Error/{statusCode}");

// 🚀 Start application
_=app.RunAsync();
app.Run();
