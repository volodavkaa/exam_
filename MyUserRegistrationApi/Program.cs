using Microsoft.EntityFrameworkCore;
using MyUserRegistrationApi.Data;
using MyUserRegistrationApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/register", async (UserRegistrationDto userDto, AppDbContext dbContext) =>
{

    if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password))
    {
        return Results.BadRequest("Username та Password є обов'язковими");
    }
    if (await dbContext.Users.AnyAsync(u => u.Username == userDto.Username))
    {
        return Results.BadRequest("Користувач з таким ім'ям вже існує");
    }

    var user = new User
    {
        Username = userDto.Username,
        Password = userDto.Password
    };

    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();

    return Results.Ok("Користувача успішно зареєстровано");
});

app.Run();

public record UserRegistrationDto(string Username, string Password);
