using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Owls_BE.Helper;
using Owls_BE.Repositories.BookRepos;
using Owls_BE.Repositories.CartRepos;
using Owls_BE.Repositories.CategoryRepos;
using Owls_BE.Repositories.DeliveryRepos;
using Owls_BE.Repositories.DiscountRepos;
using Owls_BE.Repositories.OrderRepos;
using Owls_BE.Repositories.ReviewRepos;
using Owls_BE.Repositories.UserRepos.AuthRepos;
using Owls_BE.Repositories.UserRepos.CustomerRepos;
using Owls_BE.Repositories.VoucherRepos;
using Owls_BE.Services.Image;
using Owls_BE.Services.PaymentSV;
using System.Text;

namespace Owls_BE
{
    public static class RegisterServices
    {
        public static WebApplicationBuilder RegisteredServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAuthRepos, AuthRepos>();
            builder.Services.AddScoped<IFileService, FileService>();

            builder.Services.AddScoped<ICategoryRepos, CategoryRepos>();
            builder.Services.AddScoped<IDiscountRepos, DiscountRepos>();
            builder.Services.AddScoped<IVoucherRepos, VoucherRepos>();
            builder.Services.AddScoped<IBookRepos, BookRepos>();
            builder.Services.AddScoped<IDeliveryRepos, DeliveryRepos>();
            builder.Services.AddScoped<IOrderRepos, OrderRepos>();
            builder.Services.AddScoped<IReviewRepos, ReviewRepos>();


            builder.Services.AddScoped<ICustomerRepos, CustomerRepos>();
            builder.Services.AddScoped<ICartRepos, CartRepos>();
            builder.Services.AddScoped<IPayment, PaymentSV>();

            var tokenOptions = builder.Configuration.GetSection("TokenOptions");
            var securityKey = tokenOptions["AccessTokenSecurityKey"];
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Manager", policy =>
                    policy.RequireRole(UserRole.Manager));
                options.AddPolicy("ProductAdmin", policy =>
                    policy.RequireRole(UserRole.ProductAdmin, UserRole.Manager));
                options.AddPolicy("OrderAdmin", policy =>
                    policy.RequireRole(UserRole.OrderAdmin, UserRole.Manager));
            });



            return builder;
        }
    }
}
