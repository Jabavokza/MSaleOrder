using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSaleOrder.ST_Class
{
    public class cCNVB
    {
        public static string tVB_SQLSelSalVatHD =
            "SELECT DISTINCT TOP 5000 FTTmnNum" +
            ", FTShdTransNo" +
            ", FTXihDocType" +
            ", FTXihDocNo " +            
            ", FDShdDepositDueDate AS DLVRY_DATE" +
            ", FTXihCstName" +
            ", FTXihCstAddr1 " +
            ", FTXihCstAddr2 " +
            ", FTXihCstTel AS TEL " +
            ", FTXihCstFax AS FAX " +
            //", (select ST.FTPlantCode from TCNMStoreMtn ST) " +
            ", FTShdPlantCode " +
            ", FTShdPlantCode " +
            "+ FTTmnNum " +
            "+ FTShdTransNo " +
            "+ FTXihDocNo as BELNR " +
            ", FTXihDepositDueTime" +
            ", FTXihRemark " + 
            ", FTXihEquipment " +                 
            ", FTRemark " +
            ", FTStaSentOnOff " +
            ", FTSRVName " +
            ", FNShdDocPrint " +
            "FROM [dbo].[TPSTSalVatHD] WHERE [FTXihDocType] = '17' ";

        public static string tVB_SQLSelHeader =
            "SELECT DISTINCT HD.FTTmnNum" +
            ", HD.FTShdTransNo" +
            ", HD.FTXihDocType" +
            ", HD.FTXihDocNo " +
            //*KT 61-08-08 Add Flag Action
            ", HD.FTXihDCDocStatus " +
            ", HD.FTXihDCStatusDC " +
            ", HD.FTStaSentOnOff " + //3: Update
                                     //",'CREATE' AS ACTION" +
            ", CASE WHEN HD.FTShdStaBigLot = 'Y' THEN 'ZBL' ELSE 'ZCDO' END AS TRANSTYPE" +
            ", HD.FDShdDepositDueDate AS DLVRY_DATE" +
            //    ---TENDER SQL-- -
            ", HD.FTXihCstName AS NAME1" +
            ",'' AS NAME2" +
            ",'' AS NAME3" +
            ",'' AS NAME4" +
            ",'' AS STREET" +
            ",'' AS CITY" +
            ", AD.FTZipCode AS POSTAL_CODE" +
            ",'TH' AS LAND" +
            ", AD.FTCstTelNo AS TEL" +
            ", HD.FTXihCstFax AS FAX" +
            ",'' AS DISTRICT" +
            //--POS Reservation number(Plant+POSTRansactionNO + RFNo.)
            //--*KT 61-08-07 Edit select TOP 1
            //", (SELECT TOP 1 ST.FTPlantCode from TCNMStoreMtn ST) " +
            ",HD.FTShdPlantCode " +
            "+ HD.FTTmnNum " +
            "+ HD.FTShdTransNo " +
            "+ HD.FTXihDocNo as BELNR " +

            //--*KT 61-08-07 Edit  RF Ref follow format BELNR
            // ",'' AS REF_BELNR" +
            ",CASE WHEN ISNULL(HD.FTXihCNRefDocNo,'') <> '' THEN " +
            //" (SELECT TOP 1 ST.FTPlantCode from TCNMStoreMtn ST) " +
            " HD.FTShdPlantCode " +
            "+ HD.FTTmnNum " +
            "+ HD.FTShdTransNo " +
            "+ ISNULL(HD.FTXihCNRefDocNo,'') ELSE '' END as REF_BELNR " +

            //--ZADR - ที่อยู่เต็มรูป-- -
            ",'ZADR' AS TDID_ZADR" +
             //",(SELECT FTXihCstAddr1 " +
             //"FROM TPSTSalVatAdr " +
             //"WHERE FTXihDocNo = HD.FTXihDocNo " +
             //"AND FTMapCode = HD.FTMapCode " +
             //"AND FTCstCode = HD.FTCstCode) AS TDLINE1_ZADR " +
             //",(SELECT FTXihCstAddr2 " +
             //"FROM TPSTSalVatAdr " +
             //"WHERE FTXihDocNo = HD.FTXihDocNo " +
             //"AND FTMapCode = HD.FTMapCode " +
             //"AND FTCstCode = HD.FTCstCode) AS TDLINE2_ZADR " +
             //--*KT 61-08-07 Change TPSTSalVatAdr>>TCNMCstAdrMtn
             // ", AD.FTXihCstAddr1 AS TDLINE1_ZADR" +
             // ", AD.FTXihCstAddr2 AS TDLINE2_ZADR" +
             ", AD.FTCstAddr1Inv AS TDLINE1_ZADR" +
             ", AD.FTCstAddr2Inv AS TDLINE2_ZADR" +
            //--ZDLT - 14 - 18.00-- -
            ",'ZDLT' AS TDID_ZDLT" +
            ", HD.FTXihDepositDueTime AS TDLINE_ZDLT" +
            //--ZREM----
            ",'ZREM' AS TDID_ZREM" +
            ", HD.FTXihRemark AS TDLINE1_ZREM " +       //--หมายเหตุ1--
            ", HD.FTXihEquipment AS TDLINE2_ZREM " +    //--สิ่งที่นำไปด้วย--
                                                        //--ZMAP - FileName--
            ",'ZMAP' AS TDID_ZMAP" +
            ",HD.FTMapName AS TDLINE_ZMAP " +           //--0942223333_1_20180614101523.pdf
                                                        //--Item SQL--
            ",HD.FCShdRnd " +
            ",HD.FTSRVName " +
            ",HD.FTShdPlantCode " +
            ",HD.FNShdDocPrint " +
            " FROM TPSTSalVatHD AS HD " +
            //--*KT 61-08-07 Change TPSTSalVatAdr>>TCNMCstAdrMtn
            // "LEFT JOIN TPSTSalVatAdr AS AD " +
            // "ON AD.FTXihDocNo = HD.FTXihDocNo " +
            // "AND AD.FTMapCode = HD.FTMapCode " +
            // "AND AD.FTCstCode = HD.FTCstCode)" +
            "LEFT JOIN TCNMCstAdrMtn AS AD " +
            "ON AD.FTCstCode = HD.FTCstCode " +
            "AND AD.FTCstMapCode= HD.FTMapCode " +
            //"AND AD.FTStmCode=HD.FTStmCode) " +
            "WHERE HD.FTXihDocType = '17' ";
          //  "AND ISNULL(AD.FNCstAddrID,'')<>''";

        public static string tVB_SQLSelTender =
            "SELECT DISTINCT HD.FTTmnNum" +
            ",HD.FTShdTransNo" +
            ",HD.FTXihDocType" +
            ", CASE WHEN (RC.FTTdmCode = 'T009') THEN 'T030' ELSE RC.FTTdmCode END AS TENDER_TYPE " +
            ", FCSrcNet AS AMOUNT" +
            ",RC.FTSrcBBYNo AS BONUSBUYID " +           
            "FROM [dbo].[TPSTSalVatHD] HD " +
            "INNER JOIN[dbo].[TPSTSalVatRC] RC " +
            "ON  HD.FTTmnNum = RC.FTTmnNum " +
            "AND HD.FTShdTransNo = RC.FTShdTransNo " +
            "AND HD.FTXihDocNo = RC.FTXihDocNo " +
            "WHERE HD.FTXihDocType = '17' ";

        public static string tVB_SQLSelItem =
            "SELECT DISTINCT HD.FTTmnNum" +
            ", HD.FTShdTransNo" +
            ", HD.FTXihDocType" +
            ", DT.FCSdtQtyAll AS QUANTITY" +
            ", DT.FNSdtSeqNo * 10 AS POSEX" +
            ", DT.FTPunCode AS UOM" +
            ", DT.FTSdtDCLocID AS SHIPPING_POINT " +
            ", HD.FTShdPlantCode AS SELLING_PLANT " +
            ", DT.FTSdtDisChgTxt AS SERIAL" +
            ", DT.FNSdtStaRef " +
            //  --PRICING---
            ",'RETAILPRICE' AS TYPE_PRICING " +
            ", DT.FCSdtRegPrice * DT.FCSdtQtyAll AS  AMOUNT_PRICING " +
            ",'THB'	AS CURRENCY_PRICING " +
            ",'+'	AS SIGN_PRICING " +
            ", HD.FTShdBBYNo	AS BONUSBUYID_PRICING " +
            // ---Discount sql  P001,P002...----


            // --VAT---
            ",'VAT' AS TYPE_VAT " +
            ", DT.FCSdtVat  AS  AMOUNT_VAT " +
            ",'+'		    AS SIGN_VAT " +
            ",''			AS BONUSBUYID_VAT " +
            //--SKUCODE--
            ",DT.FTSkuCode  AS EANCODE " +
            "FROM [dbo].[TPSTSalVatHD] HD " +
            "INNER JOIN[dbo].[TPSTSalVatDT] DT " +
            "ON  HD.FTTmnNum = DT.FTTmnNum " +
            "AND HD.FTShdTransNo = DT.FTShdTransNo " +
            "AND HD.FTXihDocNo = DT.FTXihDocNo "+
            "WHERE HD.FTXihDocType = '17' ";

        public static string tVB_SQLSelDiscount =
            "SELECT DISTINCT  CD.FTTmnNum " +
            ", CD.FTShdTransNo " +
            ", CD.FNSdtSeqNo " +
            ", CD.FTScdBBYProfID    AS TYPE " +
            ", CD.FCScdAmt  AS AMOUNT " +
            ",'THB'         AS CURRENCY " +
            ",'-'           AS CD_SIGN " +
            ", CD.FTScdBBYNo AS BONUSBUYID " +
            "FROM [dbo].[TPSTSalCD] CD " +
            "INNER JOIN[dbo].[TPSTSalVatHD] HD " +
            "ON  HD.FTTmnNum = CD.FTTmnNum " +
            "AND HD.FTShdTransNo = CD.FTShdTransNo " +
            "AND HD.FDShdTransDate = CD.FDShdTransDate " +
            "AND HD.FTSRVName = CD.FTSRVName " +
            "WHERE CD.FTTmnNum = {0} " +
            "AND CD.FTShdTransNo = {1} " +
            "AND CD.FNSdtSeqNo = {2} " +
            "AND HD.FTShdStaBigLot <> 'Y' " +

            "UNION SELECT DISTINCT DT.FTTmnNum " +
            ", DT.FTShdTransNo " +
            ", DT.FNSdtSeqNo " +
            ", HD.FTShdBBYProfID " +
            ",(DT.FCSdtRegPrice - DT.FCSdtSalePrice) * DT.FCSdtQty AS AMOUNT " +
            ",'THB'         AS CURRENCY " +
            ",'-'           AS CD_SIGN " +
            ", HD.FTShdBBYNo AS BONUSBUYID " +
            "FROM [dbo].[TPSTSalVatDT] DT " +
            "INNER JOIN[dbo].[TPSTSalVatHD] HD " +
            "ON  HD.FTTmnNum = DT.FTTmnNum " +
            "AND HD.FTShdTransNo = DT.FTShdTransNo " +
            "AND HD.FDShdTransDate = DT.FDShdTransDate " +
            "AND HD.FTXihDocNo = DT.FTXihDocNo " +
            "WHERE DT.FTTmnNum = {0} " +
            "AND DT.FTShdTransNo = {1} " +
            "AND DT.FNSdtSeqNo = {2} " +
            "AND DT.FCsdtQty > 1 " +
            "AND HD.FTShdStaBigLot = 'Y' ";

        public static string tVB_SQLUpdSalHD =
            "UPDATE [dbo].[TPSTSalVatHD] " +
            "SET FTStaSentOnOff = {0} " +
            "WHERE FTTmnNum = {1} " +
            "AND FTShdTransNo = {2} " +
            "AND FTShdPlantCode = {3} " +
            "AND FTXihDocNo = {4} "; 
    }
}
