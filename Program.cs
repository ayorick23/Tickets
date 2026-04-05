using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tickets.Application.Services;
using Tickets.Domain.Interfaces;
using Tickets.Domain.Interfaces.Repositories;
using Tickets.Infraestructure.Identity;
using Tickets.Infraestructure.Persistence;
using Tickets.Infraestructure.Persistence.Repositories;
using Tickets.Infrastructure.Persistence.Repositories;
using Tickets.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<AppIdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Inyectando servicios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ITicketsRepository, TicketRepository>();
builder.Services.AddScoped<TicketsCase>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication(options=>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtIssuer"],
            ValidAudience = builder.Configuration["JwtAudience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Creando data por defecto
using (var scope = app.Services.CreateScope())
{
    var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
    var roles = new[] { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleRepository.RoleExistsAsync(role))
        {
            await roleRepository.CreateRole(role);
        }
    }

    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

    if (!await userRepository.UserExists("admin@admin.com"))
    {
        var result = userRepository.CreateUser(
            new Tickets.Domain.Entities.Usuario()
            {
                Email = "admin@admin.com",
                Password = "Admin123!",
                FirstName = "Admin",
                LastName = "Admin"
            }).Result;

        var resultUserToRole = userRepository.AddToRoleAsync(result, "Admin").Result;
    }
}

app.Run();
