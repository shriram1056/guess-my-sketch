using GuessMySketch.DTO;
using GuessMySketch.Models;

namespace GuessMySketch.Data
{
    public interface IRoomRepo
    {
        Task<CookieReadDto>? CreateRoom(string name);
        Task<CookieReadDto>? JoinRoom(JoinRoomDto joinRoomDto);
    }
}