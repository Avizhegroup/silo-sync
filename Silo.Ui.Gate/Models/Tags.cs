using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silo.Ui.Gate.Models
{
   public  class Tags:object
    {
        public Tags()
        {

        }

        public string TagEPC { get; set; }

        public DateTime TagReedTime { get; set; }

        public int TagReedGateNumber { get; set; }

        public int TagReedSaveStatus { get; set; }
        public int TagReedUpdateStatus { get; set; }

        public int TagPackageId { get; set; }
        public int TagPackageStatus { get; set; }
        public int TagActionStatus { get; set; }
        public int TagBeforSendStatus { get; set; }

        public string DocumentId { get; set; }

        public string WMUsertId { get; set; }

    }


    public class GateResult
    {
        public string Row { get; set; }
        public string ProductCode { get; set; }
        public string ProductTechnicalCode { get; set; }
        public string ProductName { get; set; }
        public string Count { get; set; }
        public string SumValue { get; set; }
        public string ProductSerial { get; set; }
        public string TagSerial { get; set; }
        public string ProductType { get; set; }
        public string ProductStatus { get; set; }
        public string TagStatus { get; set; }
        public string TagInDestinationId { get; set; }
        public string ProduLockctSerial { get; set; }
        public string ProductLine { get; set; }
        public string ProductShift { get; set; }
        public string Lock { get; set; }

        public int TagPackageId { get; set; }
        public int TagPackageStatus { get; set; }
        public string DocumentId { get; set; }

        public string WMUsertId { get; set; }


        public string Freeze { get; set; }
        public string ProductOldSerial { get; set; }


        public string PMToStoreCode { get; set; }
        public string PMToStoreTitle { get; set; }
        public string PMToZoneCode { get; set; }


        public string TagGateResultStatus { get; set; }
        public DateTime TagGateReadTime { get; set; }
        public string LastInspectResult { get; set; }
        public DateTime TagRegisterDateTime { get; set; }

        public string DocumentCheckStatusDesc { get; set; }


    }
}
