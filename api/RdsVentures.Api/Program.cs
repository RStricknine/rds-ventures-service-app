using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using RdsVentures.Api.Data;
using RdsVentures.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:5173",
            "https://localhost:3000",
            "https://localhost:5173"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=RdsVenturesDb;Trusted_Connection=true;TrustServerCertificate=true;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure Blob Storage
var blobConnectionString = builder.Configuration["Azure:BlobStorage:ConnectionString"];
if (!string.IsNullOrEmpty(blobConnectionString))
{
    builder.Services.AddSingleton(x => new BlobServiceClient(blobConnectionString));
}
else
{
    // Use default Azure credentials for production
    var blobServiceUri = builder.Configuration["Azure:BlobStorage:ServiceUri"];
    if (!string.IsNullOrEmpty(blobServiceUri))
    {
        builder.Services.AddSingleton(x => new BlobServiceClient(new Uri(blobServiceUri), new DefaultAzureCredential()));
    }
    else
    {
        // Fallback for local development
        builder.Services.AddSingleton(x => new BlobServiceClient("UseDevelopmentStorage=true"));
    }
}

builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

// Configure Application Insights
var appInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrEmpty(appInsightsConnectionString))
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = appInsightsConnectionString;
    });
}

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        await SeedData(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();

async Task SeedData(AppDbContext context)
{
    if (await context.Clients.AnyAsync())
        return; // Database already seeded

    // Seed Clients
    var clients = new[]
    {
        new RdsVentures.Api.Models.Client { Name = "Green Valley Properties", ContactEmail = "contact@greenvalley.com", Phone = "555-0100" },
        new RdsVentures.Api.Models.Client { Name = "Sunset Apartments LLC", ContactEmail = "info@sunsetapts.com", Phone = "555-0200" }
    };
    context.Clients.AddRange(clients);
    await context.SaveChangesAsync();

    // Seed Properties
    var properties = new[]
    {
        new RdsVentures.Api.Models.Property { ClientId = 1, Address = "123 Oak Street", City = "Springfield", State = "IL", Zip = "62701", Notes = "Main office building" },
        new RdsVentures.Api.Models.Property { ClientId = 1, Address = "456 Maple Avenue", City = "Springfield", State = "IL", Zip = "62702", Notes = "Residential complex" },
        new RdsVentures.Api.Models.Property { ClientId = 2, Address = "789 Pine Road", City = "Chicago", State = "IL", Zip = "60601", Notes = "Apartment building" }
    };
    context.Properties.AddRange(properties);
    await context.SaveChangesAsync();

    // Seed Technicians
    var technicians = new[]
    {
        new RdsVentures.Api.Models.Technician { Name = "John Smith", Email = "john.smith@rdsventures.com", HourlyRate = 45.00m },
        new RdsVentures.Api.Models.Technician { Name = "Sarah Johnson", Email = "sarah.johnson@rdsventures.com", HourlyRate = 50.00m }
    };
    context.Technicians.AddRange(technicians);
    await context.SaveChangesAsync();

    // Seed Service Requests
    var serviceRequests = new[]
    {
        new RdsVentures.Api.Models.ServiceRequest
        {
            PropertyId = 1,
            Title = "HVAC Maintenance",
            Description = "Annual HVAC system maintenance and filter replacement",
            Priority = RdsVentures.Api.Models.Priority.Medium,
            Status = RdsVentures.Api.Models.Status.Open,
            ScheduledAt = DateTime.UtcNow.AddDays(3),
            AssignedTechId = 1
        },
        new RdsVentures.Api.Models.ServiceRequest
        {
            PropertyId = 2,
            Title = "Plumbing Repair",
            Description = "Fix leaking faucet in unit 3B",
            Priority = RdsVentures.Api.Models.Priority.High,
            Status = RdsVentures.Api.Models.Status.InProgress,
            ScheduledAt = DateTime.UtcNow.AddDays(1),
            AssignedTechId = 2
        },
        new RdsVentures.Api.Models.ServiceRequest
        {
            PropertyId = 3,
            Title = "Electrical Inspection",
            Description = "Routine electrical safety inspection",
            Priority = RdsVentures.Api.Models.Priority.Low,
            Status = RdsVentures.Api.Models.Status.Open,
            ScheduledAt = DateTime.UtcNow.AddDays(7)
        }
    };
    context.ServiceRequests.AddRange(serviceRequests);
    await context.SaveChangesAsync();
}
