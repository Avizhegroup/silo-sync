using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silo.Ui.Gate.DML
{
     
        public class TruckCrossSearch
        {
            public string NationalCode { get; set; }
            public string DriverName { get; set; }
            public string DriverPhone { get; set; }
        }


    public class TruckCross
    {
        public string plaque { get; set; }
        public string DriverName { get; set; }
        public string Id { get; set; }
    }

    public class GetTruckCrossQuery
    {
        public long Id { get; set; }
        public string Plaque { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public string NationalCode { get; set; }
        public string Serial { get; set; }
        public string Type { get; set; }
        public string TypeDesc { get; set; }
        public string Company { get; set; }
        public string LicenseCode { get; set; }
        #region Present
        public string PresentCause { get; set; }
        public int PresentTurn { get; set; }
        public DateTime? PresentDateTime { get; set; }
        public string PresentDesc { get; set; }
        public string PresentUserId { get; set; }
        public string PresentUsername { get; set; }
        public bool PresentIsSaved { get; set; } = false;
        #endregion

        #region Enter
        public DateTime? EnterDateTime { get; set; }
        public string EnterDesc { get; set; }
        public string EnterUserId { get; set; }
        public string EnterUsername { get; set; }
        public string EnterEpc { get; set; }
        public string EnterOtherEpcs { get; set; }
        public decimal EnterWeightTonage { get; set; }
        public bool EnterIsSaved { get; set; } = false;
        #endregion

        #region Exit
        public DateTime? ExitDateTime { get; set; }
        public string ExitDesc { get; set; }
        public string ExitUserId { get; set; }
        public string ExitUsername { get; set; }
        public decimal ExitWeightTonage { get; set; }
        public int ExitGateId { get; set; }
        public bool ExitIsSaved { get; set; } = false;
        public string ExitDestination { get; set; }
        #endregion
    }

}
