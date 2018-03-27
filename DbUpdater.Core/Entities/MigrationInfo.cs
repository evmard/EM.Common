using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crm.DbUpdater.Core.Entities
{
    public class MigrationInfo
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int Major { get; set; }
        [Required]
        public int Minor { get; set; }
        [Required]
        public int Build { get; set; }
        [Required]
        public string PatchHash { get; set; }

        [NotMapped]
        public Version Version
        {
            get { return new Version(Major, Minor, Build); }
            set { Major = value.Major; Minor = value.Minor; Build = value.Build; }
        }

        public virtual ICollection<AppliedPatch> Files { get; set; }
    }
}
