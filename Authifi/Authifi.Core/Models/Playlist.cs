using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Authifi.Core.Models
{
    public class Playlist
    {
        public string Name { set; get; }

        [Key]
        [Required]
        public int Id { set; get; }

        public bool Liked { set; get; }
        public virtual List<Song> Songs { set; get; }

        //public User User { set; get; }

        [ForeignKey("User")]
        public int UserID { set; get; }
    }
}
