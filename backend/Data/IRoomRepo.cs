using GuessMySketch.DTO;
using GuessMySketch.Models;

namespace GuessMySketch.Data
{
    public interface IRoomRepo
    {
        Task<SessionReadDto>? CreateRoom(string name);
        Task<SessionReadDto>? JoinRoom(JoinRoomDto joinRoomDto);
    }
}