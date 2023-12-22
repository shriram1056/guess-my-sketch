using GuessMySketch.Models;

namespace GuessMySketch.DTO
{
    public class CookieReadDto
    {

        public CookieDto? Data;


        public string? Message;


    }

    public class CookieDto
    {
        public string Code { get; set; } = null!;

        public string? Username { get; set; }

    }
}