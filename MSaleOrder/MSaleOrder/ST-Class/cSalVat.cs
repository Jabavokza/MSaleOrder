using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSaleOrder.ST_Class
{
    public class cSalVat
    {
        public static DataTable C_READxSalHeader(string ptPlant, bool pbUnsent, bool pbSuccess, bool pbFailed, string ptFTXihDocNo = null)
        {
            DataTable oDTResult = new DataTable();
            StringBuilder oSqlCmd = new StringBuilder();
            oSqlCmd.Append(cCNVB.tVB_SQLSelHeader);
            List<string> aStaList = new List<string>();
            try
            {
                if (pbUnsent)
                {
                    aStaList.Add("'','C','R','D'"); //*KT 61-08-10  Unsent = NULL or '' or '3'   // '3' IS Action UPDATE
                }
                if (pbSuccess)
                {
                    aStaList.Add(aStaList.Count > 0 ? "," : "");
                    aStaList.Add("'1','3','5'");
                }
                if (pbFailed)
                {
                    aStaList.Add(aStaList.Count > 0 ? "," : "");
                    aStaList.Add("'2','4','6'");
                }
                if(aStaList.Count > 0)
                {
                    oSqlCmd.Append("AND ");
                    if (pbUnsent)
                    {
                        oSqlCmd.Append("(HD.FTStaSentOnOff IS NULL OR ");
                    }
                    oSqlCmd.Append("HD.FTStaSentOnOff IN(");
                    foreach(string tStaStr in aStaList)
                    {
                        oSqlCmd.Append(tStaStr);
                    }
                    oSqlCmd.Append(") ");
                    if (pbUnsent)
                    {
                        oSqlCmd.Append(") ");
                    }
                    if (ptFTXihDocNo != null)
                    {
                        oSqlCmd.Append("AND HD.FTShdPlantCode + HD.FTXihDocNo in (" + ptFTXihDocNo + ") ");
                    }
                    oDTResult = cCNSP.SP_READxSQL(oSqlCmd.ToString(), ptPlant);
                }                      
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oDTResult;
        }

        public static DataTable C_READxSalHD(string ptPlant, string ptSendSta = null, string ptFTXihDocNo = null, string ptServName = null)
        {
            DataTable oDTResult = new DataTable();
            string tSQLCmd = cCNVB.tVB_SQLSelSalVatHD;
            try
            {
                switch (ptSendSta)
                {
                    case "0":
                        tSQLCmd += "AND (FTStaSentOnOff IS NULL OR FTStaSentOnOff IN ('','C','R','D') "; //  *KT 61-08-10 1: sent success,2: sent fail,3: update
                        break;
                    case "1":
                        tSQLCmd += "AND FTStaSentOnOff IN ('1','3','5') ";
                        break;
                    case "2":
                        tSQLCmd += "AND FTStaSentOnOff IN ('2','4','6') ";
                        break;
                    case "3":
                        tSQLCmd += "AND (FTStaSentOnOff IS NULL OR FTStaSentOnOff IN ('','C','R','D','2','4','6'))";
                        break;
                }
                if (ptFTXihDocNo != null)
                {
                    tSQLCmd += "AND FTXihDocNo = '" + ptFTXihDocNo + "' ";
                }
                if (ptServName != null)
                {
                    tSQLCmd += "AND FTSRVName = '" + ptServName + "' ";
                }
                oDTResult = cCNSP.SP_READxSQL(tSQLCmd, ptPlant);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oDTResult;
        }

        public static DataTable C_READxSalItem(string ptPlant, string ptSendSta = null, string ptFTXihDocNo = null, string ptServName = null)
        {
            DataTable oDTResult = new DataTable();
            string tSQLCmd = cCNVB.tVB_SQLSelItem;
            try
            {
                switch (ptSendSta)
                {
                    case "0":
                        tSQLCmd += "AND (HD.FTStaSentOnOff IS NULL OR HD.FTStaSentOnOff IN ('','C','R','D')) "; //*KT 61-08-10  Unsent NULL or '' or '3'   // '3' IS Action UPDATE
                        break;
                    case "1":
                        tSQLCmd += "AND HD.FTStaSentOnOff IN ('1','3','5') ";
                        break;
                    case "2":
                        tSQLCmd += "AND HD.FTStaSentOnOff IN ('2','4','6') ";
                        break;
                    case "3":
                        tSQLCmd += "AND (HD.FTStaSentOnOff IS NULL OR HD.FTStaSentOnOff IN ('','C','R','D','2','4','6')) ";
                        break;
                }
                if (ptFTXihDocNo != null)
                {
                    tSQLCmd += "AND HD.FTXihDocNo = '" + ptFTXihDocNo + "' ";
                }
                if (ptServName != null)
                {
                    tSQLCmd += "AND HD.FTSRVName = '" + ptServName + "' ";
                }
                oDTResult = cCNSP.SP_READxSQL(tSQLCmd, ptPlant);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oDTResult;
        }

        public static DataTable C_READxSalTender(string ptPlant, string ptSendSta = null, string ptFTXihDocNo = null,string ptServName = null)
        {
            DataTable oDTResult = new DataTable();
            string tSQLCmd = cCNVB.tVB_SQLSelTender;
            try
            {
                switch (ptSendSta)
                {
                    case "0":
                        tSQLCmd += "AND (HD.FTStaSentOnOff IS NULL OR HD.FTStaSentOnOff IN ('','C','R','D') ) "; //*KT 61-08-10  Unsent NULL or '' or '3'   // '3' IS Action UPDATE
                        break;
                    case "1":
                        tSQLCmd += "AND HD.FTStaSentOnOff IN ('1','3','5') ";
                        break;
                    case "2":
                        tSQLCmd += "AND HD.FTStaSentOnOff IN ('2','4','6') ";
                        break;
                    case "3":
                        tSQLCmd += "AND (HD.FTStaSentOnOff IS NULL OR HD.FTStaSentOnOff IN ('','C','R','D','2','4','6')) ";
                        break;
                }

                if (ptFTXihDocNo != null)
                {
                    tSQLCmd += "AND HD.FTXihDocNo = '" + ptFTXihDocNo + "' ";
                }
                if (ptServName != null)
                {
                    tSQLCmd += "AND HD.FTSRVName = '" + ptServName + "' ";
                }

                oDTResult = cCNSP.SP_READxSQL(tSQLCmd, ptPlant);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oDTResult;
        }

        public static DataTable C_READxSalDiscount(string ptPlant, string ptFTTmnNum, string ptFTShdTransNo, string ptFNSdtSeqNo)
        {
            DataTable oDTResult = new DataTable();
            try
            {
                string tSQLCmd = string.Format(cCNVB.tVB_SQLSelDiscount
                , "'" + ptFTTmnNum + "'"
                , "'" + ptFTShdTransNo + "'"
                , "'" + ptFNSdtSeqNo + "'");           
                oDTResult = cCNSP.SP_READxSQL(tSQLCmd, ptPlant);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oDTResult;
        }
    }
}
