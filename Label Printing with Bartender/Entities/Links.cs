using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Label_Printing_with_Bartender.Entities
{
    public class Links
    {
        public int Id { get; set; }
        public BartenderFormat BartenderFormat { get; set; }
        public BartenderSubString BartenderSubString { get; set; }
        public BartenderDatabase BartenderDatabase { get; set; }
        public DatabaseTable DatabaseTable { get; set; }
        public DatabaseColumn DatabaseColumn { get; set; }
    }
}
