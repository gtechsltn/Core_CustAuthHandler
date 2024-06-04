using Core_CustAuthHandler.CustomAutentication;
using Core_CustAuthHandler.Models;
using Core_CustAuthHandler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppSecurityContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppSecurityDbContext"));
});

builder.Services.AddScoped<IUserService, UserService>();

// Register the Custion Authentication Handler
builder.Services.AddAuthentication("BasicAuthentication")
        .AddScheme<AuthSchemeOptions, AuthHandler>("BasicAuthentication",null);
builder.Services.AddAuthorization();


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
// Add Security Middlewares
app.UseAuthentication();
app.UseAuthorization();

// Adding Endpoints
app.MapPost("/api/register", async (IUserService serv, User user) => { 
    await serv.CreateUserAsync(user);
    return Results.Ok   ($"The User: {user.UserName} is created successfully!!");
});

app.MapPost("/api/authuser", async(IUserService serv, User user) => { 

    var result  = await serv.AuthenticateUserAsync(user);
    
    return result;
});

// The Endpoint with Authentication call
app.MapGet("/api/sample", [Authorize] () => {
    return Results.Ok("The call is authenticated");
});



app.Run();

 