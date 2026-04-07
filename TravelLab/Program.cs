using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelLab.Data;
using TravelLab.Models;

var builder = WebApplication.CreateBuilder(args);

// Configura il DbContext (già esistente)
builder.Services.AddDbContext<TravelLabContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Aggiungi Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<TravelLabContext>()
    .AddDefaultTokenProviders();

// Configura il cookie di autenticazione
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login.html";
    options.AccessDeniedPath = "/access-denied.html";
    options.Cookie.Name = "TravelLabAuth";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// Aggiungi autorizzazione
builder.Services.AddAuthorization();

// Altri servizi (CORS, Controllers, Swagger, ecc.)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();  // <-- importante: prima di Authorization
app.UseAuthorization();

app.UseCors("AllowAll");
app.MapControllers();

// Seed del database: crea ruoli e utente admin di default
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedDatabase(services);
}

app.Run();

async Task SeedDatabase(IServiceProvider services)
{
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Crea ruoli
    string[] roles = { "Admin", "Operatore" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Crea utente admin se non esiste
    var adminEmail = "admin@travelab.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = adminEmail,
            Nome = "Admin",
            Cognome = "TravelLab"
        };
        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}