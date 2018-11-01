using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using MSaleOrder.ST_Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSaleOrder
{
    public partial class wMSaleOrderMain : Form
    {
        private static readonly log4net.ILog oC_Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        cScheduleConfig oW_ScheduleConfig = new cScheduleConfig();
        cSQLCon oW_SQLCon = new cSQLCon();
        cWebAPICon oW_WebAPICon = new cWebAPICon();
        BindingSource oW_BDSchdlTime = new BindingSource();
        BindingSource oW_BDSalHD = new BindingSource();
        DataTable oW_DTSalHD = new DataTable();
        wManualSend oW_ManualSend = null;
        object oW_TaskLock = new object();
        bool bW_SchdlChng = false;
        bool bW_ConfigChng = false;
        bool bW_ConRes = false;
        string tW_SndSta = null;
        public string tW_SelPlant = "";
        string tW_LogFileName;
        public string tW_CngPlntCd = null;
        
        public wMSaleOrderMain()
        {
            InitializeComponent();
            this.Icon = MSaleOrder.Properties.Resources.connected;
            W_INITxMain();
            this.Text = "MSaleOrder (V." + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";
        }

        private void W_INITxMain()
        {
            try
            {
                otaConfig.Selecting += new TabControlCancelEventHandler(otaConfig_Selecting);
                otaConfig.Appearance = TabAppearance.FlatButtons;
                otaConfig.ItemSize = new Size(0, 1);
                otaConfig.SizeMode = TabSizeMode.Fixed;
                ocbSelPlant.Items.AddRange(cCNSP.aPlantList.ToArray());
                if (ocbSelPlant.Items.Count > 0) ocbSelPlant.SelectedIndex = 0;
                for (int nIndex = 0; nIndex < 60; nIndex++)
                {
                    ocbTimeMinute.Items.Add(nIndex);
                    ocbDayMinute.Items.Add(nIndex);
                }
                for (int nIndex = 0; nIndex < 24; nIndex++)
                {
                    ocbTimeHour.Items.Add(nIndex);
                    ocbDayHour.Items.Add(nIndex);
                }
                ocbTimeMinute.SelectedIndexChanged += new EventHandler(ocbTimeHour_SelectedIndexChanged);
                ocbTimeHour.SelectedIndexChanged += new EventHandler(ocbTimeHour_SelectedIndexChanged);
                odgSchdlTime.RowsAdded += new DataGridViewRowsAddedEventHandler(odgSchdlTime_RowsAdded);
                odgDashBoard.RowsAdded += new DataGridViewRowsAddedEventHandler(odgDashBoard_RowsAdded);
                odgDashBoard.AutoGenerateColumns = false;
                oW_BDSalHD.DataSource = oW_DTSalHD;
                odgDashBoard.DataSource = oW_BDSalHD;
                W_INITxSchdlValue();
                W_INITxConfigValue();
                tW_SndSta = null;
                var rootAppender = ((Hierarchy)LogManager.GetRepository())
                                             .Root.Appenders.OfType<FileAppender>()
                                             .FirstOrDefault();
                tW_LogFileName = rootAppender != null ? rootAppender.File : string.Empty;
                olaLogPath.Text = tW_LogFileName;
                otaLogConsole.SuspendLayout();
                otbLogTextBox = Program.oC_LogTextBox;
                otbLogTextBox.Cursor = System.Windows.Forms.Cursors.Default;
                otbLogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
                otbLogTextBox.Location = new System.Drawing.Point(5, 35);
                otbLogTextBox.MaxLength = 65535;
                otbLogTextBox.Multiline = true;
                otbLogTextBox.ReadOnly = true;
                otbLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
                otbLogTextBox.Size = new System.Drawing.Size(566, 250);
                otbLogTextBox.TabIndex = 0;
                otaLogConsole.Controls.Add(otbLogTextBox);
                otbLogTextBox.BringToFront();
                otaLogConsole.ResumeLayout(false);
                otaLogConsole.PerformLayout();
                oW_ManualSend = new wManualSend(this);
                oW_ManualSend.FormClosed += ManualSend_FormClosed;
                W_ReadSalDB();
            }
            catch (Exception ex)
            {
                oC_Log.Error("W_INITxMain :" + ex);
            }
        }

        private void wMSaleOrderMain_Shown(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void wMSaleOrderMain_Activated(object sender, EventArgs e)
        {
            
        }

        private void W_INITxSchdlValue()
        {                                
            try
            {
                oW_ScheduleConfig = cCNSP.SP_READtCacheScheduleConfig(true);
                string[] tTime = oW_ScheduleConfig.tPerTime.Split(':');
                int nHour, nMin;
                ocbDayHour.SelectedIndex = 0;                
                ocbDayMinute.SelectedIndex = 0;
                if (tTime.Count() > 1)
                {
                    ocbTimeHour.SelectedIndex = int.TryParse(tTime[0], out nHour) ? nHour : 0;
                    ocbTimeMinute.SelectedIndex = int.TryParse(tTime[1], out nMin) ? nMin : 0;
                }
                else
                {
                    ocbTimeHour.SelectedIndex = 0;
                    ocbTimeMinute.SelectedIndex = 0;
                }                
                oW_BDSchdlTime.DataSource = oW_ScheduleConfig.aDaySchdl;
                odgSchdlTime.DataSource = oW_BDSchdlTime;
                oW_BDSchdlTime.ResetBindings(false);
                if (oW_ScheduleConfig.tScheduleType == "Time of Day")
                {
                    oraPerDay.Checked = true;
                }
                else
                {
                    oraTmInterval.Checked = true;
                }
                bW_SchdlChng = false;
                W_SETxChngNoti();
                W_GETxTimeEnb();
            }
            catch (Exception ex)
            {
                oC_Log.Error("W_INITxSchdlValue :" + ex);
            }
        }

        private void W_LOADxPlantList()
        {
            olbPlantList.Items.Clear();
            olbPlantList.Items.AddRange(cCNSP.aPlantList.ToArray());
            ocbSelPlant.Items.Clear();
            ocbSelPlant.Items.AddRange(cCNSP.aPlantList.ToArray());
           
            if (olbPlantList.Items.Count > 0)
            {
                olbPlantList.SelectedIndex = 0;
            }
            if (ocbSelPlant.Items.Count > 0) ocbSelPlant.SelectedIndex = 0;
        }

        private void W_INITxConfigValue()
        {           
            try
            {
                if (cCNSP.aPlantList.Count == 0)
                {
                    cCNSP.SP_LOADbConfig();
                }
                W_LOADxPlantList();
                if (cCNSP.aPlantList.Count > 0)
                {
                    oW_SQLCon = cCNSP.SP_READtCacheConStr(cCNSP.aPlantList[0], true);
                }
                oW_WebAPICon = cCNSP.SP_READtCacheWebAPICon(true);
                otbDBServer.Text = oW_SQLCon.tServerURI;
                otbDBName.Text = oW_SQLCon.tDBName;
                otbDBUser.Text = oW_SQLCon.tUsername;
                otbDBPassword.Text = oW_SQLCon.tPassword;
                otbWebURI.Text = oW_WebAPICon.tServerURI;
                otbWebUser.Text = oW_WebAPICon.tUsername;
                otbWebPassword.Text = oW_WebAPICon.tPassword;
                bW_ConfigChng = false;
                W_SETxChngNoti();
            }
            catch (Exception oEx)
            {
                oC_Log.Error("W_INITxConfigValue :" + oEx);
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show("Error : " + oEx.Message);
            }
        }

        public void W_GETxTimeEnb()
        {
            try
            {
                if (Program.oC_SaleOrderMan == null) return;
                bool bTimeEnb = Program.oC_SaleOrderMan.C_GETbTimeEnable();
                olaSchdlStat.Text = bTimeEnb ? "Status : Enable" : "Status : Disable";
                otoTmEnbStat.Text = bTimeEnb ? "Schedule : Enable" : "Schedule : Disable";
                ocmSchdlToggle.Text = bTimeEnb ? "Disable" : "Enable";
                otoModeStat.Text = Program.oC_SaleOrderMan.C_GETbPerTime() ? "Mode : Time Interval" : "Mode : Time of Day";
            }
            catch(Exception ex)
            {
                oC_Log.Error("W_GETxTimeEnb :" + ex);
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show(ex.Message);
            }
        }

        private void W_SETxSchdlItemEnb()
        {
            ocbTimeHour.Enabled = ocbTimeMinute.Enabled = oraTmInterval.Checked;
            ocbDayHour.Enabled = ocbDayMinute.Enabled = odgSchdlTime.Enabled = oraPerDay.Checked;
            ocmAddSchdl.Enabled  = oraPerDay.Checked;
        }

        private bool W_ReadSalDB()
        {
            try
            {
                oW_DTSalHD = cSalVat.C_READxSalHD(ocbSelPlant.SelectedItem.ToString(),tW_SndSta);
                oW_BDSalHD.DataSource = oW_DTSalHD;
                oW_BDSalHD.ResetBindings(false);
                return true;
            }
            catch (Exception ex)
            {
                oC_Log.Error("W_ReadSalDB:"+ex);
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show(this,"Error : " + ex.Message);
                return false;
            }
        }

        private void W_SETxChngNoti()
        {
            try
            {
                if (bW_SchdlChng) otoSchdl.Text = "Schdl.*";
                else otoSchdl.Text = "Schdl.";
                if (bW_ConfigChng) otoConfig.Text = "Config*";
                else otoConfig.Text = "Config";
            }
            catch (Exception ex)
            {
                oC_Log.Error("W_SETxChngNoti :" + ex);
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show(this, "Error : " + ex.Message);
            }
        }

        public void W_SETxProg(bool pbProgRunning, int pnTotalRec = 0, int pnSentRec = 0)
        {
            try
            {
                if (pbProgRunning && pnTotalRec > 0)
                {
                    otbLogTextBox.Invoke(() => otoProgText.Text = "Tranferring Data " + pnSentRec.ToString() + " of " + pnTotalRec.ToString());
                    otbLogTextBox.Invoke(() => otoProgBar.Visible = true);
                    otbLogTextBox.Invoke(() => otoProgBar.Value = (pnSentRec * 100) / pnTotalRec);
                }
                else
                {
                    otbLogTextBox.Invoke(() => otoProgText.Text = "");
                    otbLogTextBox.Invoke(() => otoProgBar.Visible = false);
                }
            }
            catch (Exception ex)
            {
                oC_Log.Error("W_SETxProg :" + ex);
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show(this, "Error : " + ex.Message);
            }
        }

        private void oraTmInterval_CheckedChanged(object sender, EventArgs e)
        {
            oraPerDay.Checked = !oraTmInterval.Checked;
            oW_ScheduleConfig.tScheduleType = "Time Interval";
            W_SETxSchdlItemEnb();
            bW_SchdlChng = true;
            W_SETxChngNoti();
        }

        private void oraPerDay_CheckedChanged(object sender, EventArgs e)
        {
            oraTmInterval.Checked = !oraPerDay.Checked;
            oW_ScheduleConfig.tScheduleType = "Time of Day";
            W_SETxSchdlItemEnb();
            bW_SchdlChng = true;
            W_SETxChngNoti();
        }

        private void ocmAddSchdl_Click(object sender, EventArgs e)
        {
            try
            {
               // string tTime = string.Format("{0:00}:{1:00}", int.Parse(ocbDayHour.SelectedItem.ToString())
               //     , int.Parse(ocbDayMinute.SelectedItem.ToString()));
                string tTime = string.Format("{0:00}:{1:00}", (int)(ocbDayHour.SelectedItem)
                    , (int)(ocbDayMinute.SelectedItem));
                if (oW_ScheduleConfig.aDaySchdl.Exists(T => T.tDayTime == tTime)) return;
                cDaySchedule oDaySchedule = new cDaySchedule { tDayTime = tTime, bEnable = true, tRemark = "" };
                oW_ScheduleConfig.aDaySchdl.Add(oDaySchedule);
                oW_ScheduleConfig.aDaySchdl.Sort((x, y) => x.tDayTime.CompareTo(y.tDayTime));
                oW_BDSchdlTime.ResetBindings(false);
                bW_SchdlChng = true;
                W_SETxChngNoti();
            }
            catch (Exception ex)
            {
                oC_Log.Error("ocmAddSchdl_Click :" + ex.Message);
            }
        }

        private void odgSchdlTime_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns[e.ColumnIndex].Name == "GrdColDel" &&
                e.RowIndex >= 0)
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                DialogResult oDlRes = MessageBox.Show("You want to delete this row?", "Delete Confirmation", MessageBoxButtons.OKCancel);
                if (oDlRes == DialogResult.OK)
                {
                    senderGrid.Rows.RemoveAt(e.RowIndex);
                    bW_SchdlChng = true;
                    W_SETxChngNoti();
                }
            }
        }
      
        private void otoSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (otaConfig.SelectedTab == otaSchdlConfig)
                {
                    cCNSP.SP_SAVExSchdlConfig(oW_ScheduleConfig);
                    bW_SchdlChng = false;                    
                }
                else if (otaConfig.SelectedTab == otaConnConfig)
                {
                    cCNSP.SP_SAVExConStr();
                    cCNSP.SP_SAVExWebAPICon(oW_WebAPICon);
                    bW_ConfigChng = false;
                    ocbSelPlant.Items.Clear();
                    ocbSelPlant.Items.AddRange(cCNSP.aPlantList.ToArray());
                    if (ocbSelPlant.Items.Count > 0) ocbSelPlant.SelectedIndex = 0;
                }
                W_SETxChngNoti();
                Program.oC_SaleOrderMan.C_SETxTimer();
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show(this, "Configuration saved successfully.");
                otoModeStat.Text = Program.oC_SaleOrderMan.C_GETbPerTime() ? "Mode : Time Interval" : "Mode : Time of Day";
            }
            catch (Exception ex)
            {
                oC_Log.Error("otoSave_Click :" + ex.Message);
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show(this, "Error : " + ex.Message);
            }
        }

        private void odgSchdlTime_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns.Contains("GrdColTime"))
            {
                for (int nIndex = 0; nIndex < e.RowCount; nIndex++)
                {
                    if(senderGrid.Rows[e.RowIndex + nIndex].Cells["GrdColTime"].Value != null)
                    senderGrid.Rows[e.RowIndex + nIndex].Cells["GrdColDel"].Value = "Delete";
                }
            }
        }

        private void odgDashBoard_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {           
            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns.Contains("FTTmnNum"))
            {
                for (int nIndex = 0; nIndex < e.RowCount; nIndex++)
                {
                    var oStaSnd = senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Value;
                    if (oStaSnd != null)
                    {
                        switch (oStaSnd.ToString())
                        {
                            case "":
                            case "C":
                            case "R":
                            case "D":
                                //senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Value = "Unsent";
                                senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Style.BackColor = Color.LightYellow;
                                break;
                            case "1":
                            case "3":
                            case "5":
                                //senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Value = "Success";
                                senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Style.BackColor = Color.LightGreen;
                                break;
                            case "2":
                            case "4":
                            case "6":
                                //senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Value = "Failed";
                                senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Style.BackColor = Color.PaleVioletRed;
                                break;
                            //case "3":
                            //    //senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Value = "Update";
                            //    senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Style.BackColor = Color.LightYellow;
                            //    break;
                            //case "Unsent":
                            //    senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Style.BackColor = Color.LightYellow;
                            //    break;
                            //case "Update":
                            //    senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Style.BackColor = Color.LightYellow;
                            //    break;
                            //case "Success":
                            //    senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Style.BackColor = Color.LightGreen;
                            //    break;
                            //case "Failed":
                            //    senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Style.BackColor = Color.PaleVioletRed;
                            //    break;
                                //default:
                                //    senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Value = "Unsent";
                                //    senderGrid.Rows[e.RowIndex + nIndex].Cells["FTStaSentOnOff"].Style.BackColor = Color.LightYellow;
                                //    break;
                        }                   
                    }
                }
            }
        }

        private void otbDBServer_TextChanged(object sender, EventArgs e)
        {
            if (oW_SQLCon != null)
            {
                oW_SQLCon.tServerURI = otbDBServer.Text;
                bW_ConfigChng = true;
                W_SETxChngNoti();
            }
        }

        private void otbDBUser_TextChanged(object sender, EventArgs e)
        {
            if (oW_SQLCon != null)
            {
                oW_SQLCon.tUsername = otbDBUser.Text;
                bW_ConfigChng = true;
                W_SETxChngNoti();
            }
        }

        private void otbDBName_TextChanged(object sender, EventArgs e)
        {
            if (oW_SQLCon != null)
            {
                oW_SQLCon.tDBName = otbDBName.Text;
                bW_ConfigChng = true;
                W_SETxChngNoti();
            }
        }

        private void otbDBPassword_TextChanged(object sender, EventArgs e)
        {
            if (oW_SQLCon != null)
            {
                oW_SQLCon.tPassword = otbDBPassword.Text;
                bW_ConfigChng = true;
                W_SETxChngNoti();
            }
        }

        private void otbWebURI_TextChanged(object sender, EventArgs e)
        {
            oW_WebAPICon.tServerURI = otbWebURI.Text;
            bW_ConfigChng = true;
            W_SETxChngNoti();
        }

        private void otbWebUser_TextChanged(object sender, EventArgs e)
        {
            oW_WebAPICon.tUsername = otbWebUser.Text;
            bW_ConfigChng = true;
            W_SETxChngNoti();
        }

        private void otbWebPassword_TextChanged(object sender, EventArgs e)
        {
            oW_WebAPICon.tPassword = otbWebPassword.Text;
            bW_ConfigChng = true;
            W_SETxChngNoti();
        }

        private void ocbTimeHour_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ocbTimeHour.SelectedIndex != -1 && ocbTimeMinute.SelectedIndex != -1)
                {
                    string tTime = string.Format("{0:00}:{1:00}", int.Parse(ocbTimeHour.SelectedItem.ToString())
                        , int.Parse(ocbTimeMinute.SelectedItem.ToString()));
                    oW_ScheduleConfig.tPerTime = tTime;
                    bW_SchdlChng = true;
                    W_SETxChngNoti();
                }
            }
            catch(Exception ex)
            {
                oC_Log.Error("ocbTimeHour_SelectedIndexChanged :" + ex.Message);
            }
        }

        private void otoHome_Click(object sender, EventArgs e)
        {
            otaConfig.SelectedTab = otaDashBoard;
        }

        private void otoSchdl_Click(object sender, EventArgs e)
        {
            otaConfig.SelectedTab = otaSchdlConfig;
        }

        private void otoConfig_Click(object sender, EventArgs e)
        {
            otaConfig.SelectedTab = otaConnConfig;
        }

        private void otaLogView_Click(object sender, EventArgs e)
        {
            otaConfig.SelectedTab = otaLogConsole;
        }

        private void ocmOpenLogFile_Click(object sender, EventArgs e)
        {
            Process.Start(tW_LogFileName);
        }

        private void ockBELNRView_CheckedChanged(object sender, EventArgs e)
        {
            odgDashBoard.Columns["BELNR"].Visible = ockBELNRView.Checked;
            odgDashBoard.Columns["FTTmnNum"].Visible = !ockBELNRView.Checked;
            odgDashBoard.Columns["FTShdTransNo"].Visible = !ockBELNRView.Checked;
            odgDashBoard.Columns["FTXihDocNo"].Visible = !ockBELNRView.Checked;
        }

        private void oraSndAllSta_CheckedChanged(object sender, EventArgs e)
        {
            if (oraSndAllSta.Checked) tW_SndSta = null;
            else if (oraNotSend.Checked) tW_SndSta = "0";
            else if (oraSndSuccess.Checked) tW_SndSta = "1";
            else if (oraSndFailed.Checked) tW_SndSta = "2";
            W_ReadSalDB();
        }

        private void ockCstmDetail_CheckedChanged(object sender, EventArgs e)
        {
            odgDashBoard.Columns["FTXihCstName"].Visible = ockCstmDetail.Checked;
            odgDashBoard.Columns["FTXihCstAddr1"].Visible = ockCstmDetail.Checked;
            odgDashBoard.Columns["FTXihCstAddr2"].Visible = ockCstmDetail.Checked;
        }

        private void ockRemark_CheckedChanged(object sender, EventArgs e)
        {
            odgDashBoard.Columns["FTXihDepositDueTime"].Visible = ockRemark.Checked;
            odgDashBoard.Columns["FTXihRemark"].Visible = ockRemark.Checked;
            odgDashBoard.Columns["FTXihEquipment"].Visible = ockRemark.Checked;
            odgDashBoard.Columns["FTRemark"].Visible = ockRemark.Checked;
        }

        private void odgSchdlTime_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            bW_SchdlChng = true;
        }

        private void otaConfig_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void otaConfig_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (otaConfig.SelectedTab == otaSchdlConfig || otaConfig.SelectedTab == otaConnConfig)
            {
                otoSave.Enabled = true;
            }
            else
            {
                otoSave.Enabled = false;
            }
        }

        private void otoReload_Click(object sender, EventArgs e)
        {
            try
            {
                if (otaConfig.SelectedTab == otaSchdlConfig)
                {
                    
                    W_INITxSchdlValue();
                }
                else if (otaConfig.SelectedTab == otaConnConfig)
                {
                    W_INITxConfigValue();
                }
                else if(otaConfig.SelectedTab == otaDashBoard)
                {
                    W_ReadSalDB();
                }
            }
            catch (Exception ex)
            {
                oC_Log.Error("otoReload_Click :" + ex.Message);
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show(this, "Error : " + ex.Message);
            }
        }

        private void wMSaleOrderMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            otaLogConsole.Controls.Remove(otbLogTextBox);
            otbLogTextBox = null;
        }

        private void ocmSchdlToggle_Click(object sender, EventArgs e)
        {          
            ocmSchdlToggle.Enabled = false;
            Program.oC_SaleOrderMan.C_SETxTimeEnable(ocmSchdlToggle.Text == "Enable");
            W_GETxTimeEnb();
            ocmSchdlToggle.Enabled = true;
        }

        private void ocmManualSend_Click(object sender, EventArgs e)
        {
            wManualSend.tW_SelPlant = ocbSelPlant.SelectedItem.ToString();
            if (Program.oC_SaleOrderMan.C_GETbTimeEnable())
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show(this, "Schedule task must be disable before use \"Manual Send\" function.");
            }
            else
            {
                if (Monitor.TryEnter(Program.oC_SaleOrderMan.oC_ThreadLock))
                {
                    Monitor.Exit(Program.oC_SaleOrderMan.oC_ThreadLock);
                    if(oW_ManualSend == null)
                    {
                        oW_ManualSend = new wManualSend(this);
                        oW_ManualSend.FormClosed += ManualSend_FormClosed;
                    }
                    else if (oW_ManualSend.IsDisposed)
                    {
                        oW_ManualSend = new wManualSend(this);
                        oW_ManualSend.FormClosed += ManualSend_FormClosed;
                    }
                    oW_ManualSend.W_SETxSelPlant();
                    if (!oW_ManualSend.Visible)
                    {                        
                        oW_ManualSend.ShowDialog(this);
                        if (wManualSend.bW_Send)
                        {
                            Task.Run(() =>
                            {
                                lock (oW_TaskLock)
                                {
                                    W_PROCxData(wManualSend.aW_SelPlant, wManualSend.bW_StaUnsent, wManualSend.bW_StaSuccess
                                     , wManualSend.bW_StaFailed, wManualSend.bW_SendAll);
                                }
                            });
                        }
                    }
                }
                else
                {
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show(this, "Transfer process is still running.");
                }
            }
        }

        private void ManualSend_FormClosed(object sender, FormClosedEventArgs e)
        {
            //oW_ManualSend = null;
            //if (wManualSend.bW_Send)
            //{
            //    W_PROCxData(wManualSend.bW_StaUnsent, wManualSend.bW_StaSuccess
            //        , wManualSend.bW_StaFailed, wManualSend.bW_SendAll);
            //}
        }

        public void W_PROCxData(List<string> paPlantList,bool pbUnsent, bool pbSuccess, bool pbFailed, bool pbSendAll)
        {
            StringBuilder oDocList = new StringBuilder();
            if (!pbSendAll)
            {
                foreach(DataGridViewRow oDgRow in odgDashBoard.SelectedRows)
                {
                    if (oDocList.Length > 0)
                    {
                        oDocList.Append(",");
                    }

                    oDocList.Append("'"+ oDgRow.Cells["FTShdPlantCode"].Value.ToString() + oDgRow.Cells["FTXihDocNo"].Value.ToString()+"'");
                }
            }
            Program.oC_SaleOrderMan.C_PROCxDBSal(paPlantList, pbUnsent, pbSuccess, pbFailed
                , oDocList.Length > 0 ? oDocList.ToString() : null);
            W_ReadSalDB();
        }

        private void ocmTestSQLCon_Click(object sender, EventArgs e)
        {
            string tConStr = "Data Source = " + otbDBServer.Text +
                ";Initial Catalog = " + otbDBName.Text +
                ";Persist Security Info=True;User ID = " + otbDBUser.Text +
                ";Password= " + otbDBPassword.Text;

            using (SqlConnection oC_SqlConn = new SqlConnection(tConStr))
            {
                try
                {
                    oC_SqlConn.Open();
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show(this, "Connecting to Database success","Connection success");

                    if (otoConfig.Text.Contains("*"))
                    {
                        MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                        DialogResult oDRes = MessageBox.Show(this, "Save current configuration?", "Save configuration",MessageBoxButtons.YesNo);
                        if(oDRes == DialogResult.Yes)
                        {
                            cCNSP.SP_SAVExConStr();
                            cCNSP.SP_SAVExWebAPICon(oW_WebAPICon);
                            bW_ConfigChng = false;
                            W_SETxChngNoti();
                        }
                    }
                }
                catch(Exception ex)
                {
                    oC_SqlConn.Close();
                    MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                    MessageBox.Show(this, ex.Message,"Connection failed");
                }
            }
        }

        private void ockShowPassWeb_CheckedChanged(object sender, EventArgs e)
        {
            if (ockShowPassWeb.Checked)
            {
                otbWebPassword.PasswordChar = '\0';
            }
            else
            {
                otbWebPassword.PasswordChar = '*';
            }
        }

        private void ockShowPassDB_CheckedChanged(object sender, EventArgs e)
        {
            if (ockShowPassDB.Checked)
            {
                otbDBPassword.PasswordChar = '\0';
            }
            else
            {
                otbDBPassword.PasswordChar = '*';
            }
        }

        private void otaToolMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                otbLogTextBox.Invoke(() => otoDBConnection.Text = "Connecting DB");
                otbLogTextBox.Invoke(() => bW_ConRes = W_ReadSalDB());
            }
            catch (Exception ex)
            {
                oC_Log.Warn("backgroundWorker1_DoWork :" + ex);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                otbLogTextBox.Invoke(() => otoDBConnection.Text = "Reading DB");
            }
            catch (Exception ex)
            {
                oC_Log.Error("backgroundWorker1_ProgressChanged :" + ex);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                string tRes = bW_ConRes ? "Read DB Complete" : "Can't connect DB";
                otbLogTextBox.Invoke(() => otoDBConnection.Text = tRes);
            }
            catch (Exception ex)
            {
                oC_Log.Error("backgroundWorker1_RunWorkerCompleted :" + ex);
            }
        }

        private void ocbSelPlant_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ocbSelPlant.SelectedIndex != -1)
                {
                    tW_SelPlant = ocbSelPlant.SelectedItem.ToString();
                }
                if (oraSndAllSta.Checked) tW_SndSta = null;
                else if (oraNotSend.Checked) tW_SndSta = "0";
                else if (oraSndSuccess.Checked) tW_SndSta = "1";
                else if (oraSndFailed.Checked) tW_SndSta = "2";
                W_ReadSalDB();
            }
            catch (Exception ex)
            {
                oC_Log.Error("ocbSelPlant_SelectedIndexChanged :" + ex);
            }
        }

        private void ocmPCCng_Click(object sender, EventArgs e)
        {
            try
            {
                string tRefName = otbSGRPlantCode.Text;
                wPlantCode wChild = new wPlantCode(this, tRefName);
                wChild.ShowDialog(this);
                if (tW_CngPlntCd != null)
                {
                    cCNSP.SP_CHNGtCacheConStr(tRefName, tW_CngPlntCd);
                    int nIndex = olbPlantList.SelectedIndex;
                    olbPlantList.Invoke(T => T.Items[nIndex] = tW_CngPlntCd);
                    //olbSGRPlantList.Items.Insert(nIndex,tW_CngPlntCd);
                    tW_CngPlntCd = null;
                }
            }
            catch (Exception ex)
            {
                oC_Log.Error("ocmPCCng_Click :" + ex);
            }
        }

        private void olbPlantList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (olbPlantList.SelectedIndex == -1)
                {
                    ocmSGRDel.Enabled = false;
                    oW_SQLCon = null;
                    otbSGRPlantCode.Text = "";
                    otbSGRPlantName.Text = "";
                    otbDBServer.Text = "";
                    otbDBName.Text = "";
                    otbDBUser.Text = "";
                    otbDBPassword.Text = "";
                }
                else
                {
                    ocmSGRDel.Enabled = true;
                    W_SETxBranchDialog(olbPlantList.SelectedItem.ToString());
                }
            }
            catch (Exception ex)
            {
                oC_Log.Error("olbPlantList_SelectedIndexChanged :" + ex);
            }
        }
        private void W_SETxBranchDialog(string ptPlantCode)
        {
            try
            {
                if (ptPlantCode != null)
                {
                    if (cCNSP.aPlantList.Contains(ptPlantCode) || ptPlantCode == "Prem")
                    {
                        oW_SQLCon = cCNSP.SP_READtCacheConStr(ptPlantCode);
                        otbSGRPlantCode.Text = ptPlantCode;
                        otbSGRPlantName.Text = oW_SQLCon.tPlantName;
                        otbDBServer.Text = oW_SQLCon.tServerURI;
                        otbDBName.Text = oW_SQLCon.tDBName;
                        otbDBUser.Text = oW_SQLCon.tUsername;
                        otbDBPassword.Text = oW_SQLCon.tPassword;

                        if (ptPlantCode == "Prem")
                        {
                            ocmSGRDel.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oC_Log.Error("SETxBranchDialog :" + ex);
            }
        }

        private void otbSGRPlantName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (oW_SQLCon != null)
                {
                    oW_SQLCon.tPlantName = otbSGRPlantName.Text;
                    bW_ConfigChng = true;
                    W_SETxChngNoti();
                }
            }
            catch (Exception ex)
            {
                oC_Log.Error("otbSGRPlantName_TextChanged :" + ex);
            }
        }

        private void ocmSGRNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!olbPlantList.Items.Contains("new"))
                {
                    cCNSP.SP_NEWtCacheConStr("new");
                    olbPlantList.Items.Add("new");
                    bW_ConfigChng = true;
                    W_SETxChngNoti();
                }
            }
            catch (Exception ex)
            {
                oC_Log.Error("ocmSGRNew_Click :" + ex);
            }
        }
        private void ocmSGRDel_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult oDr = MessageBox.Show("Are you sure you want to delete this database config?", "Delete confirmation.", MessageBoxButtons.YesNo);

                if (oDr == DialogResult.Yes)
                {
                    cCNSP.SP_DELtCacheConStr(olbPlantList.SelectedItem.ToString());
                    olbPlantList.Items.RemoveAt(olbPlantList.SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                oC_Log.Error("ocmSGRDel_Click :" + ex);
            }
        }

        private void odgDashBoard_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

            }
            catch { }
        }
    }
}
