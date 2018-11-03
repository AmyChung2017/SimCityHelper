using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALHelper;
using System.Data;

namespace DAL
{
    //测试调用数据库方法
    public class Test
    {
        //数据库连接字符串（数据库地址、数据库名、数据库用户名、数据库密码）
        private static string ConnString = "Host=localhost;Database=simcity;Port=3306;UserId=root;Password=12345678";

        public Test() { }

        public DataTable GetSomeData() {
            string commandText = "SELECT * FROM simcity.materials";
            DataSet dt = SqlHelper.ExecuteDataset(ConnString, CommandType.Text, commandText);
            DataTable customerTable = dt.Tables[0];
            //DataTable customerTable = dt.Tables["materials"];

            //???
            customerTable.Locale = System.Globalization.CultureInfo.InvariantCulture;

            return customerTable;
        }


    }
}
