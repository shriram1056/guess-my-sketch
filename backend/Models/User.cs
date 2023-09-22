using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace GuessMySketch.Models
{
    [Table("user")]
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; } = null!;

        [Column("score")]
        [Required]
        public int Score { get; set; } = 0;

        [Column("creationTimeStamp")]
        [Required]
        public DateTime CreationTimestamp { get; set; }

        [Column("room_id")]
        [ForeignKey("room")]
        public string? RoomId { get; set; }

        [Required]
        [JsonIgnore]
        public Room? Room { get; set; }

    }
}

// making the fk required would mean the deletion of User when the Room is deleted

// adding nvigation property will automatically make associations
