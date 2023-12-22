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

        public async Task<CookieReadDto>? CreateRoom(string name)
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
                        Console.WriteLine(insertedRoom);
                        await _context.Database.ExecuteSqlRawAsync("INSERT INTO public.user (name, room_id) VALUES ({0}, {1})", name, code);

                        User? insertedUser = await _context.Users.FirstOrDefaultAsync(user => user.Name == name && user.RoomId == code);
                        await _context.Database.ExecuteSqlRawAsync("UPDATE public.room SET host = {0} WHERE code = {1}", insertedUser.Name, code);

                        if (insertedUser != null)
                        {
                            return new CookieReadDto { Data = new CookieDto { Username = insertedUser.Name, Code = insertedRoom.Code } };
                        }
                    }

                }
            }
            catch (Npgsql.PostgresException e)
            {
                Console.WriteLine(e);
                return new CookieReadDto { Message = e.SqlState };

            }

            return null;
        }

        public async Task<CookieReadDto>? JoinRoom(JoinRoomDto joinRoomDto)
        {
            var room = await _context.Rooms.Include(room => room.Users).FirstOrDefaultAsync(room => room.Code == joinRoomDto.code);

            if (room != null)
            {
                if (room.GameStarted)
                {
                    return new CookieReadDto { Message = "the game has already started" };
                }


                try
                {
                    await _context.Database.ExecuteSqlRawAsync("INSERT INTO public.user (name, room_id) VALUES ({0}, {1})", joinRoomDto.name, joinRoomDto.code);

                    User? insertedUser = await _context.Users.FirstOrDefaultAsync(user => user.Name == joinRoomDto.name && user.RoomId == joinRoomDto.code);

                    if (insertedUser != null)
                    {
                        room.Users.Add(insertedUser);
                        return new CookieReadDto { Data = new CookieDto { Username = insertedUser.Name, Code = room.Code } };
                    }

                }
                catch (Npgsql.PostgresException e)
                {
                    if (e.SqlState == "23505")
                    {
                        return new CookieReadDto { Message = "the user name is taken" };
                    }
                }
            }
            return null;
        }

    }
}