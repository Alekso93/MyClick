using NLog;
using NLog.Web;
using Eshop.Web.Data;
using EShop.Domain.Identity;
using EShop.Repository.Implementation;
using EShop.Repository.Interface;
using EShop.Service.Implementation;
using EShop.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;
using System.Drawing.Text;
using EShop.Domain;
using System.Configuration;
using EShop.Service;
using Stripe;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    EmailSettings emailService = new EmailSettings();
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.GetSection("EmailSettings").Bind(emailService);

    StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSettings")["SecretKey"];

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString, b => b.MigrationsAssembly("EShop.Repository")));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddDefaultIdentity<EShopApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();

    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
    builder.Services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));
    //builder.Services.AddScoped<IEmailService>(email => new EmailService(emailService));
    //builder.Services.AddScoped<EmailSettings>(ees => emailService);
    //builder.Services.AddScoped<IBackgroundEmailSender,BackgroundEmailSender>();
    //builder.Services.AddHostedService<ConsumeScopedHostedService>();

    builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));


    builder.Services.AddTransient<IProductService, EShop.Service.Implementation.ProductService>();
    builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
    builder.Services.AddTransient<IOrderService, OrderService>();

    builder.Services.AddControllersWithViews()
        .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
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
    app.MapRazorPages();

    app.Run();
}
catch (Exception exception)
{
    //logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
