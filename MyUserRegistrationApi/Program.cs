using Microsoft.EntityFrameworkCore;
using MyUserRegistrationApi.Data;
using MyUserRegistrationApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Додаткові сервіси
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Налаштування підключення до PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Використання Swagger у режимі розробки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ендпоінт для реєстрації користувачів
app.MapPost("/api/register", async (UserRegistrationDto userDto, AppDbContext dbContext) =>
{
    // Валідація даних
    if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password))
    {
        return Results.BadRequest("Username та Password є обов'язковими");
    }

    // Перевірка чи існує користувач з таким ім'ям
    if (await dbContext.Users.AnyAsync(u => u.Username == userDto.Username))
    {
        return Results.BadRequest("Користувач з таким ім'ям вже існує");
    }

    // Створення нового користувача (пароль потрібно хешувати у реальному проекті)
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

// DTO для отримання даних реєстрації
public record UserRegistrationDto(string Username, string Password);
