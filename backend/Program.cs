using Newtonsoft.Json;
using GuessMySketch.Data;
using GuessMySketch.DTO;
using GuessMySketch.Models;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
    {
        builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins("http://localhost:3000");
    }));

builder.Services.AddScoped<IRoomRepo, RoomRepo>();

builder.Services.AddSignalR();

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
    endpoints.MapHub<RoomHub>("/chatHub", options =>
    {
        options.Transports = HttpTransportType.WebSockets;
    });
});

app.MapPost("/createRoom", async (HttpContext context, IRoomRepo repo, [FromBody] CreateRoomDto createRoomDto) =>
{

    SessionReadDto session = await repo.CreateRoom(createRoomDto.name);

    if (session != null && session.Data != null)
    {
        // Serialize the object to JSON

        Console.WriteLine(session);

        string room = JsonConvert.SerializeObject(session.Data.Room, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

        // Set the JSON string as a cookie value
        context.Response.Cookies.Append("room", room);

        context.Response.Cookies.Append("user_id", session.Data.UserId.ToString());
        context.Response.Cookies.Append("name", session.Data.Name);

        return Results.Created($"/users/{session.Data.Room.Code}", session);
    }
    else if (session != null && session.Message != null)
    {
        return Results.BadRequest(new { error = session.Message });
    }

    return Results.BadRequest(new { error = "something went wrong" });
});

app.MapPost("/joinRoom", async (HttpContext context, IRoomRepo repo, [FromBody] JoinRoomDto joinRoomDto) =>
{

    SessionReadDto session = await repo.JoinRoom(joinRoomDto);

    if (session != null && session.Data != null)
    {
        // Serialize the object to JSON

        Console.WriteLine(session);

        string room = JsonConvert.SerializeObject(session.Data.Room, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

        // Set the JSON string as a cookie value
        context.Response.Cookies.Append("room", room);

        context.Response.Cookies.Append("user_id", session.Data.UserId.ToString());
        context.Response.Cookies.Append("name", session.Data.Name);

        return Results.Created($"/users/{session.Data.Room.Code}", session);
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
