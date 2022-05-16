using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Authifi.Core.Models
{
    public class Song
    {
        [Key]
        [Required]
        public string Hash { set; get; }
        public string Lyrics { set; get; }

        public string Title { set; get; }
        public string Artist { set; get; }
        public string Duration { set; get; }

        //public Playlist playlist { set; get; }


        [ForeignKey("Playlist")]
        public int PlaylistID { set; get; }

    }
}
