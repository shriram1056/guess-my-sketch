using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuessMySketch.Models
{
    [Table("room")]
    public class Room
    {
        [Key]
        [Column("code")]
        [Required]
        public string Code { get; set; } = null!;

        [Column("word")]
        public string? Word { get; set; } = null!;

        [Column("current_drawer")]
        public string? CurrentDrawer { get; set; } = null!;

        [Column("host_id")]
        public string? HostId { get; set; } = null!;

        [Column("game_started")]
        public bool GameStarted { get; set; }

        public ICollection<User>? Users { get; set; } = new List<User>();

    }
}

// adding nvigation property will automatically make associations

// To allow for the addition of Users and Chats to a Room, it's common to initialize the collection with an empty list when defining the property. This way, when you create a new Room instance, it starts with an empty list of tags.