using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silo.Ui.Gate.Models
{
   public class Readers:Object
    {
        public Readers()
        {

        }


        public int ReaderId { get; set; }

        public string ReaderConnectionIp { get; set; }

        public int  ReaderPower { get; set; }

        public int ReaderStationCode { get; set; }


        public string ReaderTitle { get; set; }


    }

}
