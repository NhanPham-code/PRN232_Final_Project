using Service.BaseService;
using Service.Interfaces;
using Service.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
// Add services to the container.
builder.Services.AddControllersWithViews();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session tồn tại trong 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HttpClient
builder.Services.AddHttpClient<GatewayHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7112/");
});

// Dependency Injection
builder.Services.AddScoped<Service.Interfaces.IUserService, Service.Services.UserService>();
builder.Services.AddScoped<Service.Interfaces.IProductService, Service.Services.ProuctService>();
builder.Services.AddScoped<Service.Interfaces.ICartService, Service.Services.CartService>();
builder.Services.AddScoped<Service.Interfaces.IOrderService, Service.Services.OrderService>();  

builder.Services.AddScoped<Service.Interfaces.IFeedbackService, Service.Services.FeedbackService>();
builder.Services.AddScoped<Service.Services.EmailService>();
builder.Services.AddScoped<Service.Interfaces.INotificationService, Service.Services.NotificationService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession(); // Sử dụng Session

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
