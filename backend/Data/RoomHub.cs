using GuessMySketch.Data;
using GuessMySketch.DTO;
using GuessMySketch.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class RoomHub : Hub
{
    private readonly AppDbContext _context;
    public RoomHub(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task SendMessage(string message, int? countdown, bool special)
    {
        var data = await _context.Users.Include(user => user.Room).FirstOrDefaultAsync(user => user.ConnectionId == Context.ConnectionId);

        if (special)
        {
            await Clients.Group(data.RoomId).SendAsync("ReceiveMessage", new
            {
                message,
                special = true,
                name = data.Name
            });
        }
        else if (message == data.Room.Word && data.Room.CurrentDrawer != data.Name && countdown.HasValue)
        {
            await _context.Database.ExecuteSqlRawAsync("UPDATE public.room SET word = {0} WHERE code = {1}", null, data.RoomId);
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Name == data.Name);
            user.Score += countdown.Value;
            await _context.SaveChangesAsync();
            await Clients.Group(data.RoomId).SendAsync("ReceiveMessage", new
            {
                message = " has guessed the word correct",
                special = true,
                name = data.Name
            });
            await GetInfo(data.RoomId);
            await ClearCanvas(data.RoomId);
            await NextRound(data.Room.CurrentDrawer);
        }
        else
        {
            await Clients.Group(data.RoomId).SendAsync("ReceiveMessage", new
            {
                message,
                special = false,
                name = data.Name
            });
        }
    }

    public async Task SendCanvasData(Coordinate originalPosition, Coordinate newPosition, string roomId)
    {
        await Clients.OthersInGroup(roomId).SendAsync("ReceiveCanvasData", new CanvasData { originalPosition = originalPosition, newPosition = newPosition });
    }
    public async Task SendActiveColor(string activeColor, string roomId)
    {
        await Clients.OthersInGroup(roomId).SendAsync("ReceiveActiveColor", activeColor);
    }

    public async Task ClearCanvas(string roomId)
    {
        await Clients.Group(roomId).SendAsync("ClearCanvas");
    }

    public async Task NextRound(string username)
    {
        var data = await _context.Users.FirstOrDefaultAsync(user => user.Name == username);

        var user = await _context.Users
           .Where(user => user.CreatedAt > data.CreatedAt && user.RoomId == data.RoomId) // Select records with Id greater than the current user's Id
           .OrderBy(user => user.CreatedAt)
           .FirstOrDefaultAsync();


        if (user == null)
        {
            await EndGame(data.RoomId);
        }
        else
        {
            await _context.Database.ExecuteSqlRawAsync("UPDATE public.room SET game_started = {0}, current_drawer = {1} WHERE code = {2}", true, user.Name, user.RoomId);
            await Clients.Client(user.ConnectionId).SendAsync("CurrentDrawer", "your turn");
        }
    }

    public async Task GameStarted()
    {
        var data = await _context.Users.Include(user => user.Room).FirstOrDefaultAsync(user => user.ConnectionId == Context.ConnectionId);

        if (data.Room.Host == data.Name)
        {
            await _context.Database.ExecuteSqlRawAsync("UPDATE public.room SET game_started = {0}, current_drawer = {1} WHERE code = {2}", true, data.Name, data.RoomId);

            await Clients.Group(data.RoomId).SendAsync("GameStarted");

        }
        else { await Clients.Caller.SendAsync("error", "error: you're not the host"); }
    }

    public async Task NewWord(string word)
    {
        var data = await _context.Users.FirstOrDefaultAsync(user => user.ConnectionId == Context.ConnectionId);
        await _context.Database.ExecuteSqlRawAsync("UPDATE public.room SET word = {0} WHERE code = {1}", word, data.RoomId);
        await SendMessage("has choosen what they want to draw", null, true);
        await Clients.Groups(data.RoomId).SendAsync("ResetTimer", "your turn");
    }

    public async Task EndGame(string roomId)
    {
        var userWithHighestScore = _context.Users
    .Where(user => user.RoomId == roomId)
    .OrderByDescending(user => user.Score)
    .FirstOrDefault();
        await Clients.Group(roomId).SendAsync("EndGame", userWithHighestScore.Name);
    }

    public async Task GetInfo(string roomId)
    {
        var data = await _context.Users.OrderByDescending(user => user.Score)
    .Where(user => user.RoomId == roomId)
    .Select(user => new { Name = user.Name, Score = user.Score })
    .ToListAsync();
        await Clients.Group(roomId).SendAsync("GetInfo", data);
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var room = JsonConvert.DeserializeObject<RoomInfo>(httpContext.Request.Cookies["room"]);

        await _context.Database.ExecuteSqlRawAsync("UPDATE public.user SET connection_id = {0} WHERE name = {1}", Context.ConnectionId, room.username);
        await Groups.AddToGroupAsync(Context.ConnectionId, room.code);
        if (room.host)
        {
            Console.WriteLine(room.host);
            await Clients.Caller.SendAsync("host");
        }
        await SendMessage("has joined", null, true);
        await GetInfo(room.code);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var data = await _context.Users.FirstOrDefaultAsync(user => user.ConnectionId == Context.ConnectionId);

        await _context.Database.ExecuteSqlRawAsync("DELETE FROM public.user WHERE name = {0}", data.Name);

        var count = await _context.Users.CountAsync(user => user.RoomId == data.RoomId);

        if (count == 0)
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM room WHERE code = {0}", data.RoomId);
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, data.RoomId);
        // Perform actions when a user disconnects
        await base.OnDisconnectedAsync(exception);
    }
}
public class Coordinate
{
    public float x { get; set; }
    public float y { get; set; }
}

public class CanvasData
{
    public Coordinate originalPosition { get; set; } = null!;
    public Coordinate newPosition { get; set; } = null!;
}

public class RoomInfo
{
    public string code { get; set; }
    public string username { get; set; }
    public bool host { get; set; }
}