using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PaymentDemo.Manage.Configurations.Mapper;
using PaymentDemo.Manage.Data;
using PaymentDemo.Manage.Enums;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Repositories.Abstracts;
using PaymentDemo.Manage.Repositories.Implements;
using PaymentDemo.Manage.Services.Abstractions;
using PaymentDemo.Manage.Services.Implements;

namespace PaymentDemo.Manage.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationAutoMapper(this IServiceCollection services)
        {
            return services.AddAutoMapper(typeof(ServiceProfile));
        }

        public static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
        {            
            return services.AddValidatorsFromAssemblyContaining<ProductViewModelValidator>();
        }

        public static IServiceCollection AddApplicationServicesConfig(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICartService, CartService>();            
            services.AddScoped<IUserInfoService, UserInfoService>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<ICommonService, CommonService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();

            return services;
        }

        public static IServiceCollection AddApplicationRepositoriesConfig(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static void AddPaymentDemoDBContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PaymentDemoConnectionString");

            if (connectionString == null) return;

            services.AddDbContext<PaymentDBContext>(opt => opt.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(PaymentDBContext).Assembly.FullName)));
        }

        public static IServiceCollection AddHttpClientFactoryConfig(this IServiceCollection services)
        {
            services.AddHttpClient(PaymentProvider.Paypal.ToString(), x =>
            {
                x.BaseAddress = new Uri("");
            });

            services.AddHttpClient(PaymentProvider.Adyen.ToString(), x =>
            {
                x.BaseAddress = new Uri("");
            });

            return services;
        }
    }
}
