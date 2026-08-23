using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silo.Ui.Gate.Models.DML
{
    public class GetAllAggDocDto
    {
        public string DocumentKey { get; set; }
        public string DocumentType { get; set; }
        public DateTime? ImportDateTime { get; set; }
        public int ItemCount { get; set; }
        public decimal ItemSum { get; set; }
        public string DocumentData { get; set; }
    }
}
