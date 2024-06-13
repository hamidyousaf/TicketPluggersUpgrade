using Humanizer.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;
using TicketServer.Helpers;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs.Settings;
using TP.Upgrade.Application.Services;
using TP.Upgrade.Application.Utils;
using TP.Upgrade.Domain.Models.DBEntity;
using TP.Upgrade.Infrastructure.DatabaseInitializers;
using TP.Upgrade.Infrastructure.DataManagers;
using TP.Upgrade.Infrastructure.DBContext;

namespace TP.Upgrade.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var connectionString = configuration.GetConnectionString("TPConnection");
        services.AddDbContext<TP_DbContext>(o => o.UseSqlServer(connectionString, op =>
        {
            op.CommandTimeout(120);
        }));
        services.AddIdentity<User, Roles>(option =>
        {
            option.User.RequireUniqueEmail = true;
            option.Password.RequireDigit = false;
            option.Password.RequiredLength = 8;
            option.Password.RequireNonAlphanumeric = true;
            option.Password.RequireUppercase = false;
            option.Password.RequireLowercase = false;
            option.SignIn.RequireConfirmedEmail = true;
        }).AddEntityFrameworkStores<TP_DbContext>()
        .AddDefaultTokenProviders();

        #region Register MongoDB client
        var mongoDbConnectionString = configuration.GetValue<string>("MongoDBSetting:ConnectionString").ToString();
        var mongoClient = new MongoClient(mongoDbConnectionString);
        services.AddSingleton<IMongoClient>(mongoClient);
        #endregion

        services.Configure<MongoDBSetting>(configuration.GetSection("MongoDBSetting"));
        services.Configure<OrderSetting>(configuration.GetSection("OrderSetting"));

        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

        services.AddAuthentication(auth =>
        {
            auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
#pragma warning disable CS8604 // Possible null reference argument.
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = configuration["AuthSettings:Audience"],
                ValidIssuer = configuration["AuthSettings:Issuer"],
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthSettings:Key"])),
                ValidateIssuerSigningKey = true
            };
#pragma warning restore CS8604 // Possible null reference argument.
        });

        #region Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventStatusRepository, EventStatusRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddScoped<ICustomerFavouriteRepository, CustomerFavouriteRepository>();
        services.AddScoped<ISearchKeyRepository, SearchKeyRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ISpecificationAttributeOptionRepository, SpecificationAttributeOptionRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISplitTicketOptionRepository, SplitTicketOptionRepository>();
        services.AddScoped<ICustomerOrderRepository, CustomerOrderRepository>();
        services.AddScoped<IReportProblemRepository, ReportProblemRepository>();
        services.AddScoped<IProductSpecificationAttributeRepository, ProductSpecificationAttributeRepository>();
        services.AddScoped<IFeeRepository, FeeRepository>();
        services.AddScoped<IProductDocumentRepository, ProductDocumentRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ITaxRepository, TaxRepository>();
        #endregion

        #region Services
        services.AddScoped<IUserService, UserService>();
        services.AddTransient<IMailService, SendGridMailService>();
        services.AddTransient<IEventService, EventService>();
        services.AddTransient<IEventStatusService, EventStatusService>();
        services.AddTransient<ICategoryService, CategoryService>();
        services.AddTransient<IVenueService, VenueService>();
        services.AddTransient<ICustomerFavouriteService, CustomerFavouriteService>();
        services.AddTransient<ISearchKeyService, SearchKeyService>();
        services.AddTransient<ICurrencyService, CurrencyService>();
        services.AddTransient<ICustomerService, CustomerService>();
        services.AddTransient<IAddressService, AddressService>();
        services.AddTransient<ISharedService, SharedService>();
        services.AddTransient<ISpecificationAttributeOptionService, SpecificationAttributeOptionService>();
        services.AddTransient<ISplitTicketOptionService, SplitTicketOptionService>();
        services.AddTransient<ICustomerOrderService, CustomerOrderService>();
        services.AddTransient<IProductService, ProductService>();
        services.AddTransient<IReportProblemService, ReportProblemService>();
        services.AddTransient<IFileHelper, FileHelper>();
        services.AddTransient<IProductSpecificationAttributeService, ProductSpecificationAttributeService>();
        services.AddTransient<INotificationService, NotificationService>();
        services.AddSingleton<PresenceTracker>(); 
        #endregion

        #region Register automapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        #endregion
        services.AddSignalR();

        return services;
    }
}