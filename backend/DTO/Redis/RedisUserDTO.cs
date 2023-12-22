using GuessMySketch.Models;

namespace GuessMySketch.DTO
{
    public class RedisRoomDTO
    {
        public string Word { get; set; } = null!;

    }
    public class RedisDTO
    {
        public string User { get; set; } = null!;
        public string Code { get; set; } = null!;
        public bool Host { get; set; }
    }
}