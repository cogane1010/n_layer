  using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace App.Core.Helper
{
    public static class DapperHelper
    {
        public static Tuple<IEnumerable<T>, DynamicParameters>  ExecuteProcedure<T>(this SqlConnection connection,
                 string storedProcedure, DynamicParameters parameters = null,
                 int commandTimeout = 180)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Close();
                    connection.Open();
                }
                IEnumerable<T> data;
                if (parameters != null)
                {
                    
                    data = connection.Query<T>(storedProcedure, parameters,
                        commandType: CommandType.StoredProcedure, commandTimeout: commandTimeout);
                }
                else
                {
                    data =  connection.Query<T>(storedProcedure,
                        commandType: CommandType.StoredProcedure, commandTimeout: commandTimeout);
                }

                return Tuple.Create(data, parameters);
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }
            finally
            {
                connection.Close();
            }

        }


      
    }

}
