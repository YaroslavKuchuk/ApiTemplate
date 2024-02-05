using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Data;

namespace Core.Entities
{
	public abstract class BaseEntity : IBaseEntity
	{
	    [Key]
	    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
	}
}
