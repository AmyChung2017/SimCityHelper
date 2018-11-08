using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public partial class CityUpgrade
    {
        public int UpgradeId { get; set; }

        public string FacilityName { get; set; }

        public int Volume { get; set; }

        public Boolean IsWarehouse { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
