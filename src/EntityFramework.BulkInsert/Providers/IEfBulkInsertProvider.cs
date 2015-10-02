using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using EntityFramework.BulkInsert.Extensions;

namespace EntityFramework.BulkInsert.Providers
{
    public interface IEfBulkInsertProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        void Run<T>(IEnumerable<T> entities);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="transaction"></param>
        void Run<T>(IEnumerable<T> entities, IDbTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEfBulkInsertProvider SetContext(DbContext context);

#if NET45

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task RunAsync<T>(IEnumerable<T> entities);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task RunAsync<T>(IEnumerable<T> entities, IDbTransaction transaction);

        /// <summary>
        /// Get sql grography object from well known text
        /// </summary>
        /// <param name="wkt">Well known text representation of the value</param>
        /// <param name="srid">The identifier associated with the coordinate system.</param>
        /// <returns></returns>
        object GetSqlGeography(string wkt, int srid);

        /// <summary>
        /// Get sql geometry object from well known text
        /// </summary>
        /// <param name="wkt">Well known text representation of the value</param>
        /// <param name="srid">The identifier associated with the coordinate system.</param>
        /// <returns></returns>
        object GetSqlGeometry(string wkt, int srid);
      
#endif

        /// <summary>
        /// Current DbContext
        /// </summary>
        DbContext Context { get; }

        /// <summary>
        /// 
        /// </summary>
        BulkInsertOptions Options { get; set; }
    }
}