using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using System.Data;

namespace BIL
{
    public class TestBIL
    {
        public TestBIL() { }

        public DataTable GetDataFromDatabase() {
            Test a = new Test();
            return a.GetSomeData();
        }
    }
}
