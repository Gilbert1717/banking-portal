using System.Net.Http.Headers;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

// Store session into Web-Server memory.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Make the session cookie essential.
    options.Cookie.IsEssential = true;
});

// Configure api client.
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("http://localhost:5200");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapDefaultControllerRoute();

app.Run();