using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using ClinicManager.Data;
using ClinicManager.Services;

var builder = WebApplication.CreateBuilder(args);

// NLog – logowanie do plików (/logs/errors.log, /logs/all.log)
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Rejestracja serwisów domenowych
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<IVisitService, VisitService>();

// Dostęp do HttpContext (potrzebny do audytu RODO – identyfikacja użytkownika)
builder.Services.AddHttpContextAccessor();

// dodałem Role, wyłączyłem wymóg potwierdzenia email (development)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>() // 👈 dodaje obsługę ról: Admin, Lekarz, Rejestratorka
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

// Rejestracja kontrolerów API (PatientsController, VisitsController, ...)
builder.Services.AddControllers();

builder.Services.AddScoped<IProcedureService, ProcedureService>();

// Swagger / OpenAPI – dokumentacja endpointów z autoryzacją przez cookie
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "ClinicManager API", Version = "v1" });

    // Dodaje przycisk "Authorize" w Swagger UI
    options.AddSecurityDefinition("cookie", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Cookie,
        Name = ".AspNetCore.Identity.Application" // nazwa cookie Identity
    });

    // Wymaga ciasteczka dla wszystkich endpointów z [Authorize]
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "cookie"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    // Swagger UI dostępne tylko w trybie developerskim
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// Mapowanie kontrolerów API (atrybuty [Route] i [ApiController])
app.MapControllers();

// Seed danych testowych (role + konta) – tylko przy pierwszym uruchomieniu
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

app.Run();
