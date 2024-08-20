using Crawler;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.OpenApi.Models;
using Owls_BE;
using Owls_BE.Helper;
using Owls_BE.Models;
using Owls_BE.Services.RecommenderModel;
using Payment;
using Recommender;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args).RegisteredServices();
builder.Services.AddHttpClient();
builder.Services.AddPaymentDI(builder.Configuration);
builder.Services.AddCrawlService();

builder.Services.AddSingleton<IRecommenderByUser>(provider =>
{
    var context = new MLContext();
    return new RecommenderByUser(context);
});

/*builder.Services.AddSingleton<IRecommenderByProduct>(provider =>
{
    var context = new MLContext();
    return new RecommenderByProduct(context);
});*/

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

});

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddDbContext<Owls_BookContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("DB")); });
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHostedService<ModelTrainingService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
         builder =>
         {
             builder.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
         });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Owls API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT accesstoken",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",

    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme, Id = "Bearer"},
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    };
    c.AddSecurityRequirement(securityRequirement);
});




var app = builder.Build();

await LocationService.InitializeAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Manage",
    pattern: "{area:exits}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllers();

app.Run();
