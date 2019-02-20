using System;
using System.Data.Entity;
using System.Linq.Expressions;

namespace EntityFramework.MappingAPI.Extensions
{
    public static class MappingApiExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static IEntityMap[] Db(this DbContext ctx)
        {
            return EfMap.Get(ctx).Tables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static IEntityMap<T> Db<T>(this DbContext ctx)
        {
            return EfMap.Get<T>(ctx);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="dbset"></param>
        /// <returns></returns>
        public static IEntityMap<T1> Db<T, T1>(this T ctx, Expression<Func<T, DbSet<T1>>> dbset) where T : DbContext where T1 : class
        {
            return ctx.Db<T1>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEntityMap Db(this DbContext ctx, Type type)
        {
            return EfMap.Get(ctx)[type];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        public static IEntityMap Db(this DbContext ctx, string typeFullName)
        {
            return EfMap.Get(ctx)[typeFullName];
        }
    }
}
