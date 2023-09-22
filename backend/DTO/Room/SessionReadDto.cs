using GuessMySketch.Models;

namespace GuessMySketch.DTO
{
    public class SessionReadDto
    {

        public SessionData? Data;


        public string? Message;


    }

    public class SessionData
    {
        public Room Room { get; set; } = null!;

        public int UserId { get; set; }

        public string Name { get; set; } = null!;
    }
}