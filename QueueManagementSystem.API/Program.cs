using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QueueManagementSystem.Domain.Entities;
using QueueManagementSystem.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


IServiceCollection serviceCollection = builder.Services.AddDbContext<QueueDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));//to get connect sql in app.json
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
    options =>
    {
        //options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequiredLength = 9;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.User.RequireUniqueEmail = true;
    }).AddEntityFrameworkStores<QueueDbContext>().AddDefaultTokenProviders();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
