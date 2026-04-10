using InterFullMarkt.Application;
using InterFullMarkt.Infrastructure;
using InterFullMarkt.WebUI.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Configuration from appsettings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' bulunamadı.");

// 📦 Register Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

// 🩺 Health Checks Kaydı (Sistemin canlılığını izlemek için)
builder.Services.AddHealthChecks();

// 🎨 Add MVC support
builder.Services.AddControllersWithViews();

// 🔐 Add Authentication (Cookie based)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "InterFullMarkt.Auth";
    });

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
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<InterFullMarkt.Infrastructure.Data.InterFullMarktDbContext>();
        await dbContext.Database.MigrateAsync();

        // 🦁 Seed Galatasaray Data (Tabloları ve Efsane Kadroyu Doldur)
        await InterFullMarkt.Infrastructure.Data.Seeds.DbInitializer.SeedGalatasarayAsync(dbContext);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(ex, "Veritabanı oluşturulurken veya Migration uygulanırken kritik bir hata oluştu!");
    }
}

// 🌐 HTTP Middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();

// 🔐 Authorization
app.UseAuthentication();
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

// 🩺 Health Check Uç Noktası (DevOps ve Monitoring araçları için /health)
app.MapHealthChecks("/health");

// 🚀 Start application
app.Run();
