using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.Sqlite;
//using Microsoft.Data.Sqlite.Internal;

namespace Authifi.Core.Models
{
    // This class contains user members to download user information from Microsoft Graph
    // https://docs.microsoft.com/graph/api/resources/user?view=graph-rest-1.0
    public class User
    {

        [Key]
        [Required]
        public int Id { get; set; }

        public string GivenName { get; set; }

        
        public string Mail { get; set; }

       

        
        public virtual List<Playlist> Playlists { set; get; }
    }
}
