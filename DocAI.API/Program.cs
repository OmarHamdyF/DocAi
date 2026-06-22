using System.Text;
using DocAI.API.Data;
using DocAI.API.Middleware;
using DocAI.API.Services;
using DocAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<DocAIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is required in appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// HTTP Clients for external APIs
builder.Services.AddHttpClient("Icd10", c =>
    c.DefaultRequestHeaders.Add("Accept", "application/json"));
builder.Services.AddHttpClient("RxNorm", c =>
    c.DefaultRequestHeaders.Add("Accept", "application/json"));
builder.Services.AddHttpClient("Loinc", c =>
    c.DefaultRequestHeaders.Add("Accept", "application/json"));

// Application Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();
builder.Services.AddScoped<IIcd10Service, Icd10Service>();
builder.Services.AddScoped<IRxNormService, RxNormService>();
builder.Services.AddScoped<ILoincService, LoincService>();
builder.Services.AddScoped<ISnomedService, SnomedStubService>();
builder.Services.AddScoped<IComprehendMedicalService, ComprehendMedicalStubService>();
builder.Services.AddScoped<IUmlsService, UmlsStubService>();
builder.Services.AddScoped<IAuditEngineService, AuditEngineService>();

// CORS – allow any localhost origin in development
builder.Services.AddCors(options =>
    options.AddPolicy("Angular", policy =>
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()));

// Controllers + OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info.Title = "DocAI - Clinical Documentation Copilot";
        document.Info.Version = "v1";
        document.Info.Description = "AI-powered clinical documentation and insurance validation API";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DocAIDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opts =>
    {
        opts.Title = "DocAI API";
        opts.Theme = ScalarTheme.Purple;
    });
}

app.UseCors("Angular");

// Skip HTTPS redirection when running behind a reverse proxy (e.g. Docker + Nginx)
if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
