using System;
using System.ComponentModel.DataAnnotations;

namespace Crm.DbUpdater.Core.Entities
{
    public class AppliedPatch
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public PatchFileType Type { get; set; }
        [Required]
        public string Hash { get; set; }
        [Required]
        public DateTime Installed { get; set; }
    }
}
