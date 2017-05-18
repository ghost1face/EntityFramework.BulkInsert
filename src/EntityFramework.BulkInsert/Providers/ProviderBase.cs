using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using EntityFramework.BulkInsert.Extensions;
#if NET45
using System.Threading.Tasks;
#endif

namespace EntityFramework.BulkInsert.Providers
{
    public abstract class ProviderBase<TConnection, TTransaction> : IEfBulkInsertProvider
        where TConnection : IDbConnection
        where TTransaction : IDbTransaction
    {
        /// <summary>
        /// Current DbContext
        /// </summary>
        public DbContext Context { get; private set; }

        public BulkInsertOptions Options { get; set; }

        /// <summary>
        /// Connection string which current dbcontext is using
        /// </summary>
        protected virtual string ConnectionString
        {
            get
            {
                return (string)DbConnection.GetPrivateFieldValue("_connectionString");
            }
        }

        protected virtual IDbConnection DbConnection
        {
            get { return Context.Database.Connection; }
        }

#if NET45

        /// <summary>
        /// Get sql grography object from well known text
        /// </summary>
        /// <param name="wkt">Well known text representation of the value</param>
        /// <param name="srid">The identifier associated with the coordinate system.</param>
        /// <returns></returns>
        public abstract object GetSqlGeography(string wkt, int srid);

        /// <summary>
        /// Get sql geometry object from well known text
        /// </summary>
        /// <param name="wkt">Well known text representation of the value</param>
        /// <param name="srid">The identifier associated with the coordinate system.</param>
        /// <returns></returns>
        public abstract object GetSqlGeometry(string wkt, int srid);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="transaction"></param>
        public Task RunAsync<T>(IEnumerable<T> entities, IDbTransaction transaction)
        {
            return RunAsync(entities, (TTransaction)transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        public virtual async Task RunAsync<T>(IEnumerable<T> entities)
        {
            using (var dbConnection = GetConnection())
            {
                dbConnection.Open();

                using (var transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        await RunAsync(entities, transaction);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        if (transaction.Connection != null)
                        {
                            transaction.Rollback();
                        }
                        throw;
                    }
                }
            }
        }

#endif

        /// <summary>
        /// Sets DbContext for bulk insert to use
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEfBulkInsertProvider SetContext(DbContext context)
        {
            Context = context;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection()
        {
            if (Options.Connection != null)
                return Options.Connection;
            else
                return CreateConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract TConnection CreateConnection();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="transaction"></param>
        public void Run<T>(IEnumerable<T> entities, IDbTransaction transaction)
        {
            Run(entities, (TTransaction)transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        public virtual void Run<T>(IEnumerable<T> entities)
        {
            IDbConnection dbConnection = null;

            if (Options.Connection == null)
            {
                dbConnection = GetConnection();
            }
            else
                dbConnection = Options.Connection;

            try
            {

                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                if (Options.Connection != null && Options.Transaction != null)
                {
                    Run(entities, Options.Transaction);
                }
                else
                {
                    using (var transaction = dbConnection.BeginTransaction())
                    {
                        try
                        {
                            Run(entities, transaction);
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            if (transaction.Connection != null)
                            {
                                transaction.Rollback();
                            }
                            throw;
                        }
                    }
                }
            }
            finally
            {
                // See if we made the connection and dispose if so
                if(Options.Connection == null)
                    dbConnection.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="transaction"></param>
        public abstract void Run<T>(IEnumerable<T> entities, TTransaction transaction);

#if NET45
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities">The entities.</param>
        /// <param name="transaction">The transaction.</param>
        public abstract Task RunAsync<T>(IEnumerable<T> entities, TTransaction transaction);
#endif

    }
}