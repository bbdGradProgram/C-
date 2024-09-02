using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Services;
using Server.Config;
using Microsoft.AspNetCore.Authentication;
using Amazon.Lambda;
using Amazon.CloudWatchEvents;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<WebsiteMonitorContext>(options =>
{

        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var database = Environment.GetEnvironmentVariable("DB_NAME");
        var username = Environment.GetEnvironmentVariable("DB_USER");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var connectionString = $"Host={host};Database={database};Username={username};Password={password}";
        options.UseNpgsql(connectionString);

});


builder.Services.AddScoped<IWebsiteService, WebsiteService>();
builder.Services.AddScoped<IMonitorLogService, MonitorLogService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMonitorService, MonitorService>();
builder.Services.AddAutoMapper(typeof(MappingProfile)); // Register AutoMapper
// Configure AWS Options from appsettings.json or environment variables
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonLambda>();
builder.Services.AddAWSService<IAmazonCloudWatchEvents>();
builder.Services.AddHttpClient(); 
builder.Services.AddAuthentication("GitHubOAuth")
                .AddScheme<AuthenticationSchemeOptions, GitHubOAuthHandler>("GitHubOAuth", options => {});
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Website Monitor API", Version = "v1" });
    
    // Define the OAuth2 scheme
    c.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.OAuth2,
        Flows = new Microsoft.OpenApi.Models.OpenApiOAuthFlows
        {
            AuthorizationCode = new Microsoft.OpenApi.Models.OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://github.com/login/oauth/authorize"),
                TokenUrl = new Uri("https://localhost:7212/api/auth/callback"),
                Scopes = new Dictionary<string, string>
                {
                    { "read:user", "Read access to user profile" },
                    { "user:email", "Read access to user email" }
                    
                }
            }
        }
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { "read:user" }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.OAuthAppName("Website Monitor");
        c.OAuth2RedirectUrl("https://localhost:7212/api/auth/callback");
    });
}

app.UseHttpsRedirection();


app.MapControllers();

app.Run();
