using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using App.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Linq;
using System.Data.Common;
using System.Data;
using System.Reflection;

namespace App.Core.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IDatabaseTransaction BeginTransaction();
        IBaseRepository<TEntity> GetDataRepository<TEntity>() where TEntity : class;
        Task<int> CommitAsync();
        void SaveChanges();
        DbContext Context { get; }
        Task<List<T>> ExecuteStoredProc<T>(string storedProcName, Dictionary<string, object> procParams) where T : class;
    }

    public class UnitOfWork : IUnitOfWork
    {
        public DbContext _context { get; set; }

        private readonly Dictionary<Type, object> _repositories;

        public UnitOfWork(DbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        //public IMenuRepository Menus => _menuRepository ??= new MenuRepository(_context);
        public DbContext Context { get; private set; }
        public IBaseRepository<TEntity> GetDataRepository<TEntity>() where TEntity : class
        {
            var key = typeof(TEntity);

            if (_repositories.TryGetValue(key, out object repository))
            {
                return repository as IBaseRepository<TEntity>;
            }

            repository = new BaseRepository<TEntity>(_context);
            _repositories.Add(typeof(TEntity), repository);
            return repository as IBaseRepository<TEntity>;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Execute procedure from database using it's name and params that is protected from the SQL injection attacks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcName">Name of the procedure that should be executed.</param>
        /// <param name="procParams">Dictionary of params that procedure takes. </param>
        /// <returns>List of objects that are mapped in T type, returned by procedure.</returns>
        public async Task<List<T>> ExecuteStoredProc<T>(string storedProcName, Dictionary<string, object> procParams) where T : class
        {
            DbConnection conn = _context.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await using (DbCommand command = conn.CreateCommand())
                {
                    command.CommandText = storedProcName;
                    command.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, object> procParam in procParams)
                    {
                        DbParameter param = command.CreateParameter();
                        param.ParameterName = procParam.Key;
                        param.Value = procParam.Value;
                        command.Parameters.Add(param);
                    }

                    DbDataReader reader = await command.ExecuteReaderAsync();
                    List<T> objList = new List<T>();
                    IEnumerable<PropertyInfo> props = typeof(T).GetRuntimeProperties();
                    Dictionary<string, DbColumn> colMapping = reader.GetColumnSchema()
                        .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                        .ToDictionary(key => key.ColumnName.ToLower());

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            T obj = Activator.CreateInstance<T>();
                            foreach (PropertyInfo prop in props)
                            {
                                if (colMapping.ContainsKey(prop.Name.ToLower()))
                                {
                                    object val = reader.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
                                    prop.SetValue(obj, val == DBNull.Value ? null : val);
                                }
                            }
                            objList.Add(obj);
                        }
                    }
                    reader.Dispose();

                    return objList;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message, e.InnerException);
            }
            finally
            {
                conn.Close();
            }

            return null; // default state
        }



        public void Dispose()
        {
            _context.Dispose();
        }

        public IDatabaseTransaction BeginTransaction()
        {
            return new DatabaseTransaction(_context.Database.BeginTransaction());
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}