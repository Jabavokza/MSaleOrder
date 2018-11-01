using log4net;
using MSaleOrder.ST_Class;
using MSaleOrder.X_Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSaleOrder
{
    static class Program
    {
        public static wMSaleOrderMain wC_Main;
        public static cSaleOrderMan oC_SaleOrderMan;
        public static System.Windows.Forms.NotifyIcon oC_MSaleOrderNotifyIco;
        public static System.Windows.Forms.ContextMenu oC_NotiIcoMenu;
        public static System.Windows.Forms.MenuItem oC_MenuExit;
        public static System.Windows.Forms.MenuItem oC_MenuOpen;
        public static System.Windows.Forms.MenuItem oC_MenuHide;
        public static System.Windows.Forms.TextBox oC_LogTextBox;
        public static int nC_ProgPcn;
        private static readonly log4net.ILog oC_Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex mutex = new System.Threading.Mutex(false, "TheMallMSaleOrder");
            try
            {
                if (mutex.WaitOne(0, false))
                {
                    // Run the application
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
                    oC_LogTextBox = new TextBox();
                    C_LOADxConfig();
                    wC_Main = new wMSaleOrderMain();                  
                    var appender = LogManager.GetRepository().GetAppenders().Where(a => a.Name == "TextBoxAppender").FirstOrDefault();

                    if (appender != null)
                    {
                        ((cTextBoxAppender)appender).otbLogTextBox = oC_LogTextBox;
                    }

                    C_INITxNotifyIcon();                   
                    wC_Main.Activate();                   
                    oC_SaleOrderMan = new cSaleOrderMan();
                    oC_SaleOrderMan.C_SETxTimer();
                    wC_Main.W_GETxTimeEnb();
                    wC_Main.Show();
                    Application.Run();
                }
                else
                {
                    MessageBox.Show("An instance of the application is already running.");
                }
            }
            catch (Exception ex)
            {
                oC_Log.Error("Main :" + ex);
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.Close();
                    mutex = null;
                }
            }           
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            oC_MSaleOrderNotifyIco.Dispose();
        }

        public static void C_SETxProg(bool pbProgRunning,int pnTotalRec = 0,int pnSentRec = 0)
        {
            if (pbProgRunning && pnTotalRec > 0)
            {
                nC_ProgPcn = (pnSentRec * 100 ) / pnTotalRec;
            }
            else
            {
                nC_ProgPcn = 0;
            }

            if (!wC_Main.IsDisposed)
            {
                wC_Main.W_SETxProg(pbProgRunning, pnTotalRec, pnSentRec);
            }
        }


        private static void C_LOADxConfig()
        {
            cCNSP.SP_LOADbConfig();
        }

        private static void C_SAVExConfig()
        {
            cCNSP.SP_SAVExConStr();
            cCNSP.SP_SAVExWebAPICon(cCNSP.SP_READtCacheWebAPICon());
            cCNSP.SP_SAVExSchdlConfig(cCNSP.SP_READtCacheScheduleConfig());
        }

        public static void C_INITxNotifyIcon()
        {
            try
            {
                oC_MSaleOrderNotifyIco = new NotifyIcon();
                oC_NotiIcoMenu = new ContextMenu();
                oC_MenuExit = new MenuItem("Close MSaleOrder", new System.EventHandler(C_MenuExit_Click));
                oC_MenuOpen = new MenuItem("Open MSaleOrder", new System.EventHandler(C_MenuOpen_Click));
                oC_MenuHide = new MenuItem("Hide MSaleOrder", new System.EventHandler(C_MenuHide_Click));
                oC_NotiIcoMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { oC_MenuOpen, oC_MenuHide, oC_MenuExit });
                oC_MSaleOrderNotifyIco.Text = "MSaleOrder";
                oC_MSaleOrderNotifyIco.Icon = MSaleOrder.Properties.Resources.connected;
                oC_MSaleOrderNotifyIco.ContextMenu = oC_NotiIcoMenu;
                oC_MSaleOrderNotifyIco.BalloonTipIcon = ToolTipIcon.Info;
                oC_MSaleOrderNotifyIco.BalloonTipTitle = "MSaleOrder Manager";
                oC_MSaleOrderNotifyIco.BalloonTipText = "Left- click to open management Form." + Environment.NewLine + "Right-click on the icon for more options.";
                oC_MSaleOrderNotifyIco.ShowBalloonTip(2000);
                oC_MSaleOrderNotifyIco.Visible = true;
                oC_MSaleOrderNotifyIco.Click += new System.EventHandler(C_NotiIco_Click);
            }
            catch (Exception oEx)
            {
                oC_Log.Error("C_INITxNotifyIcon :" + oEx);
                MessageBox.Show("Program initialize failed. See detail in AppPath~\\LogErr\\ErrLog.log", "MSaleOrder Application can not start.");
                Application.Exit();
            }
        }

        private static void C_MenuOpen_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            if (wC_Main.IsDisposed)
            {
                wC_Main = new wMSaleOrderMain();
            }

            wC_Main.Show();
        }

        private static void C_MenuHide_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            if (wC_Main.IsDisposed)
            {
                wC_Main = new wMSaleOrderMain();
            }

            wC_Main.Hide();
        }

        private static void C_MenuExit_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            oC_MSaleOrderNotifyIco.Dispose();
            Application.Exit();
        }

        private static void C_NotiIco_Click(object Sender, EventArgs e)
        {
            // Show the form when the user double clicks on the notify icon.
            // Set the WindowState to normal if the form is minimized.
            if (wC_Main.IsDisposed)
            {
                wC_Main = new wMSaleOrderMain();
            }

            if (wC_Main.WindowState == FormWindowState.Minimized)
                wC_Main.WindowState = FormWindowState.Normal;

            // Activate the form.
            wC_Main.Show();
        }
    }
}
