using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class EFMigrationsHistory
    {
        [Key]
        public string MigrationId { get; set; }
        public string ProductVersion { get; set; }
    }
}
