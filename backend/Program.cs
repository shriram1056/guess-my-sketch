using Newtonsoft.Json;
using GuessMySketch.Data;
using GuessMySketch.DTO;
using GuessMySketch.Models;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
    {
        builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithOrigins("http://localhost:3000");
    }));

builder.Services.AddScoped<IRoomRepo, RoomRepo>();

builder.Services.AddSignalR(opts => opts.KeepAliveInterval = TimeSpan.FromSeconds(15)).AddMessagePackProtocol()
        .AddStackExchangeRedis(o =>
        {
            o.Configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("RedisConnectionString"));
            o.Configuration.ChannelPrefix = "backend";
            o.Configuration.AbortOnConnectFail = false;
        });


builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase")));

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

app.UseCors("CorsPolicy");

app.UseRouting();


app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<RoomHub>("/room", options =>
    {
        options.Transports = HttpTransportType.WebSockets;
    });
});

app.MapPost("/createRoom", async (HttpContext context, IRoomRepo repo, [FromBody] CreateRoomDto createRoomDto) =>
{

    CookieReadDto session = await repo.CreateRoom(createRoomDto.name);

    if (session != null && session.Data != null)
    {
        // Set the JSON string as a cookie value
        context.Response.Cookies.Append("room", JsonConvert.SerializeObject(new { code = session.Data.Code, username = session.Data.Username, host = true }), new CookieOptions { Expires = DateTime.Now.AddHours(24), Secure = true, HttpOnly = true, SameSite = SameSiteMode.None });

        return Results.Created($"/users/{session.Data.Code}", new { code = session.Data.Code, username = session.Data.Username });
    }
    else if (session != null && session.Message != null)
    {
        return Results.BadRequest(new { error = session.Message });
    }

    return Results.BadRequest(new { error = "something went wrong" });
});

app.MapPost("/joinRoom", async (HttpContext context, IRoomRepo repo, [FromBody] JoinRoomDto joinRoomDto) =>
{

    CookieReadDto session = await repo.JoinRoom(joinRoomDto);

    if (session != null && session.Data != null)
    {

        // Set the JSON string as a cookie value
        context.Response.Cookies.Append("room", JsonConvert.SerializeObject(new { code = session.Data.Code, username = session.Data.Username, host = false }), new CookieOptions { Expires = DateTime.Now.AddHours(24), Secure = true, HttpOnly = true, SameSite = SameSiteMode.None });

        return Results.Created($"/users/{session.Data.Code}", new { code = session.Data.Code, username = session.Data.Username });
    }
    else if (session != null && session.Message != null)
    {
        return Results.BadRequest(new { error = session.Message });
    }

    return Results.BadRequest(new { error = "something went wrong" });
});

app.UseAuthorization();

app.MapControllers();
app.Run();
