// ===============================================================================
// 该类是基于Microsoft Data Access Application Block for .NET所改写
// http://msdn.microsoft.com/library/en-us/dnbda/html/daab-rm.asp
// ===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//导入MySQL
using MySql.Data.MySqlClient;
using System.Data;

namespace DALHelper
{
    /// <summary>
	/// SqlHelper用于连接数据库
	/// </summary>
    public sealed class SqlHelper
    {
        //数据库连接字符串（数据库地址、数据库名、数据库用户名、数据库密码）
        private static string ConnString = "Host=localhost;Database=school;Port=3306;UserId=root;Password=12345678";


        #region 私有方法、构造器

        // SqlHelper只提供静态方法，防止生成实例
        private SqlHelper() { }

        /// <summary>
        /// 将MySqlParameter参数数组放到MySqlCommand中
        /// 参数：MySqlCommand、MySqlParameter命令参数数组
        /// 返回：无
        /// 
        /// 若参数为Null或者空值，会将DbNull传入MySqlCommand中
        /// 
        /// </summary>
        /// <param name="command">MySqlCommand</param>
        /// <param name="commandParameters">MySqlParameter命令参数数组</param>
        private static void AttachParameters(MySqlCommand command, MySqlParameter[] commandParameters)
        {
            // 检查MySql命令是否为空
            if (command == null) throw new ArgumentNullException("command");
            // 如果有参数
            if (commandParameters != null)
            {
                foreach (MySqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // 检查未被赋值的输入输出数据和参数本身
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// 打开MySQL数据库连接，执行MySQL命令
        /// 参数：MySqlCommand、MySqlConnection、MySqlTransaction、命令类型、Sql命令、MySqlParameter命令参数数组、是否关闭数据库连接
        /// 返回：无
        /// </summary>
        /// <param name="command">MySqlCommand</param>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="transaction">MySqlTransaction</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParamter数组</param>
        /// <param name="mustCloseConnection"><c>true</c></param>
        private static void PrepareCommand(MySqlCommand command, MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, MySqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            // 检查MySql命令
            if (command == null) throw new ArgumentNullException("command");
            // 检查Sql语句
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            // 检查数据库连接状态，如果连接未打开就将连接打开
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // 将MySqlCommand与MySqlConnection结合在一起
            command.Connection = connection;

            // 在MySqlCommand设置Sql命令（存储过程名称或者Sql语句）
            command.CommandText = commandText;

            // 如果有事务，就将事务添加到MySqlCommand中
            if (transaction != null)
            {
                // 检查事务的状态
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // 在MySqlCommand设置命令类型
            command.CommandType = commandType;

            // 如果有命令参数，将命令参数添加到MySqlCommand中
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }

        #endregion

        #region ExecuteNonQuery：插入、更新、删除 

        /*==================BEGIN Sql命令无参数=========================*/
        /// <summary>
        /// 参数：数据库连接符、命令类型、Sql命令
        /// 返回：受影响行数
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串（包含数据库名称、用户名、密码等）</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <returns>返回受sql语句所影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// 参数：MySqlConnection、命令类型、Sql命令
        /// 返回：受影响行数
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <returns>返回受sql语句所影响的行数</returns>
        public static int ExecuteNonQuery(MySqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// 参数：MySqlTransaction、命令类型、Sql命令
        /// 返回：受影响行数
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="transaction">MySqlTransaction</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <returns>返回受sql语句所影响的行数</returns>
        public static int ExecuteNonQuery(MySqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, (MySqlParameter[])null);
        }
        /*==================END Sql命令无参数=========================*/

        /*==================BEGIN Sql命令有参数=========================*/
        /// <summary>
        /// 参数：数据库连接符、命令类型、Sql命令、MySqlParameter命令参数数组
        /// 返回：受影响行数
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串（包含数据库名称、用户名、密码等）</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParamter数组</param>
        /// <returns>返回受sql语句所影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            // 检查数据库连接字符串是否有效
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

            // 打开连接，结束之后关闭连接
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }
        
        /// <summary>
        /// 参数：MySqlConnection、命令类型、Sql命令、MySqlParameter命令参数数组
        /// 返回：受影响行数
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParamter数组</param>
        /// <returns>返回受sql语句所影响的行数</returns>
        public static int ExecuteNonQuery(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            // 检查数据库连接字符串是否有效
            if (connection == null) throw new ArgumentNullException("connection");

            // 创建MySql命令
            MySqlCommand cmd = new MySqlCommand();
            //是否一定要关闭连接
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (MySqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行MySql命令
            int retval = cmd.ExecuteNonQuery();

            // 解绑MySqlParameters
            cmd.Parameters.Clear();

            // 如果需要，关闭连接
            if (mustCloseConnection)
                connection.Close();

            return retval;
        }

        /// <summary>
        /// 参数：MySqlTransaction、命令类型、Sql命令、MySqlParameter命令参数数组
        /// 返回：受影响行数
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">MySqlTransaction</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParamter数组</param>
        /// <returns>返回受sql语句所影响的行数</returns>
        public static int ExecuteNonQuery(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            // 检查事务是否有效
            if (transaction == null) throw new ArgumentNullException("transaction");
            // 检查事务状态
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // 创建MySql命令
            MySqlCommand cmd = new MySqlCommand();
            //是否一定要关闭连接
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行MySql命令
            int retval = cmd.ExecuteNonQuery();

            // 解绑MySqlParameters
            cmd.Parameters.Clear();

            return retval;
        }
        /*==================END Sql命令有参数=========================*/

        /*==================BEGIN 存储过程=========================*/
        /// <summary>
        /// 功能：执行存储过程
        /// 参数：数据库连接符、存储过程名称、params object存储过程参数数组
        /// 返回：受影响行数
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串（包含数据库名称、用户名、密码等）</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">params object数组，严格按照存储过程的参数顺序</param>
        /// <returns>返回受sql语句所影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            //暂略
            return 0;
        }

        /// <summary>
        /// 功能：执行存储过程
        /// 参数：MySqlConnection、存储过程名称、params object存储过程参数数组
        /// 返回：受影响行数
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">params object数组，严格按照存储过程的参数顺序</param>
        /// <returns>返回受sql语句所影响的行数</returns>
        public static int ExecuteNonQuery(MySqlConnection connection, string spName, params object[] parameterValues)
        {
            // 暂略
            return 0;
        }

        /// <summary>
        /// 功能：执行存储过程
        /// 参数：MySqlTransaction、存储过程名称、params object存储过程参数数组
        /// 返回：受影响行数
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">MySqlTransaction</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">params object数组，严格按照存储过程的参数顺序</param>
        /// <returns>返回受sql语句所影响的行数</returns>
        public static int ExecuteNonQuery(MySqlTransaction transaction, string spName, params object[] parameterValues)
        {
            // 暂略
            return 0;
        }
        /*==================END 存储过程=========================*/

        #endregion

        #region ExecuteDataset：查询，返回Dataset格式 

        /*==================BEGIN Sql命令无参数=========================*/
        /// <summary>
        /// 参数：数据库连接符、命令类型、Sql命令
        /// 返回：Dataset
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串（包含数据库名称、用户名、密码等）</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <returns>返回一个Dataset</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionString, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// 参数：MySqlConnection、命令类型、Sql命令
        /// 返回：Dataset    
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <returns>返回一个Dataset</returns>
        public static DataSet ExecuteDataset(MySqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connection, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// 参数：MySqlTransaction、命令类型、Sql命令
        /// 返回：Dataset
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">MySqlTransaction</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <returns>返回一个Dataset</returns>
        public static DataSet ExecuteDataset(MySqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataset(transaction, commandType, commandText, (MySqlParameter[])null);
        }
        /*==================END Sql命令无参数=========================*/

        /*==================BEGIN Sql命令有参数=========================*/
        /// <summary>
        /// 参数：数据库连接符、命令类型、Sql命令、MySqlParameter命令参数数组
        /// 返回：Dataset
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串（包含数据库名称、用户名、密码等）</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParamter数组</param>
        /// <returns>返回一个Dataset</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            // 检查数据库连接字符串是否有效
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

            // 打开连接，结束之后关闭连接
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// 参数：MySqlConnection、命令类型、Sql命令、MySqlParameter命令参数数组
        /// 返回：Dataset
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParamter数组</param>
        /// <returns>返回一个Dataset</returns>
        public static DataSet ExecuteDataset(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            // 检查数据库连接字符串是否有效
            if (connection == null) throw new ArgumentNullException("connection");

            // 创建MySql命令
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (MySqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // 创建DataAdapter & DataSet
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();

                // 填充Dataset
                da.Fill(ds);

                // 解绑MySqlParameters
                cmd.Parameters.Clear();

                if (mustCloseConnection)
                    connection.Close();

                // 返回Dataset
                return ds;
            }
        }

        /// <summary>
        /// 参数：MySqlTransaction、命令类型、Sql命令、MySqlParameter命令参数数组
        /// 返回：受影响行数
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">MySqlTransaction</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParamter数组</param>
        /// <returns>返回一个Dataset</returns>
		public static DataSet ExecuteDataset(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            // 检查事务是否有效
            if (transaction == null) throw new ArgumentNullException("transaction");
            // 检查事务状态
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // 创建MySql命令
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 创建DataAdapter & DataSet
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();

                // 填充Dataset
                da.Fill(ds);

                // 解绑MySqlParameters
                cmd.Parameters.Clear();

                // 返回Dataset
                return ds;
            }
        }
        /*==================END Sql命令有参数=========================*/

        /*==================BEGIN 存储过程=========================*/
        /// <summary>
        /// 功能：执行存储过程
        /// 参数：数据库连接符、存储过程名称、params object存储过程参数数组
        /// 返回：Dataset
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串（包含数据库名称、用户名、密码等）n</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">params object数组，严格按照存储过程的参数顺序</param>
        /// <returns>返回一个Dataset</returns>
        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            // 暂略
            return null;
        }

        /// <summary>
        /// 功能：执行存储过程
        /// 参数：MySqlConnection、存储过程名称、params object存储过程参数数组
        /// 返回：Dataset
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">params object数组，严格按照存储过程的参数顺序</param>
        /// <returns>返回一个Dataset</returns>
        public static DataSet ExecuteDataset(MySqlConnection connection, string spName, params object[] parameterValues)
        {
            // 暂略
            return null;
        }

        /// <summary>
        /// 功能：执行存储过程
        /// 参数：MySqlTransaction、存储过程名称、params object存储过程参数数组
        /// 返回：Dataset
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">MySqlTransaction</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">params object数组，严格按照存储过程的参数顺序</param>
        /// <returns>返回一个Dataset</returns>
        public static DataSet ExecuteDataset(MySqlTransaction transaction, string spName, params object[] parameterValues)
        {
            // 暂略
            return null;
        }
        /*==================END 存储过程=========================*/

        #endregion

        #region ExecuteReader：查询，返回MySqlDataReader格式 

        /// <summary>
        /// 用于表示数据库连接是由SqlHelper还是Caller创建的，根据不同的情况调用ExecuteReader方法
        /// </summary>
        private enum SqlConnectionOwnership
        {
            /// <summary>SqlHelper创建数据库连接</summary>
            Internal,
            /// <summary>Caller创建数据库连接</summary>
            External
        }

        /// <summary>
        /// 参数：MySqlConnection、MySqlTransaction、命令类型、Sql命令、MySqlParameters数组、数据库连接创建者
        /// 返回：MySqlDataReader
        /// </summary>
        /// <remarks>
        /// 若数据库连接由自身的SqlHelper打开，则在DataReader关闭后自动断开连接
        /// 
        /// 若数据库连接由外部的Caller打开，则由Caller负责断开连接
        /// </remarks>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="transaction">MySqlTransaction或者'null'</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParameters数组或者'null'</param>
        /// <param name="connectionOwnership">表示数据库连接是由SqlHelper还是Caller创建的</param>
        /// <returns>返回MySqlDataReader</returns>
        private static MySqlDataReader ExecuteReader(MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, MySqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            // 检查数据库连接有效性
            if (connection == null) throw new ArgumentNullException("connection");

            bool mustCloseConnection = false;
            // 创建MySqlCommand
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

                // 创建MySqlDataReader
                MySqlDataReader dataReader;

                // 若数据库连接由外部的Caller打开，则由Caller负责断开连接
                if (connectionOwnership == SqlConnectionOwnership.External)
                {
                    dataReader = cmd.ExecuteReader();
                }
                else  // 若数据库连接由自身的SqlHelper打开，则在DataReader关闭后自动断开连接
                {
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }

                // 解绑MySqlParameters
                bool canClear = true;
                foreach (MySqlParameter commandParameter in cmd.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                        canClear = false;
                }

                if (canClear)
                {
                    cmd.Parameters.Clear();
                }

                return dataReader;
            }
            catch
            {
                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }

        /*==================BEGIN Sql命令无参数=========================*/
        /// <summary>
        /// 参数：数据库连接符、命令类型、Sql命令
        /// 返回：包含结果集的MySqlDataReader
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串（包含数据库名称、用户名、密码等）</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <returns>返回一个包含结果集的MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteReader(connectionString, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// 参数：MySqlConnection、命令类型、Sql命令
        /// 返回：包含结果集的MySqlDataReader
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <returns>返回一个包含结果集的MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(MySqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteReader(connection, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// 参数：MySqlTransaction、命令类型、Sql命令
        /// 返回：包含结果集的MySqlDataReader
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">MySqlTransaction</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <returns>返回一个包含结果集的MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(MySqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteReader(transaction, commandType, commandText, (MySqlParameter[])null);
        }
        /*==================END Sql命令无参数=========================*/

        /*==================BEGIN Sql命令有参数=========================*/
        /// <summary>
        /// 参数：数据库连接符、命令类型、Sql命令、MySqlParameter命令参数数组
        /// 返回：包含结果集的MySqlDataReader
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串（包含数据库名称、用户名、密码等）</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParamter数组</param>
        /// <returns>返回一个包含结果集的MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            /// 检查数据库连接字符串是否有效
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();

                // 在SqlHelper自身调用ExecuteReader方法
                return ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
            }
            catch
            {
                // 如果无法返回MySqlDataReader就关闭数据库连接
                if (connection != null) connection.Close();
                throw;
            }

        }

        /// <summary>
        /// 参数：MySqlConnection、命令类型、Sql命令、MySqlParameter命令参数数组
        /// 返回：包含结果集的MySqlDataReader
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParamter数组</param>
        /// <returns>返回一个包含结果集的MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            return ExecuteReader(connection, (MySqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        /// <summary>
        /// 参数：MySqlTransaction、命令类型、Sql命令、MySqlParameter命令参数数组
        /// 返回：包含结果集的MySqlDataReader
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///   MySqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">MySqlTransaction</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParamter数组</param>
        /// <returns>返回一个包含结果集的MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            // 检查事务是否有效
            if (transaction == null) throw new ArgumentNullException("transaction");
            // 检查事务状态
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        /*==================END Sql命令有参数=========================*/

        /*==================BEGIN 存储过程=========================*/
        /// <summary>
        /// 功能：执行存储过程
        /// 参数：数据库连接符、存储过程名称、params object存储过程参数数组
        /// 返回：包含结果集的MySqlDataReader
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串（包含数据库名称、用户名、密码等）</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">params object数组，严格按照存储过程的参数顺序</param>
        /// <returns>返回一个包含结果集的MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        {
            // 暂略
            return null;
        }

        /// <summary>
        /// 功能：执行存储过程
        /// 参数：MySqlConnection、存储过程名称、params object存储过程参数数组
        /// 返回：包含结果集的MySqlDataReader
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">params object数组，严格按照存储过程的参数顺序</param>
        /// <returns>返回一个包含结果集的MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(MySqlConnection connection, string spName, params object[] parameterValues)
        {
            //暂略
            return null;
        }

        /// <summary>
        /// 功能：执行存储过程
        /// 参数：MySqlTransaction、存储过程名称、params object存储过程参数数组
        /// 返回：包含结果集的MySqlDataReader
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">MySqlTransaction</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">params object数组，严格按照存储过程的参数顺序</param>
        /// <returns>返回一个包含结果集的MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(MySqlTransaction transaction, string spName, params object[] parameterValues)
        {
            //暂略
            return null;
        }
        /*==================END 存储过程=========================*/
        #endregion

        #region ExecuteScalar：查询，返回包含1x1数据集的ojbect 

        /*==================BEGIN Sql命令无参数=========================*/
        /// <summary>
        /// 参数：数据库连接符、命令类型、Sql命令
        /// 返回：一个包含1x1数据集的object
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串（包含数据库名称、用户名、密码等）</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <returns>返回一个包含1x1数据集的对象</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connectionString, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// 参数：MySqlConnection、命令类型、Sql命令
        /// 返回：一个包含1x1数据集的object
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <returns>返回一个包含1x1数据集的对象</returns>
        public static object ExecuteScalar(MySqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connection, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// 参数：MySqlTransaction、命令类型、Sql命令
        /// 返回：一个包含1x1数据集的object
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="transaction">MySqlTransaction</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <returns>返回一个包含1x1数据集的对象</returns>
        public static object ExecuteScalar(MySqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteScalar(transaction, commandType, commandText, (MySqlParameter[])null);
        }
        /*==================END Sql命令无参数=========================*/

        /*==================BEGIN Sql命令有参数=========================*/
        /// <summary>
        /// 参数：数据库连接符、命令类型、Sql命令、MySqlParameter命令参数数组
        /// 返回：一个包含1x1数据集的object
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串（包含数据库名称、用户名、密码等）</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParamter数组</param>
        /// <returns>返回一个包含1x1数据集的对象</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            // 检查数据库连接字符串是否有效
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            
            // 打开连接，结束之后关闭连接
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                return ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// 参数：MySqlConnection、命令类型、Sql命令、MySqlParameter命令参数数组
        /// 返回：一个包含1x1数据集的object
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParamter数组</param>
        /// <returns>返回一个包含1x1数据集的对象</returns>
        public static object ExecuteScalar(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            // 检查数据库连接字符串是否有效
            if (connection == null) throw new ArgumentNullException("connection");

            // 创建MySql命令
            MySqlCommand cmd = new MySqlCommand();

            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (MySqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行查询命令
            object retval = cmd.ExecuteScalar();

            // 解绑MySqlParameters
            cmd.Parameters.Clear();

            if (mustCloseConnection)
                connection.Close();

            return retval;
        }

        /// <summary>
        /// 参数：MySqlTransaction、命令类型、Sql命令、MySqlParameter命令参数数组
        /// 返回：一个包含1x1数据集的object
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">MySqlTransaction</param>
        /// <param name="commandType">命令类型（存储过程、文字等）</param>
        /// <param name="commandText">存储过程的名称或sql语句</param>
        /// <param name="commandParameters">MySqlParamter数组</param>
        /// <returns>返回一个包含1x1数据集的对象</returns>        
        public static object ExecuteScalar(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            // 检查事务是否有效
            if (transaction == null) throw new ArgumentNullException("transaction");
            // 检查事务状态
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // 创建MySql命令
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // 执行查询命令
            object retval = cmd.ExecuteScalar();

            // 解绑MySqlParameters
            cmd.Parameters.Clear();
            return retval;
        }
        /*==================END Sql命令有参数=========================*/

        /*==================BEGIN 存储过程=========================*/
        /// <summary>
        /// 功能：执行存储过程
        /// 参数：数据库连接符、存储过程名称、params object存储过程参数数组
        /// 返回：一个包含1x1数据集的object
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="connectionString">数据库连接字符串（包含数据库名称、用户名、密码等）</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">params object数组，严格按照存储过程的参数顺序</param>
        /// <returns>返回一个包含1x1数据集的对象</returns>
        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            //暂略
            return null;
        }

        /// <summary>
        /// 功能：执行存储过程
        /// 参数：MySqlConnection、存储过程名称、params object存储过程参数数组
        /// 返回：一个包含1x1数据集的object
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="connection">MySqlConnection</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">params object数组，严格按照存储过程的参数顺序</param>
        /// <returns>返回一个包含1x1数据集的对象</returns>
        public static object ExecuteScalar(MySqlConnection connection, string spName, params object[] parameterValues)
        {
            //暂略
            return null;
        }

        /// <summary>
        /// 功能：执行存储过程
        /// 参数：MySqlTransaction、存储过程名称、params object存储过程参数数组
        /// 返回：一个包含1x1数据集的object
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="transaction">MySqlTransaction</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">params object数组，严格按照存储过程的参数顺序</param>
        /// <returns>返回一个包含1x1数据集的对象</returns>
        public static object ExecuteScalar(MySqlTransaction transaction, string spName, params object[] parameterValues)
        {
            //暂略
            return null;
        }
        /*==================END 存储过程=========================*/
        #endregion ExecuteScalar	

        #region ExecuteXmlReader：读取XML文件 
        // 暂略
        #endregion
    }
}
