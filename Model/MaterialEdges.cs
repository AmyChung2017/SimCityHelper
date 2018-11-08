using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public partial class MaterialEdges
    {
        public int EdgeId { get; set; }

        public int FromMaterial { get; set; }

        public int ToMaterial { get; set; }

        public int Volume { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
