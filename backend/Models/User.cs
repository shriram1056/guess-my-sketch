using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuessMySketch.Models
{
    [Table("user")]
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
        
        [Column("score")]
        public string Score { get; set; }

        [Required]
        [ForeignKey("room_id")]
        public Room Room { get; set; }

        public ICollection<Chat> Chats { get; set; }
    }
}

// making the fk required would mean the deletion of User when the Room is deleted

// adding nvigation property will automatically make associations
