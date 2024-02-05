using Core.Data;


namespace Core.Entities
{
    public class IsDeleteBaseEntity : BaseEntity, IIsDeleteBaseEntity
    {
        public bool IsDelete { get; set; }
    }
}
