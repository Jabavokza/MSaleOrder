using MSaleOrder.ST_Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSaleOrder
{
    public partial class wManualSend : Form
    {
        public static bool bW_Send = false;
        public static bool bW_StaUnsent = false;
        public static bool bW_StaSuccess = false;
        public static bool bW_StaFailed = false;
        public static bool bW_SendAll = false;
        public static List<string> aW_SelPlant = new List<string>();
        public static string tW_SelPlant = "";
        public wManualSend(wMSaleOrderMain pwParent)
        {
            InitializeComponent();
            
            //tW_SelPlant = pwParent.tW_SelPlant;
            try
            {
                W_SETxSelPlant();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            this.ControlBox = false;
        }

        private void ocmSendCancel_Click(object sender, EventArgs e)
        {
            bW_Send = false;
            this.Close();
        }

        public void W_SETxSelPlant()
        {
            ockMnlSndAllPln.Checked = false;
            olbMnlSndPlnList.Items.Clear();
            if (cCNSP.aPlantList.Count > 0)
            {
                olbMnlSndPlnList.Items.AddRange(cCNSP.aPlantList.ToArray());
                for (int nIndex = 0; nIndex < olbMnlSndPlnList.Items.Count; nIndex++)
                {
                    if (olbMnlSndPlnList.Items[nIndex].ToString() != tW_SelPlant)
                    {
                        olbMnlSndPlnList.SetItemCheckState(nIndex, CheckState.Unchecked);
                    }
                    else
                    {
                        olbMnlSndPlnList.SetItemCheckState(nIndex, CheckState.Checked);
                    }
                }
            }
        }

        private void ocmSendOK_Click(object sender, EventArgs e)
        {
            aW_SelPlant.Clear();
            aW_SelPlant.AddRange(olbMnlSndPlnList.CheckedItems.Cast<string>());
            bW_Send = true;
            bW_StaUnsent = ockStaUnsent.Checked;
            bW_StaSuccess = ockStaSuccess.Checked;
            bW_StaFailed = ockStaFailed.Checked;
            bW_SendAll = oraSendAllRow.Checked;
            //  owMain.Invoke(() => owMain.W_PROCxData(ockStaUnsent.Checked, ockStaSuccess.Checked, ockStaFailed.Checked, oraSendSelected.Checked ? "selected" : "all"));
           //Program.wC_Main.Invoke(() => Program.wC_Main.W_PROCxData(ockStaUnsent.Checked, ockStaSuccess.Checked, ockStaFailed.Checked,oraSendSelected.Checked ? "selected" : "all"));
           // Program.wC_Main.W_PROCxData(ockStaUnsent.Checked, ockStaSuccess.Checked, ockStaFailed.Checked, oraSendSelected.Checked);
            this.Close();
        }

        private void oraSendAllRow_CheckedChanged(object sender, EventArgs e)
        {
            ockMnlSndAllPln.Enabled = olbMnlSndPlnList.Enabled = oraSendAllRow.Checked;
            if (!oraSendAllRow.Checked)
            {
                
                ockMnlSndAllPln.Checked = false;
                for (int nIndex = 0; nIndex < olbMnlSndPlnList.Items.Count; nIndex++)
                {
                    if (olbMnlSndPlnList.Items[nIndex].ToString() != tW_SelPlant)
                    {
                        olbMnlSndPlnList.SetItemCheckState(nIndex, CheckState.Unchecked);
                    }
                    else
                    {
                        olbMnlSndPlnList.SetItemCheckState(nIndex, CheckState.Checked);
                    }
                }
                
            }
        }

        private void ockMnlSndAllPln_CheckedChanged(object sender, EventArgs e)
        {
            ockMnlSndAllPln.CheckedChanged -= ockMnlSndAllPln_CheckedChanged;
            olbMnlSndPlnList.ItemCheck -= olbMnlSndPlnList_ItemCheck;
            if (ockMnlSndAllPln.Checked)
            {
                for (int nIndex = 0; nIndex < olbMnlSndPlnList.Items.Count; nIndex++)
                {
                    olbMnlSndPlnList.SetItemCheckState(nIndex, CheckState.Checked);
                }
            }
            else
            {
                for (int nIndex = 0; nIndex < olbMnlSndPlnList.Items.Count; nIndex++)
                {
                    if (olbMnlSndPlnList.Items[nIndex].ToString() != tW_SelPlant)
                    {
                        olbMnlSndPlnList.SetItemCheckState(nIndex, CheckState.Unchecked);
                    }
                    else
                    {
                        olbMnlSndPlnList.SetItemCheckState(nIndex, CheckState.Checked);
                    }
                }
            }
            ockMnlSndAllPln.CheckedChanged += ockMnlSndAllPln_CheckedChanged;
            olbMnlSndPlnList.ItemCheck += olbMnlSndPlnList_ItemCheck;
        }
        private void olbMnlSndPlnList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckedListBox oCLb = (CheckedListBox)sender;
            // Switch off event handler
            oCLb.ItemCheck -= olbMnlSndPlnList_ItemCheck;
            ockMnlSndAllPln.CheckedChanged -= ockMnlSndAllPln_CheckedChanged;
            oCLb.SetItemCheckState(e.Index, e.NewValue);
            // Switch on event handler
            
            ockMnlSndAllPln.Checked = (oCLb.CheckedItems.Count == oCLb.Items.Count);
            oCLb.ItemCheck += olbMnlSndPlnList_ItemCheck;
            ockMnlSndAllPln.CheckedChanged += ockMnlSndAllPln_CheckedChanged;
            if (oCLb.CheckedItems.Count == 0)
            {
                oCLb.SetItemCheckState(e.Index, CheckState.Checked);
            }
            
        }
    }
}
