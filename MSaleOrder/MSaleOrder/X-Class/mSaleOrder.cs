using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSaleOrder.X_Class
{
    public class TENDER
    {

        [JsonProperty("TENDER_TYPE")]
        public string TENDER_TYPE { get; set; }

        [JsonProperty("AMOUNT")]
        public string AMOUNT { get; set; }

        [JsonProperty("BONUSBUYID")]
        public string BONUSBUYID { get; set; }
    }

    public class TDBODY
    {

        [JsonProperty("TDLINE")]
        public string TDLINE { get; set; }
    }

    public class TEXT
    {

        [JsonProperty("TDID")]
        public string TDID { get; set; }

        [JsonProperty("TDBODY")]
        public IList<TDBODY> TDBODY { get; set; }
    }

    public class PRICING
    {

        [JsonProperty("TYPE")]
        public string TYPE { get; set; }

        [JsonProperty("AMOUNT")]
        public string AMOUNT { get; set; }

        [JsonProperty("CURRENCY")]
        public string CURRENCY { get; set; }

        [JsonProperty("SIGN")]
        public string SIGN { get; set; }

        [JsonProperty("BONUSBUYID")]
        public string BONUSBUYID { get; set; }
    }

    public class Item
    {

        [JsonProperty("POSEX")]
        public string POSEX { get; set; }

        [JsonProperty("QUANTITY")]
        public string QUANTITY { get; set; }

        [JsonProperty("UOM")]
        public string UOM { get; set; }

        [JsonProperty("SERIALNUMBER")]
        public string SERIAL { get; set; }

        [JsonProperty("SHIPPING_POINT")]
        public string SHIPPING_POINT { get; set; }

        [JsonProperty("SELLING_PLANT")]
        public string SELLING_PLANT { get; set; }

        [JsonProperty("PRICING")]
        public IList<PRICING> PRICING { get; set; }

        [JsonProperty("EANCODE")]
        public string EANCODE { get; set; }

        
    }

    public class Header
    {

        [JsonProperty("ACTION")]
        public string ACTION { get; set; }

        [JsonProperty("TRANSTYPE")]
        public string TRANSTYPE { get; set; }

        [JsonProperty("DLVRY_DATE")]
        public string DLVRY_DATE { get; set; }

        [JsonProperty("TENDER")]
        public IList<TENDER> TENDER { get; set; }

        [JsonProperty("NAME1")]
        public string NAME1 { get; set; }

        [JsonProperty("NAME2")]
        public string NAME2 { get; set; }

        [JsonProperty("NAME3")]
        public string NAME3 { get; set; }

        [JsonProperty("NAME4")]
        public string NAME4 { get; set; }

        [JsonProperty("STREET")]
        public string STREET { get; set; }

        [JsonProperty("CITY")]
        public string CITY { get; set; }

        [JsonProperty("POSTAL_CODE")]
        public string POSTAL_CODE { get; set; }

        [JsonProperty("LAND")]
        public string LAND { get; set; }

        [JsonProperty("TEL")]
        public string TEL { get; set; }

        [JsonProperty("FAX")]
        public string FAX { get; set; }

        [JsonProperty("DISTRICT")]
        public string DISTRICT { get; set; }

        [JsonProperty("BELNR")]
        public string BELNR { get; set; }

        [JsonProperty("REF_BELNR")]
        public string REF_BELNR { get; set; }

        [JsonProperty("TEXT")]
        public IList<TEXT> TEXT { get; set; }

        [JsonProperty("Item")]
        public IList<Item> Item { get; set; }
    }

    public class SalesOrder
    {

        [JsonProperty("Header")]
        public Header Header { get; set; }
    }

    public class mSaleOrder
    {

        [JsonProperty("SalesOrder")]
        public SalesOrder SalesOrder { get; set; }
    }    
}
