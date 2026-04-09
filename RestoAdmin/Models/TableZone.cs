using System.Collections.Generic;

namespace RestoAdmin.Models
{
    public class TableZone
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
    }
}