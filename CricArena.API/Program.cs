using CricArena.API.Middleware;
using CricArena.Business.Services;
using CricArena.Business.Services.Interfaces;
using CricArena.Data.Context;
using CricArena.Data.Repositories;
using CricArena.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DbConnectionString"));
});

//It tells ASP.NET Core to use the authentication middleware and configure it to use JWT Bearer tokens for authentication. It also specifies the parameters for validating the JWT tokens, such as the issuer, audience, signing key, and lifetime validation. This ensures that only valid tokens issued by the specified issuer and intended for the specified audience are accepted by the application.
// Basically setting rules against which validation needs to be done
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //Add authentication to the app of type JWT auth (there are other types of auth also wheihc can be added such as (cookies, OAuth, OpenID Connect etc)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters  //What are the required things in the token that needs to be present and validated
        {
            ValidateIssuer = true, //CricArena
            ValidateAudience = true, //CricArenaUser
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();  //This is required to access the current user information in the services layer
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();  //This is required to access the current user information in the services layer

//Services registration
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClubService, ClubService>();

//Repository registration
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClubRepository, ClubRepository>();
builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();



var app = builder.Build();

// Configure the HTTP request pipeline.
// Use native OpenAPI with Scalar UI (.NET 10 compatible)
app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
