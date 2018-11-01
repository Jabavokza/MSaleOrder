using MSaleOrder.X_Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MSaleOrder.ST_Class
{
    public class cCNSP
    {
        private static ObjectCache oSP_DBCache = MemoryCache.Default;
        public static List<string> aPlantList = new List<string>();
        private static readonly log4net.ILog oC_Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool SP_LOADbConfig()
        {
            SP_GETtConStrList();
            oSP_DBCache.Set("WebAPICon", SP_LOADtWebAPICon(), DateTimeOffset.UtcNow.AddHours(5));
            oSP_DBCache.Set("ScheduleConfig", SP_LOADxSchdlConfig(), DateTimeOffset.UtcNow.AddHours(5));
            return true;
        }

        private static string SP_READtCacheFilePath()
        {
            string tJsonFilePath = null;

            if (!oSP_DBCache.Contains("JsonFilePath"))
            {
                tJsonFilePath = SP_GETtConfigStr("DBFilePath");
                oSP_DBCache.Set("JsonFilePath", tJsonFilePath, DateTimeOffset.UtcNow.AddHours(5));
            }
            else
            {
                tJsonFilePath = oSP_DBCache.Get("JsonFilePath") as string;
            }
            return tJsonFilePath;
        }

        public static void SP_DELtCacheConStr(string tPlantCode)
        {
            if (oSP_DBCache.Contains("DBCon" + tPlantCode))
            {
                oSP_DBCache.Remove("DBCon" + tPlantCode);
            }
            if (aPlantList.Contains(tPlantCode))
            {
                aPlantList.Remove(tPlantCode);
            }
        }

        public static void SP_NEWtCacheConStr(string tPlantCode)
        {
            if (!oSP_DBCache.Contains("DBCon" + tPlantCode))
            {
                cSQLCon oSQLCon = new cSQLCon();
                oSP_DBCache.Set("DBCon" + tPlantCode, oSQLCon, DateTimeOffset.UtcNow.AddHours(5));
            }
            if (!aPlantList.Contains(tPlantCode))
            {
                aPlantList.Add(tPlantCode);
            }
        }

        public static void SP_CHNGtCacheConStr(string tPlantCode, string tNewCode)
        {
            if (oSP_DBCache.Contains("DBCon" + tPlantCode))
            {
                cSQLCon oSQLCon = new cSQLCon();
                oSQLCon = (oSP_DBCache.Get("DBCon" + tPlantCode) as cSQLCon).C_GEToClone();
                oSP_DBCache.Remove("DBCon" + tPlantCode);
                oSP_DBCache.Set("DBCon" + tNewCode, oSQLCon, DateTimeOffset.UtcNow.AddHours(5));
            }
            if (aPlantList.Contains(tPlantCode))
            {
                aPlantList.Remove(tPlantCode);
                aPlantList.Add(tNewCode);
            }
        }

        public static cSQLCon SP_READtCacheConStr(string tPlantCode, bool bNewObj = false)
        {
            cSQLCon oSQLCon = new cSQLCon();
            try
            {
                if (!oSP_DBCache.Contains("DBCon" + tPlantCode))
                {
                    oSQLCon = SP_GETtConStr(tPlantCode);
                    oSP_DBCache.Set("DBCon" + tPlantCode, oSQLCon, DateTimeOffset.UtcNow.AddHours(5));
                }
                else
                {
                    if (bNewObj)
                    {
                        oSQLCon = (oSP_DBCache.Get("DBCon" + tPlantCode) as cSQLCon).C_GEToClone();
                    }
                    else
                    {
                        oSQLCon = oSP_DBCache.Get("DBCon" + tPlantCode) as cSQLCon;
                    }
                }
            }
            catch (Exception ex)
            {
                oC_Log.Error(ex.Message);
                throw ex;
            }
            return oSQLCon;
        }

        public static cWebAPICon SP_READtCacheWebAPICon(bool bNewObj = false)
        {
            cWebAPICon oWebAPICon = new cWebAPICon();
            try
            {
                if (!oSP_DBCache.Contains("WebAPICon"))
                {

                    oWebAPICon = SP_LOADtWebAPICon();
                    oSP_DBCache.Set("WebAPICon", oWebAPICon, DateTimeOffset.UtcNow.AddHours(5));
                }
                else
                {
                    if (bNewObj)
                    {
                        oWebAPICon = (oSP_DBCache.Get("WebAPICon") as cWebAPICon).C_GEToClone();
                    }
                    else
                    {
                        oWebAPICon = oSP_DBCache.Get("WebAPICon") as cWebAPICon;
                    }                  
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oWebAPICon;
        }

        public static cScheduleConfig SP_READtCacheScheduleConfig(bool bNewObj = false)
        {
            cScheduleConfig oScheduleConfig = new cScheduleConfig();
            try
            { 
                if (!oSP_DBCache.Contains("ScheduleConfig"))
                {

                    oScheduleConfig = SP_LOADxSchdlConfig();
                    oSP_DBCache.Set("ScheduleConfig", oScheduleConfig, DateTimeOffset.UtcNow.AddHours(5));
                }
                else
                {                   
                    if (bNewObj)
                    {
                        oScheduleConfig = (oSP_DBCache.Get("ScheduleConfig") as cScheduleConfig).C_GEToClone();
                    }
                    else
                    {
                        oScheduleConfig = oSP_DBCache.Get("ScheduleConfig") as cScheduleConfig;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oScheduleConfig;
        }

        public static string SP_READtCacheHashToken()
        {
            string tHashcode = null;
            try
            {
                if (!oSP_DBCache.Contains("PlantHashCode"))
                {
                    string tHashStr = "TheMall" + SP_GETtConfigStr("PlantCode") + "DISTRIB" + SP_GETtConfigStr("PlantName");
                    tHashcode = SP_CALtMD5byStr(tHashStr);
                    oSP_DBCache.Set("PlantHashCode", tHashcode, DateTimeOffset.UtcNow.AddHours(24));
                }
                else
                {
                    tHashcode = oSP_DBCache.Get("PlantHashCode") as string;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tHashcode;
        }

        public static DataTable SP_READxSQL(string ptSQLCmd, string tPlantCode)
        {
            //SqlConnection oC_SqlConn = new SqlConnection((ptDBName == eDBName.DT) ? oC_ConDT.cGETtString() : oC_ConHQ.cGETtString());           
            DataTable oDtTable = new DataTable();
            try
            {
                using (SqlConnection oC_SqlConn = new SqlConnection(SP_READtCacheConStr(tPlantCode).C_GETtString()))
                using (SqlCommand oSqlCmd = new SqlCommand(ptSQLCmd, oC_SqlConn))
                using (SqlDataAdapter oSqlAdt = new SqlDataAdapter(oSqlCmd))
                {
                    oC_SqlConn.Open();
                    oSqlAdt.Fill(oDtTable);
                    oC_SqlConn.Close();
                }
                return oDtTable;
            }
            catch (Exception ex)
            {
                oC_Log.Error("SP_READxSQL:" + ex.Message +"Query:"+ ptSQLCmd);
                throw ex;
            }
        }

        public static string SP_DATEtChngFmt(string ptBaseString,string ptFormat)
        {
            string tResult = "";
            DateTime dResult = new DateTime();
            try
            {
                if(DateTime.TryParse(ptBaseString,out dResult)){
                    tResult = dResult.ToString(ptFormat);
                }
                else
                {
                    throw new Exception("Fails converting datetime string : "+ptBaseString+" ,Format "+ ptFormat);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return tResult;
        }

        public static List<string> SP_SPLITtStr(string ptStr, int nSize)
        {
            string[] aSplitStr = ptStr.Split(' ');
            List<string> aResList = new List<string>();
            StringBuilder oStrBd = new StringBuilder();
            try
            {
                foreach (string tSplitStr in aSplitStr)
                {                    
                    if (oStrBd.Length + tSplitStr.Length + 1 > nSize)
                    {
                        aResList.Add(oStrBd.ToString());
                        oStrBd.Clear();
                        oStrBd.Append(tSplitStr);
                    }
                    else
                    {
                        if (oStrBd.Length > 0)
                        {
                            oStrBd.Append(" ");
                        }
                        oStrBd.Append(tSplitStr);
                    }
                }
                if (oStrBd.Length > 0)
                {
                    aResList.Add(oStrBd.ToString());
                }               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return aResList;
        }

        public static int SP_INSERTxSQL(string ptSQLCmd, string tPlantCode)
        {
            int nResult = 0;
            try
            {
                using (SqlConnection oC_SqlConn = new SqlConnection(SP_READtCacheConStr(tPlantCode).C_GETtString()))
                using (SqlDataAdapter oSqlAdt = new SqlDataAdapter())
                {
                    oSqlAdt.InsertCommand = new SqlCommand(ptSQLCmd, oC_SqlConn);
                    oC_SqlConn.Open();
                    nResult = oSqlAdt.InsertCommand.ExecuteNonQuery();
                    oC_SqlConn.Close();
                }
                return nResult;
            }
            catch (Exception ex)
            {
                oC_Log.Error(ex.Message);
                return 0;
            }
        }

        public static int SP_UPDnSQL(string ptSQLCmd, string tPlantCode)
        {
            int nResult = 0;
            try
            {
                using (SqlConnection oC_SqlConn = new SqlConnection(SP_READtCacheConStr(tPlantCode).C_GETtString()))
                using (SqlDataAdapter oSqlAdt = new SqlDataAdapter())
                {
                    oSqlAdt.UpdateCommand = new SqlCommand(ptSQLCmd, oC_SqlConn);
                    oC_SqlConn.Open();
                    nResult = oSqlAdt.UpdateCommand.ExecuteNonQuery();
                    oC_SqlConn.Close();
                }
                return nResult;
            }
            catch (Exception ex)
            {
                oC_Log.Error(ex.Message);
                return 0;
            }
        }

        public static bool SP_CHKxSQLDataExist(string ptSQLCmd, string tPlantCode)
        {
            DataTable oDT = SP_READxSQL(ptSQLCmd, tPlantCode);
            return (oDT.Rows.Count > 0);
        }

        public static string SP_GETtQueryStr(string tTable, string tCmd)
        {
            string tQueryStr = null;
            string tFilePath = AppDomain.CurrentDomain.BaseDirectory + "Config\\DBQuery.xml";
            XmlDocument oXMLDoc = new XmlDocument();
            try
            {
                oXMLDoc.Load(tFilePath);

                XmlNodeList aNodeList = oXMLDoc.SelectNodes("//Table[@name='" + tTable + "']");
                if (aNodeList.Count > 0)
                {
                    XmlElement InformationElement = (XmlElement)aNodeList[0];
                    tQueryStr = InformationElement.GetElementsByTagName(tCmd)[0].InnerText;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tQueryStr;
        }

        public static cWebAPICon SP_LOADtWebAPICon()
        {
            string tFilePath = AppDomain.CurrentDomain.BaseDirectory + "Config\\Config.xml";
            cWebAPICon oWebAPICon = new cWebAPICon();
            XmlDocument oXMLDoc = new XmlDocument();
            try
            {
                oXMLDoc.Load(tFilePath);
                XmlNodeList oElemList = oXMLDoc.GetElementsByTagName("WebAPIConn");
                foreach (XmlNode node in oElemList)
                {
                    XmlElement InformationElement = (XmlElement)node;
                    oWebAPICon.tServerURI = InformationElement.GetElementsByTagName("ServerURI")[0].InnerText;
                    oWebAPICon.tUsername = InformationElement.GetElementsByTagName("Username")[0].InnerText;
                    oWebAPICon.tPassword = InformationElement.GetElementsByTagName("Password")[0].InnerText;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oWebAPICon;
        }

        public static void SP_SAVExWebAPICon(cWebAPICon poWebAPICon)
        {
            oSP_DBCache.Set("WebAPICon", poWebAPICon, DateTimeOffset.UtcNow.AddHours(5));
            string tFilePath = AppDomain.CurrentDomain.BaseDirectory + "Config\\";
            XmlDocument oXMLDoc = new XmlDocument();
            try
            {
                if (!Directory.Exists(tFilePath))
                {
                    Directory.CreateDirectory(tFilePath);
                }
                tFilePath += "Config.xml";
                if (!File.Exists(tFilePath))
                {
                    using (XmlWriter oWriter = XmlWriter.Create(tFilePath))
                    {
                        oWriter.WriteStartDocument();
                        oWriter.WriteStartElement("mSaleOrderConfig");
                        oWriter.WriteEndElement();
                        oWriter.WriteEndDocument();
                        oWriter.Close();
                    }
                }
                oXMLDoc.Load(tFilePath);
                XmlNode oNode = oXMLDoc.DocumentElement.SelectSingleNode("WebAPIConn");
                if (oNode == null)
                {
                    XmlNode oRootNode = oXMLDoc.DocumentElement;
                    XmlElement oElement;
                    oElement = oXMLDoc.CreateElement("WebAPIConn");
                    oRootNode.AppendChild(oElement);
                    oNode = oRootNode.SelectSingleNode("WebAPIConn");
                }
                XmlNode oChildNode = oNode.SelectSingleNode("ServerURI");
                if (oChildNode == null)
                {
                    XmlElement oElement;
                    oElement = oXMLDoc.CreateElement("ServerURI");
                    oElement.InnerText = poWebAPICon.tServerURI;
                    oNode.AppendChild(oElement);
                    oChildNode = oNode.SelectSingleNode("ServerURI");
                }
                oChildNode.InnerText = poWebAPICon.tServerURI;
                XmlNode oChildNode2 = oNode.SelectSingleNode("Username");
                if (oChildNode2 == null)
                {
                    XmlElement oElement;
                    oElement = oXMLDoc.CreateElement("Username");
                    oElement.InnerText = poWebAPICon.tUsername;
                    oNode.AppendChild(oElement);
                    oChildNode2 = oNode.SelectSingleNode("Username");
                }
                oChildNode2.InnerText = poWebAPICon.tUsername;
                XmlNode oChildNode3 = oNode.SelectSingleNode("Password");
                if (oChildNode3 == null)
                {
                    XmlElement oElement;
                    oElement = oXMLDoc.CreateElement("Password");
                    oElement.InnerText = poWebAPICon.tPassword;
                    oNode.AppendChild(oElement);
                    oChildNode3 = oNode.SelectSingleNode("Password");
                }
                oChildNode3.InnerText = poWebAPICon.tPassword;
                oXMLDoc.Save(tFilePath);
                oXMLDoc = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static cSQLCon SP_GETtConStr(string tPlantCode)
        {
            string tFilePath = AppDomain.CurrentDomain.BaseDirectory + "Config\\Config.xml";
            cSQLCon oConStr = new cSQLCon();
            XmlDocument oXMLDoc = new XmlDocument();
            try
            {
                oXMLDoc.Load(tFilePath);
                XmlNodeList oElemList = oXMLDoc.GetElementsByTagName("ClientDB");
                foreach (XmlNode node in oElemList)
                {
                    XmlElement InformationElement = (XmlElement)node;
                    if (InformationElement.GetAttribute("PlantCode") == tPlantCode)
                    {
                        oConStr.tPlantName = InformationElement.GetElementsByTagName("PlantName").Count > 0 ?
                            InformationElement.GetElementsByTagName("PlantName")[0].InnerText :
                            "";
                        oConStr.tServerURI = InformationElement.GetElementsByTagName("ServerURI").Count > 0 ?
                            InformationElement.GetElementsByTagName("ServerURI")[0].InnerText :
                            "";
                        oConStr.tUsername = InformationElement.GetElementsByTagName("Username").Count > 0 ?
                            InformationElement.GetElementsByTagName("Username")[0].InnerText :
                            "";
                        oConStr.tPassword = InformationElement.GetElementsByTagName("Password").Count > 0 ?
                            InformationElement.GetElementsByTagName("Password")[0].InnerText :
                            "";
                        oConStr.tDBName = InformationElement.GetElementsByTagName("DataBase").Count > 0 ?
                            InformationElement.GetElementsByTagName("DataBase")[0].InnerText :
                            "";
                        return oConStr;
                    }
                }               
            }
            catch (Exception ex)
            {
                oC_Log.Error(ex.Message);
                throw ex;
            }
            return null;
        }

        public static void SP_GETtConStrList()
        {
            string tFilePath = AppDomain.CurrentDomain.BaseDirectory + "Config\\Config.xml";
            string tPlantCode;
            aPlantList.Clear();

            XmlDocument oXMLDoc = new XmlDocument();
            try
            {
                oXMLDoc.Load(tFilePath);
                XmlNodeList oElemList = oXMLDoc.GetElementsByTagName("ClientDB");
                foreach (XmlNode node in oElemList)
                {
                    XmlElement InformationElement = (XmlElement)node;
                    cSQLCon oConStr = new cSQLCon();
                    tPlantCode = InformationElement.GetAttribute("PlantCode");
                    oConStr.tPlantName = InformationElement.GetElementsByTagName("PlantName").Count > 0 ?
                            InformationElement.GetElementsByTagName("PlantName")[0].InnerText :
                            "";
                    oConStr.tServerURI = InformationElement.GetElementsByTagName("ServerURI").Count > 0 ?
                            InformationElement.GetElementsByTagName("ServerURI")[0].InnerText :
                            "";
                    oConStr.tUsername = InformationElement.GetElementsByTagName("Username").Count > 0 ?
                            InformationElement.GetElementsByTagName("Username")[0].InnerText :
                            "";
                    oConStr.tPassword = InformationElement.GetElementsByTagName("Password").Count > 0 ?
                            InformationElement.GetElementsByTagName("Password")[0].InnerText :
                            "";
                    oConStr.tDBName = InformationElement.GetElementsByTagName("DataBase").Count > 0 ?
                            InformationElement.GetElementsByTagName("DataBase")[0].InnerText :
                            "";
                    aPlantList.Add(tPlantCode);
                    oSP_DBCache.Set("DBCon" + tPlantCode, oConStr, DateTimeOffset.UtcNow.AddHours(5));
                }               
            }
            catch (Exception ex)
            {
                oC_Log.Error(ex.Message);
                throw ex;
            }
        }

        public static void SP_SAVExConStr()
        {
            string tFilePath = AppDomain.CurrentDomain.BaseDirectory + "Config\\";
            XmlDocument oXMLDoc = new XmlDocument();
            try
            {
                if (!Directory.Exists(tFilePath))
                {
                    Directory.CreateDirectory(tFilePath);
                }

                tFilePath += "Config.xml";
                if (!File.Exists(tFilePath))
                {
                    using (XmlWriter oWriter = XmlWriter.Create(tFilePath))
                    {
                        oWriter.WriteStartDocument();
                        oWriter.WriteStartElement("mSaleOrderConfig");
                        oWriter.WriteEndElement();
                        oWriter.WriteEndDocument();
                        oWriter.Close();
                    }
                }

                XDocument oXDoc = XDocument.Load(tFilePath);
                oXDoc.Element("mSaleOrderConfig").Elements("DBConfig").Remove();
                var oCfgNode = new XElement("DBConfig");                
                foreach (string tPlantCode in aPlantList)
                {
                    cSQLCon oConStr = (oSP_DBCache.Get("DBCon" + tPlantCode) as cSQLCon).C_GEToClone();//SP_READtCacheConStr(tPlantCode);
                    var oNode =
                        new XElement("ClientDB",
                            new XAttribute("PlantCode", tPlantCode),
                            new XElement("PlantName", oConStr.tPlantName),
                            new XElement("ServerURI", oConStr.tServerURI),
                            new XElement("Username", oConStr.tUsername),
                            new XElement("Password", oConStr.tPassword),
                            new XElement("DataBase", oConStr.tDBName)
                        );

                    oCfgNode.Add(oNode);
                    oConStr = null;
                }
                oXDoc.Element("mSaleOrderConfig").Add(oCfgNode);
                oXDoc.Save(tFilePath);

            }
            catch (Exception ex)
            {
                oC_Log.Error(ex.Message);
                throw ex;
            }
        }

        public static void SP_SAVExConStr(cSQLCon poSQLCon,string ptPlantCode)
        {
            oSP_DBCache.Set("ConStr", poSQLCon, DateTimeOffset.UtcNow.AddHours(5));
            string tFilePath = AppDomain.CurrentDomain.BaseDirectory + "Config\\";
            XmlDocument oXMLDoc = new XmlDocument();
            try
            {
                if (!Directory.Exists(tFilePath))
                {
                    Directory.CreateDirectory(tFilePath);
                }

                tFilePath += "Config.xml";
                if (!File.Exists(tFilePath))
                {
                    using (XmlWriter oWriter = XmlWriter.Create(tFilePath))
                    {
                        oWriter.WriteStartDocument();
                        oWriter.WriteStartElement("mSaleOrderConfig");
                        oWriter.WriteEndElement();
                        oWriter.WriteEndDocument();
                        oWriter.Close();
                    }
                }

                XDocument oXDoc = XDocument.Load(tFilePath);
                oXDoc.Element("mSaleOrderConfig").Elements("ClientDB").Remove();
                foreach (string tPlantCode in aPlantList)
                {
                    cSQLCon oConStr = SP_READtCacheConStr(tPlantCode);
                    var oNode =
                        new XElement("ClientDB",
                            new XAttribute("PlantCode", tPlantCode),
                            new XElement("PlantName", oConStr.tPlantName),
                            new XElement("ServerURI", oConStr.tServerURI),
                            new XElement("Username", oConStr.tUsername),
                            new XElement("Password", oConStr.tPassword),
                            new XElement("DataBase", oConStr.tDBName)
                        );
                    oXDoc.Element("mSaleOrderConfig").Add(oNode);
                }
                oXDoc.Save(tFilePath);

            }
            catch (Exception ex)
            {
                oC_Log.Error(ex.Message);
                throw ex;
            }
        }

        public static string SP_GETtConfigStr(string ptTagName)
        {
            string tFilePath = AppDomain.CurrentDomain.BaseDirectory + "Config\\Config.xml";
            string tResult = null;
            XmlDocument oXMLDoc = new XmlDocument();
            try
            {
                oXMLDoc.Load(tFilePath);
                XmlNodeList oElemList = oXMLDoc.GetElementsByTagName("Config");
                foreach (XmlNode node in oElemList)
                {
                    XmlElement InformationElement = (XmlElement)node;
                    tResult = InformationElement.GetElementsByTagName(ptTagName)[0].InnerText;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tResult;
        }

        public static cScheduleConfig SP_LOADxSchdlConfig()
        {
            string tFilePath = AppDomain.CurrentDomain.BaseDirectory + "Config\\Config.xml";
            cScheduleConfig oScheduleConfig = new cScheduleConfig();
            XmlDocument oXMLDoc = new XmlDocument();
            try
            {
                oXMLDoc.Load(tFilePath);
                oScheduleConfig.aDaySchdl = new List<cDaySchedule>();
                XmlNodeList oElemList = oXMLDoc.GetElementsByTagName("ScheduleConfig");
                foreach (XmlNode node in oElemList)
                {
                    XmlElement oElmDaySchdl = (XmlElement)node;
                    oScheduleConfig.tScheduleType = oElmDaySchdl.SelectSingleNode("ScheduleType").InnerText;
                    oScheduleConfig.tPerTime = oElmDaySchdl.SelectSingleNode("PerTime").InnerText;
                }
                oElemList = oXMLDoc.GetElementsByTagName("DaySchdl");                
                foreach (XmlNode node in oElemList)
                {
                    cDaySchedule oDaySchedule = new cDaySchedule(); 
                    XmlElement oElmDaySchdl = (XmlElement)node;
                    oDaySchedule.tDayTime = oElmDaySchdl.SelectSingleNode("DayTime").InnerText;
                    oDaySchedule.bEnable = oElmDaySchdl.SelectSingleNode("Enable").InnerText == "True";
                    oDaySchedule.tRemark = oElmDaySchdl.SelectSingleNode("Remark").InnerText;
                    oScheduleConfig.aDaySchdl.Add(oDaySchedule);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oScheduleConfig;
        }

        public static void SP_SAVExSchdlConfig(cScheduleConfig poScheduleConfig)
        {
            oSP_DBCache.Set("ScheduleConfig", poScheduleConfig, DateTimeOffset.UtcNow.AddHours(5));
            string tFilePath = AppDomain.CurrentDomain.BaseDirectory + "Config\\";
            XmlDocument oXMLDoc = new XmlDocument();
            try
            {
                if (!Directory.Exists(tFilePath))
                {
                    Directory.CreateDirectory(tFilePath);
                }
                tFilePath += "Config.xml";
                if (!File.Exists(tFilePath))
                {
                    using (XmlWriter oWriter = XmlWriter.Create(tFilePath))
                    {
                        oWriter.WriteStartDocument();
                        oWriter.WriteStartElement("mSaleOrderConfig");
                        oWriter.WriteEndElement();
                        oWriter.WriteEndDocument();
                        oWriter.Close();
                    }
                }
                oXMLDoc.Load(tFilePath);
                XmlNodeList aNodeList = oXMLDoc.GetElementsByTagName("DaySchdl");
                while (aNodeList.Count > 0)
                {
                    XmlNode oParent = aNodeList[0].ParentNode;
                    oParent.RemoveChild(aNodeList.Item(0));
                }
                foreach (XmlNode oDelNode in aNodeList)
                {
                    oDelNode.ParentNode.RemoveChild(oDelNode);
                }
                XmlNode oNode = oXMLDoc.DocumentElement.SelectSingleNode("ScheduleConfig");
                if (oNode == null)
                {
                    XmlNode oRootNode = oXMLDoc.DocumentElement;
                    XmlElement oElement;
                    oElement = oXMLDoc.CreateElement("ScheduleConfig");
                    oRootNode.AppendChild(oElement);
                    oNode = oRootNode.SelectSingleNode("ScheduleConfig");
                }
                XmlNode oChildNode = oNode.SelectSingleNode("ScheduleType");
                if (oChildNode == null)
                {
                    XmlElement oElement;
                    oElement = oXMLDoc.CreateElement("ScheduleType");
                    oElement.InnerText = poScheduleConfig.tScheduleType;
                    oNode.AppendChild(oElement);
                    oChildNode = oNode.SelectSingleNode("ScheduleType");
                }
                oChildNode.InnerText = poScheduleConfig.tScheduleType;
                XmlNode oChildNode2 = oNode.SelectSingleNode("PerTime");
                if (oChildNode2 == null)
                {
                    XmlElement oElement;
                    oElement = oXMLDoc.CreateElement("PerTime");
                    oElement.InnerText = poScheduleConfig.tPerTime;
                    oNode.AppendChild(oElement);
                    oChildNode2 = oNode.SelectSingleNode("PerTime");
                }
                oChildNode2.InnerText = poScheduleConfig.tPerTime;
                foreach (cDaySchedule oDaySchdl in poScheduleConfig.aDaySchdl)
                {
                    XmlElement oElmDaySchdl;
                    XmlElement oElmTime, oElmEnable, oElmRemark;
                    oElmDaySchdl = oXMLDoc.CreateElement("DaySchdl");
                    oElmTime = oXMLDoc.CreateElement("DayTime");
                    oElmTime.InnerText = oDaySchdl.tDayTime;
                    oElmDaySchdl.AppendChild(oElmTime);
                    oElmEnable = oXMLDoc.CreateElement("Enable");
                    oElmEnable.InnerText = oDaySchdl.bEnable.ToString();
                    oElmDaySchdl.AppendChild(oElmEnable);
                    oElmRemark = oXMLDoc.CreateElement("Remark");
                    oElmRemark.InnerText = oDaySchdl.tRemark;
                    oElmDaySchdl.AppendChild(oElmRemark);
                    oNode.AppendChild(oElmDaySchdl);
                }
                oXMLDoc.Save(tFilePath);
                oXMLDoc = null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static string SP_CALtMD5byFile(string ptFilename)
        {
            using (var oMd5 = MD5.Create())
            {
                using (var oStream = File.OpenRead(ptFilename))
                {
                    var oHash = oMd5.ComputeHash(oStream);
                    return BitConverter.ToString(oHash).Replace("-", "");
                }
            }
        }

        public static string SP_CALtMD5byStr(string ptInput)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] aInputBytes = System.Text.Encoding.ASCII.GetBytes(ptInput);
                byte[] aHashBytes = md5.ComputeHash(aInputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder oSb = new StringBuilder();
                for (int i = 0; i < aHashBytes.Length; i++)
                {
                    oSb.Append(aHashBytes[i].ToString("X2"));
                }
                return oSb.ToString();
            }
        }
       
    }

    public class cWebAPICon
    {  
        public string tServerURI { get; set; } = "";
        public string tUsername { get; set; } = "";
        public string tPassword { get; set; } = "";
        public cWebAPICon C_GEToClone()
        {
            cWebAPICon oWebAPICon = new cWebAPICon
            {
                tServerURI = tServerURI,
                tUsername = tUsername,
                tPassword = tPassword
            };
            return oWebAPICon;
        }
    }

    public class cSQLCon
    {
        public cSQLCon() { }
        public cSQLCon(string ptPlantName, string ptServerURI, string ptUsername, string ptPassword, string ptDBName)
        {
            tPlantName = ptPlantName;
            tServerURI = ptServerURI;
            tUsername = ptUsername;
            tPassword = ptPassword;
            tDBName = ptDBName;
        }
        public string C_GETtString()
        {
            string tConStr = "Data Source = " + tServerURI +
                ";Initial Catalog = " + tDBName +
                ";Persist Security Info=True;User ID = " + tUsername +
                ";Password= " + tPassword;
            return tConStr;
        }
        public cSQLCon C_GEToClone()
        {
            cSQLCon oSQLCon = new cSQLCon
            {
                tPlantName = tPlantName,
                tServerURI = tServerURI,
                tUsername = tUsername,
                tPassword = tPassword,
                tDBName = tDBName
            };
            return oSQLCon;
        }
        public string tServerURI { get; set; } = "";
        public string tUsername { get; set; } = "";
        public string tPassword { get; set; } = "";
        public string tDBName { get; set; } = "";
        public string tPlantName { get; set; } = "";
    }

    public class cFileDetail
    {
        public string tFileName { get; set; }
        public long nFileSize { get; set; }
        public string tHashMD5 { get; set; }
        public string tFilePath { get; set; }
    }

    public class cScheduleConfig
    {
        public cScheduleConfig()
        {
            tScheduleType = "";
            tPerTime = "";
            aDaySchdl = new List<cDaySchedule>();
        }
        public string tScheduleType { get; set; }
        public string tPerTime { get; set; }
        public List<cDaySchedule> aDaySchdl { get; set; }
        public cScheduleConfig C_GEToClone()
        {
            cScheduleConfig oScheduleConfig = new cScheduleConfig
            {
                tScheduleType = tScheduleType,
                tPerTime = tPerTime,
                aDaySchdl = new List<cDaySchedule>()
            };
            aDaySchdl.ForEach((oDaySchedule) =>
            {
                oScheduleConfig.aDaySchdl.Add(new cDaySchedule
                {
                    tDayTime = oDaySchedule.tDayTime,
                    tRemark = oDaySchedule.tRemark,
                    bEnable =oDaySchedule .bEnable
                });
            });
            return oScheduleConfig;
         }
    }

    public class cDaySchedule
    {
        public string tDayTime { get; set; }
        public string tRemark { get; set; }
        public bool bEnable { get; set; }
    }
}
