using System;
using System.Linq.Expressions;

namespace EntityFramework.MappingAPI
{
    /// <summary>
    /// Generic entity map
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IEntityMap<T> : IEntityMap
    {
        /// <summary>
        /// Get property mapping by predicate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IPropertyMap Prop<T1>(Expression<Func<T, T1>> predicate);
    }
}