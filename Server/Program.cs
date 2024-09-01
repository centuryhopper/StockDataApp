

/*


dotnet ef dbcontext scaffold "INSERT_CONNECTION_STRING_HERE" Npgsql.EntityFrameworkCore.PostgreSQL -o Contexts

to test api in swagger:
    run "dotnet watch run"

to test entire app from blazor wasm:
    run "dotnet watch run --launch-profile https"


*/

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Server.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options=>{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });
});

// Configure the Identity database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Environment.IsDevelopment()
                ?
                    builder.Configuration.GetConnectionString("UserManagementDB")
                :
                    Environment.GetEnvironmentVariable("UserManagementDB"))
        );

builder.Services.AddDbContext<StockDataDbContext>(options =>
{
    options.UseNpgsql(builder.Environment.IsDevelopment()
                ?
                    builder.Configuration.GetConnectionString("StockDataDB")
                :
                    Environment.GetEnvironmentVariable("StockDataDB")
                ).EnableSensitiveDataLogging();
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddRoles<ApplicationRole>();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 10;
    options.Password.RequiredUniqueChars = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = true;
});

if (!builder.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8081";
    builder.WebHost.UseUrls($"http://*:{port}");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");


app.Run();
