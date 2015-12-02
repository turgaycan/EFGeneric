namespace EFGeneric.Base.Entity.Core
{

    /// <summary>
    /// generic interface..
    /// </summary>
    public interface IEntity<PK>
    {
        PK Id { get; set; }
    }
}