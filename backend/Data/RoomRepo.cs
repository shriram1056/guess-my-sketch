using GuessMySketch.DTO;
using GuessMySketch.Helpers;
using GuessMySketch.Models;
using Microsoft.EntityFrameworkCore;

namespace GuessMySketch.Data
{
    public class RoomRepo : IRoomRepo
    {
        private readonly AppDbContext _context;

        public RoomRepo(AppDbContext context) // this is registered in startup.cs as dbContext
        {
            _context = context;
        }

        public async Task<SessionReadDto>? CreateRoom(string name)
        {
            string code = Util.RandomCode();
            var room = await _context.Rooms.FirstOrDefaultAsync(room => room.Code == code);

            while (room != null)
            {
                code = Util.RandomCode();
                room = await _context.Rooms.FirstOrDefaultAsync(room => room.Code == code);
            }

            int rowsAffected = await _context.Database.ExecuteSqlRawAsync("INSERT INTO room (code) VALUES ({0})", code);

            try
            {
                if (rowsAffected > 0)
                {
                    // If rows were inserted successfully, you can retrieve the inserted record
                    Room? insertedRoom = await _context.Rooms.FirstOrDefaultAsync(room => room.Code == code);

                    if (insertedRoom != null)
                    {
                        await _context.Database.ExecuteSqlRawAsync("INSERT INTO public.user (name, room_id) VALUES ({0}, {1})", name, code);

                        User? insertedUser = await _context.Users.FirstOrDefaultAsync(user => user.Name == name && user.RoomId == code);

                        if (insertedUser != null)
                        {
                            return new SessionReadDto { Data = new SessionData { Room = insertedRoom, UserId = insertedUser.Id, Name = name } };
                        }
                    }

                }
            }
            catch (Npgsql.PostgresException e)
            {
                return new SessionReadDto { Message = e.SqlState };

            }

            return null;
        }

        public async Task<SessionReadDto>? JoinRoom(JoinRoomDto joinRoomDto)
        {
            var room = await _context.Rooms.Include(room => room.Users).FirstOrDefaultAsync(room => room.Code == joinRoomDto.code);

            if (room != null)
            {
                if (room.GameStarted)
                {
                    return new SessionReadDto { Message = "the game has already started" };
                }


                try
                {
                    await _context.Database.ExecuteSqlRawAsync("INSERT INTO public.user (name, room_id) VALUES ({0}, {1})", joinRoomDto.name, joinRoomDto.code);

                    User? insertedUser = await _context.Users.FirstOrDefaultAsync(user => user.Name == joinRoomDto.name && user.RoomId == joinRoomDto.code);

                    if (insertedUser != null)
                    {
                        room.Users.Add(insertedUser);
                        return new SessionReadDto { Data = new SessionData { Room = room, UserId = insertedUser.Id, Name = joinRoomDto.name } };
                    }

                }
                catch (Npgsql.PostgresException e)
                {
                    if (e.SqlState == "23505")
                    {
                        return new SessionReadDto { Message = "the user name is taken" };
                    }
                }
            }
            return null;
        }

    }
}