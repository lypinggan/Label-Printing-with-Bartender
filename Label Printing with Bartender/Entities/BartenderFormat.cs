using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Label_Printing_with_Bartender.Entities
{
    public class BartenderFormat
    {
        /// <summary>
        /// Database identity.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of the Bartender Format.
        /// </summary>
        public string FormatName { get; set; }

        /// <summary>
        /// Where the BArtender Format is located.
        /// </summary>
        public string FormatPath { get; set; }

        /// <summary>
        /// Collection of SubString that is held in the Bartender Document.
        /// </summary>
        public ICollection<BartenderSubString> SubStrings { get; set; }

        /// <summary>
        /// Collection of Databases that is held in the Bartender Document.
        /// </summary>
        public ICollection<BartenderDatabase> BartenderDatabases { get; set; }

    }

    public class BartenderSubString
    {
        /// <summary>
        /// Database identity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Bartender SubString Name.
        /// </summary>
        public string SubStringName { get; set; }

        /// <summary>
        /// Bartender SubString Value.
        /// </summary>
        public string SubStringValue { get; set; }

        /// <summary>
        /// Bartender SubString SerialBy.
        /// </summary>
        public string SubStringSerializeBy { get; set; }

        /// <summary>
        /// Bartender SubString SerializeEvery.
        /// </summary>
        public int SubStringSerializeEvery { get; set; }

        /// <summary>
        /// Bartender SubString Rollover.
        /// </summary>
        public bool SubStringRollover { get; set; }

        /// <summary>
        /// Bartender SubString RolloverLimit.
        /// </summary>
        public string SubStringRolloverLimit { get; set; }

        /// <summary>
        /// Bartender SubString RolloverResetValue.
        /// </summary>
        public string SubStringRolloverResetValue { get; set; }

        /// <summary>
        /// Bartender SubString SubStringType.
        /// </summary>
        public Seagull.BarTender.Print.SubstringType SubStringType { get; set; }

        /// <summary>
        /// Foreign Key for BartenderFormat
        /// </summary>
        public int BartenderFormatId { get; set; }
    }
}
