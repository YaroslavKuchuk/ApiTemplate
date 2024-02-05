namespace Core.Data
{
    public interface IIsDeleteBaseEntity : IBaseEntity
    {
        bool IsDelete { get; set; }
    }
}
