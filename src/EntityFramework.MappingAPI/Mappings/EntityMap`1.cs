using System;
using System.Linq.Expressions;

namespace EntityFramework.MappingAPI.Mappings
{
    internal class EntityMap<T> : EntityMap, IEntityMap<T>
    {
        public IPropertyMap Prop<T1>(Expression<Func<T, T1>> predicate)
        {
            var predicateString = predicate.ToString();
            var i = predicateString.IndexOf('.');
            var propName = predicateString.Substring(i + 1);
            return base[propName];
        }
    }
}
