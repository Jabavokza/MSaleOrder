using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSaleOrder.WebApiRes
{
    class mSaleOrderRes
    {
        [JsonProperty("SaleOrderRes")]
        public cSaleOrderRes SaleOrderRes { get; set; }
    }

    public class cSaleOrderRes
    {
        [JsonProperty("Header")]
        public List<Header> Header { get; set; }
    }
    public class Result
    {
        [JsonProperty("RET_CODE")]
        public string RET_CODE { get; set; }

        [JsonProperty("MSG_CODE")]
        public string MSG_CODE { get; set; }

        [JsonProperty("MSG_TEXT")]
        public string MSG_TEXT { get; set; }
    }

    public class Header
    {
        [JsonProperty("BELNR")]
        public string BELNR { get; set; }

        [JsonProperty("TRANSTYPE")]
        public string TRANSTYPE { get; set; }

        [JsonProperty("DLVRY_DATE")]
        public string DLVRY_DATE { get; set; }

        [JsonProperty("ACTION")]
        public string ACTION { get; set; }

        [JsonProperty("Result")]
        public Result Result { get; set; }
    }

}
