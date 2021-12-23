using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Flocon.Mailing;
using Flocon.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Set the Mongo Database and add DB services
var DBsettings = builder.Configuration.GetSection(nameof(FloconDbSettings));
builder.Services.Configure<FloconDbSettings>(DBsettings);
builder.Services.AddSingleton<IFloconDbSettings>(sp =>
    sp.GetRequiredService<IOptions<FloconDbSettings>>().Value);
builder.Services.AddSingleton<CustomersService>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();


// Manage identities
builder.Services.AddIdentityMongoDbProvider<UserFlocon, MongoRole>(identity =>
    {
        // Sets password requirements
        identity.Password.RequiredLength = 8;
        identity.Password.RequireDigit = true;
        identity.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        identity.Lockout.MaxFailedAccessAttempts = 5;
    },
    mongo =>
    {
        mongo.ConnectionString = DBsettings.GetValue<string>("ConnectionString");
        mongo.MigrationCollection = DBsettings.GetValue<string>("AuthMigrationsCollectionName");
        mongo.UsersCollection = DBsettings.GetValue<string>("UsersCollectionName");
        mongo.RolesCollection = DBsettings.GetValue<string>("RolesCollectionName");
    }
);

// This is required to ensure server can identify user after login
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    // ToDo : Check that
    //options.LoginPath = "/Identity/Account/Login";
    //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
