using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MSaleOrder.ST_Class;
using MSaleOrder.WebApiRes;
using Newtonsoft.Json;

namespace MSaleOrder.X_Class
{
    public class cSaleOrderMan
    {
        private static readonly log4net.ILog oC_Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private System.Timers.Timer oC_Timer = new System.Timers.Timer();
        private System.Timers.Timer oC_DayTimer = new System.Timers.Timer();
        private bool bC_PerTime = false;
        private bool bC_TimeEnb = true;
        private List<cTimeOfDay> aC_TimeSchdlList = new List<cTimeOfDay>();
        public object oC_ThreadLock = new object();
        private object oC_SelLock = new object();

        private List<string> aCreateSta = new List<string> { "C", "1", "2"};
        private List<string> aChangeSta = new List<string> { "R", "3", "4" };
        private List<string> aCancelSta = new List<string> { "D", "5", "6" };
        
        public cSaleOrderMan ()
        {
            try
            {
                oC_Timer.Elapsed += On_oC_TimerTick;
                oC_DayTimer.Elapsed += On_oC_DayTimerTick;
                oC_DayTimer.Interval = 1000;
                oC_DayTimer.AutoReset = true;
                C_SETxTimer();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void C_SETxTimeEnable(bool pbTimeEnb)
        {
            bC_TimeEnb = pbTimeEnb;
            oC_Timer.Enabled = bC_PerTime && bC_TimeEnb;
            oC_DayTimer.Enabled = (!bC_PerTime) && bC_TimeEnb;
            oC_Log.Debug("C_SETxTimeEnable :" + pbTimeEnb.ToString());
        }

        public bool C_GETbTimeEnable()
        {
            return bC_TimeEnb;
        }

        public bool C_GETbPerTime()
        {
            return bC_PerTime;
        }

        public void C_SETxTimer()
        {
            try
            {
                oC_Timer.Enabled = false;
                oC_DayTimer.Enabled = false;
                cScheduleConfig oSchdlConf = cCNSP.SP_LOADxSchdlConfig();
                string[] tTime = oSchdlConf.tPerTime.Split(':');
                oC_Timer.Interval = (int.Parse(tTime[0]) * 3600000) + (int.Parse(tTime[1]) * 60000);
                bC_PerTime = oSchdlConf.tScheduleType == "Time Interval";
                aC_TimeSchdlList.Clear();
                foreach (cDaySchedule oDaySchdl in oSchdlConf.aDaySchdl)
                {
                    if (oDaySchdl.bEnable)
                    {
                        string[] tDayTime = oDaySchdl.tDayTime.Split(':');

                        cTimeOfDay oTimeOfDay = new cTimeOfDay
                        {
                            nHour = int.Parse(tDayTime[0])
                            ,
                            nMinute = int.Parse(tDayTime[1])
                        };

                        aC_TimeSchdlList.Add(oTimeOfDay);
                    }
                }
                if (oSchdlConf.tScheduleType == "Time Interval" && oC_Timer.Interval >= 9000)
                {
                    oC_Timer.Enabled = true;
                    oC_Timer.Start();
                    oC_Log.Debug("Timer mode = Time Interval ,Interval = " + (oC_Timer.Interval / 60000).ToString() + " minutes");
                }
                else if (oSchdlConf.tScheduleType == "Time of Day" && aC_TimeSchdlList.Count > 0)
                {
                    oC_DayTimer.Enabled = true;
                    oC_DayTimer.Start();
                    string tSchdlList = "";

                    foreach(cTimeOfDay oTimeOfDay in aC_TimeSchdlList)
                    {
                        tSchdlList += "{" + oTimeOfDay.nHour.ToString("00") + ":" + oTimeOfDay.nMinute.ToString("00") + "} ";
                    }

                    oC_Log.Debug("Timer mode = Time of Day ,TimeList = " + tSchdlList);
                }
            }
            catch (Exception ex)
            {
                oC_Log.Error("C_SETxTimer :" + ex);
            }
        }

        private void On_oC_TimerTick(Object source, ElapsedEventArgs e)
        {
            try
            {
                if (bC_PerTime && bC_TimeEnb)
                {
                    oC_Timer.Stop();
                    oC_Log.Debug("On_oC_TimerTick : Start process on " + String.Join(",", cCNSP.aPlantList.ToArray()));
                    C_PROCxDBSal(cCNSP.aPlantList);
                    oC_Timer.Start();
                }
            }
            catch (Exception ex)
            {
                oC_Log.Error("On_oC_TimerTick:" + ex);
            }
        }

        private void On_oC_DayTimerTick(Object source, ElapsedEventArgs e)
        {
            try
            {
                if (!bC_PerTime && bC_TimeEnb && aC_TimeSchdlList.Count > 0)
                {
                    cTimeOfDay oTimeOfDay = new cTimeOfDay
                    {
                        nHour = DateTime.Now.TimeOfDay.Hours
                        ,
                        nMinute = DateTime.Now.TimeOfDay.Minutes
                    };
                    int nResult = aC_TimeSchdlList.FindIndex(T => (T.nHour == oTimeOfDay.nHour && T.nMinute == oTimeOfDay.nMinute));

                    if (nResult >= 0 && nResult < aC_TimeSchdlList.Count)
                    {
                        if (!aC_TimeSchdlList[nResult].bLastTick)
                        {
                            oC_DayTimer.Stop();
                            oC_Log.Debug("On_oC_DayTimerTick : Start process on " + String.Join(",", cCNSP.aPlantList.ToArray()));
                            C_PROCxDBSal(cCNSP.aPlantList);
                            aC_TimeSchdlList[nResult].bLastTick = true;
                        }
                    }
                    else
                    {
                        nResult = aC_TimeSchdlList.FindIndex(T => (T.bLastTick));
                        if (nResult >= 0 && nResult < aC_TimeSchdlList.Count)
                        {
                            aC_TimeSchdlList[nResult].bLastTick = false;
                        }
                    }
                    oC_DayTimer.Start();
                }
            }
            catch (Exception ex)
            {
                oC_Log.Error("On_oC_DayTimerTick:" + ex);
            }
        }

        public void C_PROCxDBSal(List<string> paPlantList, bool pbUnsent = true, bool pbSuccess = false, bool pbFailed = true, string ptFTXihDocNo = null, string ptAction = "CREATE")
        {
            lock (oC_SelLock)
            {
                foreach (string tPlant in paPlantList) {
                    DataTable oDTSalHeader = cSalVat.C_READxSalHeader(tPlant,pbUnsent, pbSuccess, pbFailed, ptFTXihDocNo);
                    C_PROCxDBSal(tPlant, oDTSalHeader, ptAction);
                }
            }
        }

        private void C_PROCxDBSal(string ptPlant, DataTable poDTSalHeader, string ptAction = "CREATE")
        {
            oC_Log.Debug("Start Processing data from table TPSTSalVatHD Plant :" + ptPlant);
            //mSaleOrder oSaleOrder;
            try
            {
                /// -------Select data header from TPSTSalVatHD where FTStaSendOnOff == null or "0"
                //DataTable oDTSalHeader = cSalVat.C_READxSalHeader("3");
                if (poDTSalHeader != null)
                {
                    int nSentCount = 0;
                    if (poDTSalHeader.Rows.Count > 0)
                    {
                        Program.C_SETxProg(true, poDTSalHeader.Rows.Count, nSentCount);
                    }
                    oC_Log.Debug("Send data :" + poDTSalHeader.Rows.Count +" rescords");
                    foreach (DataRow oDrHD in poDTSalHeader.Rows)
                    {
                        //*KT 61-08-08 Flag Action   CREATE/CANCEL
                        //if (oDrHD["FTXihDCDocStatus"].ToString() == "2" && oDrHD["FTXihDCStatusDC"].ToString() == "4") 
                        if (aCancelSta.Contains(oDrHD["FTStaSentOnOff"].ToString()))
                            ptAction = "DELETE";
                        else if (aChangeSta.Contains(oDrHD["FTStaSentOnOff"].ToString()))//|| Convert.ToInt32(oDrHD["FNShdDocPrint"]) > 1) //*KT 61-08-10 FTStaSentOnOff ='3' is Action UPDATE
                            ptAction = "CHANGE";
                        else
                            ptAction = "CREATE";
                        C_PROCxDataRow(ptPlant, oDrHD, ptAction);                           
                        nSentCount++;
                        Program.C_SETxProg(true, poDTSalHeader.Rows.Count, nSentCount);
                    }
                    oC_Log.Debug("Finish sending data :" + poDTSalHeader.Rows.Count + " records");
                    //if(oSaleOrder.SalesOrder.Header.Count > 0)
                    //{                        
                    //    C_POSTxSalToWebAPi(oSaleOrder);
                    //}
                }
                Program.C_SETxProg(false);
            }
            catch (Exception ex)
            {
                oC_Log.Error("C_PROCxDBSal:"+ex);
                Program.C_SETxProg(false);
            }
        }

        private void C_PROCxDataRow(string ptPlant, DataRow poDrSalHD, string ptAction = "CREATE")
        {
            mSaleOrder oSaleOrder;
            lock (oC_ThreadLock)
            {
                try
                {
                    oSaleOrder = new mSaleOrder { SalesOrder = new SalesOrder() };
                    /// -------Set Header Parameter
                    Header oHeader = new Header
                    {
                        ACTION = ptAction,
                        BELNR = poDrSalHD["BELNR"].ToString(),
                        DLVRY_DATE = cCNSP.SP_DATEtChngFmt(poDrSalHD["DLVRY_DATE"].ToString(), "yyyyMMdd"),
                        REF_BELNR = poDrSalHD["REF_BELNR"].ToString(),
                        TRANSTYPE = poDrSalHD["TRANSTYPE"].ToString(),
                        NAME1 = poDrSalHD["NAME1"].ToString(),
                        NAME2 = poDrSalHD["NAME2"].ToString(),
                        NAME3 = poDrSalHD["NAME3"].ToString(),
                        NAME4 = poDrSalHD["NAME4"].ToString(),
                        LAND = poDrSalHD["LAND"].ToString(),
                        POSTAL_CODE = poDrSalHD["POSTAL_CODE"].ToString(),
                        CITY = poDrSalHD["CITY"].ToString(),
                        DISTRICT = poDrSalHD["DISTRICT"].ToString(),
                        STREET = poDrSalHD["STREET"].ToString(),
                        FAX = poDrSalHD["FAX"].ToString(),
                        TEL = poDrSalHD["TEL"].ToString(),
                        Item = new List<Item>(),
                        TENDER = new List<TENDER>(),
                        TEXT = new List<TEXT>()
                    };

                    string tFullAddr = poDrSalHD["TDLINE1_ZADR"].ToString() + " " + poDrSalHD["TDLINE2_ZADR"].ToString() + " " + poDrSalHD["POSTAL_CODE"].ToString();
                    List<string> aAddrLine = cCNSP.SP_SPLITtStr(tFullAddr, 60);
                    if( aAddrLine.Count > 0)
                        oHeader.STREET = aAddrLine[0];
                    else
                        oHeader.STREET = "";
                    /// -------Fill TEXT List
                    TEXT oTextAdr = new TEXT    /// ZADR TEXT
                    {
                        TDID = poDrSalHD["TDID_ZADR"].ToString(),
                        TDBODY = new List<TDBODY>()
                    };

                    aAddrLine = cCNSP.SP_SPLITtStr(tFullAddr, 70);

                    foreach (string tAddrLine in aAddrLine)
                    {
                        TDBODY oLineBody = new TDBODY { TDLINE = tAddrLine };
                        oTextAdr.TDBODY.Add(oLineBody);
                    }

                    TEXT oTextDlt = new TEXT    /// ZDLT TEXT
                    {
                        TDID = poDrSalHD["TDID_ZDLT"].ToString(),
                        TDBODY = new List<TDBODY> { new TDBODY { TDLINE = poDrSalHD["TDLINE_ZDLT"].ToString() } }
                    };

                    TEXT oTextRem = new TEXT    /// ZREM TEXT
                    {
                        TDID = poDrSalHD["TDID_ZREM"].ToString(),
                        TDBODY = new List<TDBODY> { new TDBODY {TDLINE = poDrSalHD["TDLINE1_ZREM"].ToString()}
                            , new TDBODY {TDLINE = poDrSalHD["TDLINE2_ZREM"].ToString()}}
                    };

                    string tMapName = poDrSalHD["TDLINE_ZMAP"].ToString();

                    if (tMapName != null)
                    {
                        tMapName = Path.GetFileName(tMapName);
                    }

                    TEXT oTextMap = new TEXT    /// ZMAP TEXT
                    {
                        TDID = poDrSalHD["TDID_ZMAP"].ToString(),
                        TDBODY = new List<TDBODY> { new TDBODY { TDLINE = tMapName } }
                    };

                    oHeader.TEXT.Add(oTextAdr);
                    oHeader.TEXT.Add(oTextDlt);
                    oHeader.TEXT.Add(oTextRem);
                    oHeader.TEXT.Add(oTextMap);

                    /// -------Fill Tender List
                    string tFTXihDocNo = poDrSalHD["FTXihDocNo"].ToString();
                    string tServName = poDrSalHD["FTSRVName"].ToString();
                    //string tTenderType;
                    DataTable oDTSalTender = cSalVat.C_READxSalTender(ptPlant, null, tFTXihDocNo, tServName);

                    if (oDTSalTender != null)
                    {
                        foreach (DataRow oDrTender in oDTSalTender.Rows)
                        {
                            //if (oDrTender["TENDER_TYPE"].ToString() == "TOO9")
                            //{
                            //    tTenderType = "TO30";
                            //}
                            //else
                            //{
                            //    tTenderType = oDrTender["TENDER_TYPE"].ToString();
                            //}

                            TENDER oTender = new TENDER
                            {
                                BONUSBUYID = oDrTender["BONUSBUYID"].ToString(),
                                AMOUNT = String.Format("{0:0.00}", oDrTender["AMOUNT"]),
                                TENDER_TYPE = oDrTender["TENDER_TYPE"].ToString()
                            };

                            oHeader.TENDER.Add(oTender);
                        }

                        try
                        {
                            if (Convert.ToDouble(poDrSalHD["FCShdRnd"]) != 0)
                            {
                                TENDER oTender = new TENDER
                                {
                                    BONUSBUYID = "",
                                    AMOUNT = String.Format("{0:0.00}", poDrSalHD["FCShdRnd"]),
                                    TENDER_TYPE = "T032"
                                };

                                oHeader.TENDER.Add(oTender);
                            }
                        }
                        catch (Exception ex)
                        {
                            oC_Log.Error(ex);
                        }
                    }

                    /// -------Fill Item List
                    DataTable oDTSalItem = cSalVat.C_READxSalItem(ptPlant,null, tFTXihDocNo, tServName);

                    if (oDTSalItem != null)
                    {
                        foreach (DataRow oDrItem in oDTSalItem.Rows)
                        {

                            Item oItem = new Item
                            {
                                EANCODE = oDrItem["EANCODE"].ToString(),
                                POSEX = oDrItem["POSEX"].ToString(),
                                UOM = oDrItem["UOM"].ToString(),
                                QUANTITY = String.Format("{0:0.00}", oDrItem["QUANTITY"]),
                                SHIPPING_POINT = oDrItem["SHIPPING_POINT"].ToString(),
                                SELLING_PLANT = oDrItem["SELLING_PLANT"].ToString(),
                                SERIAL = oDrItem["SERIAL"].ToString(),
                                PRICING = new List<PRICING>()
                            };

                            /// -------Fill PRICING List
                            PRICING oPricing = new PRICING
                            {
                                TYPE = oDrItem["TYPE_PRICING"].ToString(),
                                AMOUNT = String.Format("{0:0.00}", oDrItem["AMOUNT_PRICING"]),
                                CURRENCY = oDrItem["CURRENCY_PRICING"].ToString(),
                                SIGN = oDrItem["SIGN_PRICING"].ToString(),
                                BONUSBUYID = oDrItem["BONUSBUYID_PRICING"].ToString()
                            };
                            oItem.PRICING.Add(oPricing);

                            /// -------Fill PRICING List Discount
                            string tPosEx = oDrItem["FNSdtStaRef"].ToString();
                            //if (tPosEx.Length > 1)
                            //{
                            //    tPosEx = tPosEx.Substring(0, tPosEx.Length - 1);
                            //}
                            DataTable oDTSalDisc = cSalVat.C_READxSalDiscount(ptPlant,oDrItem["FTTmnNum"].ToString()
                                , oDrItem["FTShdTransNo"].ToString(), tPosEx);
                            if (oDTSalDisc != null)
                            {
                                foreach (DataRow oDrDisc in oDTSalDisc.Rows)
                                {
                                    PRICING oPricingDsc = new PRICING
                                    {
                                        TYPE = oDrDisc["TYPE"].ToString(),
                                        AMOUNT = String.Format("{0:0.00}", oDrDisc["AMOUNT"]),
                                        // AMOUNT = ((int)(oDrDisc["AMOUNT"])).ToString("0.00"),
                                        CURRENCY = oDrDisc["CURRENCY"].ToString(),
                                        SIGN = oDrDisc["CD_SIGN"].ToString(),
                                        BONUSBUYID = oDrDisc["BONUSBUYID"].ToString()
                                    };
                                    oItem.PRICING.Add(oPricingDsc);
                                }
                            }

                            /// -------Fill PRICING List Vat
                            PRICING oPricingVat = new PRICING
                            {
                                TYPE = oDrItem["TYPE_VAT"].ToString(),
                                AMOUNT = String.Format("{0:0.00}", oDrItem["AMOUNT_VAT"]),
                                CURRENCY = oDrItem["CURRENCY_PRICING"].ToString(),
                                SIGN = oDrItem["SIGN_VAT"].ToString(),
                                BONUSBUYID = oDrItem["BONUSBUYID_VAT"].ToString()
                            };
                            oItem.PRICING.Add(oPricingVat);
                            oHeader.Item.Add(oItem);
                        }
                    }

                    oSaleOrder.SalesOrder.Header = oHeader;
                    oC_Log.Debug("Add Header with BELNR : " + oHeader.BELNR);
                    /// Set FTStaSendOnOff Flag As Fail
                    string tFailSta = ptAction == "DELETE" ? "6" : ptAction == "CHANGE" ? "4" : "2";

                    string tSQLUpdCmd = string.Format(cCNVB.tVB_SQLUpdSalHD, "'" + tFailSta + "'", "'" + poDrSalHD["FTTmnNum"].ToString() + "'"
                        , "'" + poDrSalHD["FTShdTransNo"].ToString() + "'", "'" + poDrSalHD["FTShdPlantCode"].ToString() + "'", "'" + poDrSalHD["FTXihDocNo"].ToString() + "'");

                    cCNSP.SP_UPDnSQL(tSQLUpdCmd,ptPlant);
                    oC_Log.Debug("Update StaSend of SalVatHD with FTXihDocNo : " + poDrSalHD["FTXihDocNo"].ToString());
                    C_POSTxSalToWebAPi(oSaleOrder, ptPlant);
                }
                catch (Exception ex)
                {
                    oC_Log.Error("C_PROCxDataRow:" + ex);
                }
            }
        }

        private void C_POSTxSalToWebAPi(mSaleOrder poSaleOrder,string ptPlantCode)
        {
            string tJsonStr = "";
            string tResult = "";
            cWebAPICon cWebAPICon = cCNSP.SP_READtCacheWebAPICon();
            mSaleOrderRes oSaleOrderRes = new mSaleOrderRes();
            try
            {
                //oC_Log.Debug("Posting request to URI : " + cWebAPICon.tServerURI);
                tJsonStr = JsonConvert.SerializeObject(poSaleOrder);
                oC_Log.Debug("Send Request to : " + cWebAPICon.tServerURI +"\r\n<<<======JsonREQ======>>>\r\n" + tJsonStr + "\r\n<<<======EndJsonREQ======>>>");
                HttpWebRequest oWebReq = (HttpWebRequest)WebRequest.Create(cWebAPICon.tServerURI);
                oWebReq.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(cWebAPICon.tUsername + ":" + cWebAPICon.tPassword)));
                oWebReq.Method = "POST";

                byte[] aData = Encoding.UTF8.GetBytes(tJsonStr.ToString());

                oWebReq.ContentLength = aData.Length;
                oWebReq.ContentType = "application/json;charset=UTF-8";

                using (var oStream = oWebReq.GetRequestStream())
                {
                    oStream.Write(aData, 0, aData.Length);
                }

                oC_Log.Debug("Finish posting to URI : " + cWebAPICon.tServerURI);

                using (HttpWebResponse oResp = (HttpWebResponse)oWebReq.GetResponse())
                //using (StreamReader oSr = new StreamReader(oResp.GetResponseStream()))
                {
                    //tResult = oSr.ReadToEnd();
                    //oSaleOrderRes = JsonConvert.DeserializeObject<mSaleOrderRes>(tResult);
                    oC_Log.Debug("Receive " + oResp.StatusCode.ToString() + " response from URI : " + cWebAPICon.tServerURI);
                    string tPlantCode = poSaleOrder.SalesOrder.Header.BELNR.Substring(0, 4);
                    string tFTTmnNm = poSaleOrder.SalesOrder.Header.BELNR.Substring(4, 5);
                    string tFTShdTransNo = poSaleOrder.SalesOrder.Header.BELNR.Substring(9, 5);
                    string tFTXihDocNo = poSaleOrder.SalesOrder.Header.BELNR.Substring(14);
                    string tStaSendRes = (oResp.StatusCode == HttpStatusCode.OK || oResp.StatusCode == HttpStatusCode.Accepted) ? "FTStaSentOnOff - 1" : "FTStaSentOnOff";
                    string tSQLUpdCmd = string.Format(cCNVB.tVB_SQLUpdSalHD, tStaSendRes
                        , "'" + tFTTmnNm + "'", "'" + tFTShdTransNo + "'", "'" + tPlantCode + "'", "'" + tFTXihDocNo + "'");
                    cCNSP.SP_UPDnSQL(tSQLUpdCmd, ptPlantCode);
                }               
            }
            catch (Exception ex)
            {
                oC_Log.Error("C_POSTxSalToWebAPi:" + ex);
            }
        }
    }

    class cTimeOfDay
    {        
        public bool bLastTick { get; set; } = false;
        public int nHour { get; set; }
        public int nMinute { get; set; }
    }
}
