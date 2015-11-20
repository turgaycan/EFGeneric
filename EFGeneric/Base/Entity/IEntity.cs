using System;

namespace EFGeneric.Base.Entity
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IEntity<TId> where TId : IComparable
    {
        TId Id { get; }
    }
}