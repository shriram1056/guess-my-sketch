using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuessMySketch.Models
{
    [Table("room")]
    public class Room
    {
        [Key]
        [Column("code")]
        public string Code { get; set; }

        [Column("active")]
        public bool Active {get; set;}

        public ICollection<User> Users { get; set; } =  new List<User>();

        [ForeignKey("RoomId")]
        public ICollection<Chat> Chats { get; set; } =  new List<Chat>();
    }
}

// adding nvigation property will automatically make associations

// To allow for the addition of Users and Chats to a Room, it's common to initialize the collection with an empty list when defining the property. This way, when you create a new Room instance, it starts with an empty list of tags.