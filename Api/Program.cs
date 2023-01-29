using System.Text.Json.Serialization;
using Api.Data;
using Api.Models.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApiContext")));

builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<BillPayRepository>();
builder.Services.AddScoped<LoginRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Ignore JSON reference cycles during serialisation.
//builder.Services.AddControllers().AddJsonOptions(options =>
//    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed data - assumption: data has been seeded by McbaSystem
// using(var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     try
//     {
//         SeedData.Initialize(services);
//     }
//     catch(Exception ex)
//     {
//         var logger = services.GetRequiredService<ILogger<Program>>();
//         logger.LogError(ex, "An error occurred seeding the DB.");
//     }
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// .NET 6 Minimal APIs - Simple Example.
// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-7.0

// GET api/UsingMapGet?name=Matthew&repeat=100
app.MapGet("api/UsingMapGet", (string name, int? repeat) =>
{
    if (string.IsNullOrWhiteSpace(name))
        name = "(empty)";
    if (repeat is null or < 1)
        repeat = 1;

    return string.Join(' ', Enumerable.Repeat(name, repeat.Value));
});

app.Run();