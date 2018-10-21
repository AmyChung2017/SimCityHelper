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

        //数据库连接对象
        private static MySqlConnection conn = new MySqlConnection(ConnString);

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

        #region 非查询方法ExecuteNonQuery

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
        /*==================BEGIN Sql命令有参数=========================*/

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

    }
}
