using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public partial class Materials
    {
        public int MaterialId { get; set; }

        public string MaterialName { get; set; }

        public int CategoryId { get; set; }

        public int LeadTime { get; set; }

        public string TimeUnit { get; set; }

        public decimal LowestExportPrice { get; set; }

        public decimal HighestExportPrice { get; set; }

        public decimal LowestImortPrice { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        #region
        /*其他数据参考格式：
         public enum Title
        {
            King,
            Sir
        };

         private string hisName;
        private bool good;
        private Title hisTitle;

        public Knight(Title title, string name, bool good)
        {
            hisTitle = title;
            hisName = name;
            this.good = good;
        }

        public Knight()
        {
            hisTitle = Title.Sir;
            hisName = "<enter name>";
            good = true;
        }

        public string Name
        {
            get
            {
                return hisName;
            }

            set
            {
                hisName = value;
            }
        }

        public bool GoodGuy
        {
            get
            {
                return good;
            }
            set
            {
                good = value;
            }
        }

        public Title Title
        {
            get
            {
                return hisTitle;
            }
            set
            {
                hisTitle = value;
            }
        }
         
         */
        #endregion
    }
}
