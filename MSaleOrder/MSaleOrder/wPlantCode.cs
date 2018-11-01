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
    public partial class wPlantCode : Form
    {
        private string tW_PlantCode;
        private wMSaleOrderMain wW_Parent;
       
        public wPlantCode(wMSaleOrderMain pwParent, string ptRefName)
        {
            InitializeComponent();
            wW_Parent = pwParent;
            tW_PlantCode = ptRefName;
            otbPCCngPlntCd.Text = tW_PlantCode;
        }

        private void ocmPCCngCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ocmPCCngOK_Click(object sender, EventArgs e)
        {
            if(otbPCCngPlntCd.Text == tW_PlantCode)
            {
                this.Close();
            }
            else if (cCNSP.aPlantList.Contains(otbPCCngPlntCd.Text))
            {
                olaPCCngMssg.Text = "Duplicate name.";
            }
            else
            {
                wW_Parent.tW_CngPlntCd = otbPCCngPlntCd.Text;
                this.Close();
            }
        }

        private void wPlantCode_Load(object sender, EventArgs e)
        {

        }
    }
}
