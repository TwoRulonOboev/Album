using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAlbom.Models
{
    public class Album
    {
        public string? _id { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Title { get; set; }
        public virtual List<Photo> Photos { get; set; } = null!;
    }

}
