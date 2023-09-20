using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuessMySketch.Models
{
    [Table("chat")]
    public class Chat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("value")]
        public string Value { get; set; }

        
        [Column("room_id")] // Change property name to match the desired column name
        [ForeignKey("room")]
        public string RoomId { get; set; }

        [Required]
        [Column("user_id")] // Change property name to match the desired column name
        [ForeignKey("user")]
        public Guid UserId { get; set; }

        [Required]
        public Room Room { get; set; }

        [Required]
        public User User { get; set; }
    }
}

// adding navigation property will automatically make associations

// However, that results in some defaults like the FK is defaulted to int. So, we need to create a new field RoomId with the appropriate type and since the key "code" is different than the FK (room_id) we need to explicitly say what it is in "Room.cs"
