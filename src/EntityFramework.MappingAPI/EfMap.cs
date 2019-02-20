using EntityFramework.MappingAPI.Mappings;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace EntityFramework.MappingAPI
{
    /// <summary>
    /// 
    /// </summary>
    internal class EfMap
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<string, DbMapping> Mappings = new Dictionary<string, DbMapping>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEntityMap<T> Get<T>(DbContext context)
        {
            return (IEntityMap<T>)Get(context)[typeof(T)];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEntityMap Get(DbContext context, Type type)
        {
            return Get(context)[type];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEntityMap Get(DbContext context, string typeFullName)
        {
            return Get(context)[typeFullName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static DbMapping Get(DbContext context)
        {
            var cacheKey = context.GetType().FullName;

            var iDbModelCacheKeyProvider = context as IDbModelCacheKeyProvider;
            if (iDbModelCacheKeyProvider != null)
            {
                cacheKey = iDbModelCacheKeyProvider.CacheKey;
            }

            DbMapping mapping;
            if (Mappings.TryGetValue(cacheKey, out mapping))
                return mapping;

            mapping = new DbMapping(context);

            Mappings[cacheKey] = mapping;
            return mapping;
        }
    }
}
