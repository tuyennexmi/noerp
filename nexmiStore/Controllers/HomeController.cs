// HomeController.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Data;
using System.Globalization;
using nexmiStore.Models;
using NEXMI;

namespace nexmiStore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(String action)
        {            
            return View();
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult AppInfo()
        {
            return PartialView();
        }

        //public void ExportSalesPreview(String FromDate, String ToDate, String UserID)
        //{
        //    DataTable dt = new DataTable();
        //    DataColumn dc;
        //    dc = new DataColumn();
        //    dc.ColumnName = "STT";
        //    dc.Caption = "STT";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "ProductName";
        //    dc.Caption = "Tên xe";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "TimeUsed";
        //    dc.Caption = "Số giờ sử dụng";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "Amount";
        //    dc.Caption = "Doanh thu";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "RealAmount";
        //    dc.Caption = "Thực thu";
        //    dt.Columns.Add(dc);

        //    DateTime From = NMCommon.convertDate(FromDate);
        //    DateTime To = NMCommon.convertDate(ToDate).AddDays(1);

        //    SPWSI = new NEXMI.NMSearchProductWS.NMSearchProductWSI();
        //    SPWSI.Mode = "SRC_FOR_REPORT";
        //    SPWSI.FromDate = From;
        //    SPWSI.ToDate = To;
        //    SPWSI.OrderTypeId = ((int)NMConstant.OrderType.Service).ToString();
        //    SPWSI.UserId = UserID;
        //    SPWSI = SPWS.callService(SPWSI);

        //    if (SPWSI.WsiError == "")
        //    {
        //        int hour, minute;
        //        double Total1 = 0;
        //        double Total2 = 0;
        //        double Total3 = 0;
        //        for (int i = 0; i < SPWSI.ProductIDList.Length; i++)
        //        {
        //            dt.Rows.Add();
        //            dt.Rows[i]["STT"] = i + 1;
        //            dt.Rows[i]["ProductName"] = "Số " + SPWSI.ProductNameList[i].ToString();
        //            hour = int.Parse(SPWSI.TotalOrderQuantities[i].ToString()) / 60;
        //            minute = int.Parse(SPWSI.TotalOrderQuantities[i].ToString()) % 60;
        //            dt.Rows[i]["TimeUsed"] = String.Format("{0:0,0}", hour) + ":" + String.Format("{0:0,0}", minute);
        //            dt.Rows[i]["Amount"] = double.Parse(SPWSI.TotalOrderSales[i].ToString());
        //            dt.Rows[i]["RealAmount"] = double.Parse(SPWSI.TotalRealSales[i].ToString());
        //            Total1 += int.Parse(SPWSI.TotalOrderQuantities[i].ToString());
        //            Total2 += double.Parse(SPWSI.TotalOrderSales[i].ToString());
        //            Total3 += double.Parse(SPWSI.TotalRealSales[i].ToString());
        //        }
        //        dt.Rows.Add();
        //        dt.Rows[dt.Rows.Count - 1]["STT"] = "";
        //        dt.Rows[dt.Rows.Count - 1]["ProductName"] = "TỔNG CỘNG:";
        //        dt.Rows[dt.Rows.Count - 1]["TimeUsed"] = String.Format("{0:0,0}", Total1 / 60) + ":" + String.Format("{0:0,0}", Total1 % 60);
        //        dt.Rows[dt.Rows.Count - 1]["Amount"] = Total2;
        //        //dt.Rows[dt.Rows.Count - 1]["RealAmount"] = Total3;
        //    }
        //    ExportToExcel2("TheoDoiDoanhThu" + DateTime.Today.ToString("dd-MM") + ".xls", dt);
        //}

        //public void ExportSalesReport(String FromDate, String ToDate, String UserId)
        //{
        //    DataTable dt = new DataTable();
        //    DataColumn dc;
        //    dc = new DataColumn();
        //    dc.ColumnName = "STT";
        //    dc.Caption = "STT";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "ProductName";
        //    dc.Caption = "Tên xe";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "TimeUsed";
        //    dc.Caption = "Số giờ sử dụng";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "Amount";
        //    dc.Caption = "Doanh thu";
        //    dt.Columns.Add(dc);
        //    //dc = new DataColumn();
        //    //dc.ColumnName = "RealAmount";
        //    //dc.Caption = "Thực thu";
        //    //dt.Columns.Add(dc);

        //    DateTime From = NMCommon.convertDate(FromDate);
        //    DateTime To = NMCommon.convertDate(ToDate).AddDays(1);

        //    SPWSI = new NEXMI.NMSearchProductWS.NMSearchProductWSI();
        //    SPWSI.Mode = "SRC_FOR_REPORT";
        //    SPWSI.FromDate = From;
        //    SPWSI.ToDate = To;
        //    SPWSI.OrderTypeId = ((int)NMConstant.OrderType.Service).ToString();
        //    SPWSI.UserId = UserId;
        //    SPWSI = SPWS.callService(SPWSI);

        //    if (SPWSI.WsiError == "")
        //    {
        //        int hour, minute;
        //        double Total1 = 0;
        //        double Total2 = 0;
        //        double Total3 = 0;
        //        for (int i = 0; i < SPWSI.ProductIDList.Length; i++)
        //        {
        //            dt.Rows.Add();
        //            dt.Rows[i]["STT"] = i + 1;
        //            dt.Rows[i]["ProductName"] = "Số " + SPWSI.ProductNameList[i].ToString();
        //            hour = int.Parse(SPWSI.TotalOrderQuantities[i].ToString()) / 60;
        //            minute = int.Parse(SPWSI.TotalOrderQuantities[i].ToString()) % 60;
        //            dt.Rows[i]["TimeUsed"] = String.Format("{0:0,0}", hour) + ":" + String.Format("{0:0,0}", minute);
        //            dt.Rows[i]["Amount"] = double.Parse(SPWSI.TotalOrderSales[i].ToString());
        //            //dt.Rows[i]["RealAmount"] = double.Parse(SPWSI.TotalRealSales[i].ToString());
        //            Total1 += int.Parse(SPWSI.TotalOrderQuantities[i].ToString());
        //            Total2 += double.Parse(SPWSI.TotalOrderSales[i].ToString());
        //            Total3 += double.Parse(SPWSI.TotalRealSales[i].ToString());
        //        }
        //        dt.Rows.Add();
        //        dt.Rows[dt.Rows.Count - 1]["STT"] = "";
        //        dt.Rows[dt.Rows.Count - 1]["ProductName"] = "TỔNG CỘNG:";
        //        dt.Rows[dt.Rows.Count - 1]["TimeUsed"] = String.Format("{0:0,0}", Total1 / 60) + ":" + String.Format("{0:0,0}", Total1 % 60);
        //        dt.Rows[dt.Rows.Count - 1]["Amount"] = Total2;
        //        //dt.Rows[dt.Rows.Count - 1]["RealAmount"] = Total3;
        //    }
        //    ExportToExcel2("BaoCaoDoanhThuTheoXe" + DateTime.Today.ToString("dd-MM") + ".xls", dt);
        //}

        //public void ExportSalesReport2(String FromDate, String ToDate, String UserId)
        //{
        //    DataTable dt = new DataTable();
        //    DataColumn dc;
        //    dc = new DataColumn();
        //    dc.ColumnName = "STT";
        //    dc.Caption = "STT";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "OrderCode";
        //    dc.Caption = "<%= NEXMI.NMCommon.GetInterface("ORDER_NO", Session["Lang"].ToString()) %>";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "CardId";
        //    dc.Caption = "Số thẻ";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "TimeUsed";
        //    dc.Caption = "Số giờ sử dụng";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "TotalAmount";
        //    dc.Caption = "Doanh thu";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "RealAmount";
        //    dc.Caption = "Thực thu";
        //    dt.Columns.Add(dc);

        //    DateTime From = NMCommon.convertDate(FromDate);
        //    DateTime To = NMCommon.convertDate(ToDate).AddDays(1);

        //    SOWSI = new NEXMI.NMSearchOrderWS.NMSearchOrderWSI();
        //    SOWSI.Mode = "SRC_FOR_REPORT";
        //    SOWSI.FromOrderDate = From;
        //    SOWSI.ToOrderDate = To;
        //    SOWSI.OrderTypeId = ((int)NMConstant.OrderType.Service).ToString();
        //    SOWSI.UserId = UserId;
        //    SOWSI = SOWS.callService(SOWSI);

        //    if (SOWSI.WsiError == "")
        //    {
        //        double Total1 = 0;
        //        double Total2 = 0;
        //        double Total3 = 0;
        //        for (int i = 0; i < SOWSI.OrderIdList.Length; i++)
        //        {
        //            dt.Rows.Add();
        //            dt.Rows[i]["STT"] = i + 1;
        //            dt.Rows[i]["OrderCode"] = SOWSI.OrderIdList[i].ToString();
        //            dt.Rows[i]["CardId"] = SOWSI.CardIDList[i].ToString();
        //            dt.Rows[i]["TimeUsed"] = String.Format("{0:0,0}", int.Parse(SOWSI.QuantityList[i].ToString()) / 60) + ":" + String.Format("{0:0,0}", int.Parse(SOWSI.QuantityList[i].ToString()) % 60);
        //            dt.Rows[i]["TotalAmount"] = String.Format("{0:0,0}", double.Parse(SOWSI.TotalAmountList[i].ToString()));
        //            dt.Rows[i]["RealAmount"] = String.Format("{0:0,0}", double.Parse(SOWSI.AmountList[i].ToString()));
        //            Total1 += double.Parse(SOWSI.QuantityList[i].ToString());
        //            Total2 += double.Parse(SOWSI.TotalAmountList[i].ToString());
        //            Total3 += double.Parse(SOWSI.AmountList[i].ToString());
        //        }
        //        dt.Rows.Add();
        //        dt.Rows[dt.Rows.Count - 1]["STT"] = "";
        //        dt.Rows[dt.Rows.Count - 1]["OrderCode"] = "";
        //        dt.Rows[dt.Rows.Count - 1]["CardId"] = "TỔNG CỘNG:";
        //        dt.Rows[dt.Rows.Count - 1]["TimeUsed"] = String.Format("{0:0,0}", Total1 / 60) + ":" + String.Format("{0:0,0}", Total1 % 60);
        //        dt.Rows[dt.Rows.Count - 1]["TotalAmount"] = Total2;
        //        dt.Rows[dt.Rows.Count - 1]["RealAmount"] = Total3;
        //    }
        //    ExportToExcel2("BaoCaoDoanhThuTheoDonHang" + DateTime.Today.ToString("dd-MM") + ".xls", dt);
        //}

        //public void ExportTuitionsReport(String FromDate, String ToDate)
        //{
        //    DataTable dt = new DataTable();
        //    DataColumn dc;
        //    dc = new DataColumn();
        //    dc.ColumnName = "STT";
        //    dc.Caption = "STT";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "Date";
        //    dc.Caption = "Ngày thu";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "StudentName";
        //    dc.Caption = "Tên học viên";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "Course";
        //    dc.Caption = "Khóa học";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "Tuition";
        //    dc.Caption = "Học phí";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "Fee";
        //    dc.Caption = "Lệ phí";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "Practice";
        //    dc.Caption = "Ôn luyện";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "PayCode";
        //    dc.Caption = "Số hóa đơn";
        //    dt.Columns.Add(dc);
        //    dc = new DataColumn();
        //    dc.ColumnName = "Note";
        //    dc.Caption = "<%= NEXMI.NMCommon.GetInterface("NOTE", Session["Lang"].ToString()) %>";
        //    dt.Columns.Add(dc);

        //    DateTime From = NMCommon.convertDate(FromDate);
        //    DateTime To = NMCommon.convertDate(ToDate).AddDays(1);

        //    SPayWSI = new NEXMI.NMSearchPayWS.NMSearchPayWSI();
        //    SPayWSI.Mode = "SRC_FOR_REPORT";
        //    SPayWSI.FromDate = From;
        //    SPayWSI.ToDate = To;
        //    SPayWSI.MasterOrderTypeIdSearch = ((int)NMConstant.OrderType.Course).ToString();
        //    SPayWSI = SPayWS.callService(SPayWSI);

        //    if (SPayWSI.WsiErorr == "")
        //    {
        //        double TotalTuition = 0, TotalFee = 0, TotalPractice = 0;
        //        for (int i = 0; i < SPayWSI.PayCodeList.Length; i++)
        //        {
        //            dt.Rows.Add();
        //            dt.Rows[i]["STT"] = i + 1;
        //            dt.Rows[i]["Date"] = DateTime.Parse(SPayWSI.CreateTimeList[i].ToString()).ToString("dd-MM-yyyy");
        //            dt.Rows[i]["StudentName"] = SPayWSI.CustomerNameList[i].ToString();
        //            dt.Rows[i]["Course"] = SPayWSI.CustomerTypeNameList[i].ToString();
        //            dt.Rows[i]["Tuition"] = double.Parse(SPayWSI.DetailPayAmountList[i].ToString());
        //            TotalTuition += double.Parse(SPayWSI.DetailPayAmountList[i].ToString());
        //            dt.Rows[i]["Fee"] = double.Parse(SPayWSI.DetailRealPayAmountList[i].ToString());
        //            TotalFee += double.Parse(SPayWSI.DetailRealPayAmountList[i].ToString());
        //            dt.Rows[i]["Practice"] = double.Parse(SPayWSI.PracticeAmountList[i].ToString());
        //            TotalPractice += double.Parse(SPayWSI.PracticeAmountList[i].ToString());
        //            dt.Rows[i]["PayCode"] = "'" + SPayWSI.PayCodeList[i].ToString();
        //            dt.Rows[i]["Note"] = SPayWSI.NoteList[i].ToString();
        //        }
        //        dt.Rows.Add();
        //        dt.Rows[dt.Rows.Count - 1]["STT"] = "";
        //        dt.Rows[dt.Rows.Count - 1]["Date"] = "";
        //        dt.Rows[dt.Rows.Count - 1]["StudentName"] = "";
        //        dt.Rows[dt.Rows.Count - 1]["Course"] = "TỔNG CỘNG:";
        //        dt.Rows[dt.Rows.Count - 1]["Tuition"] = TotalTuition;
        //        dt.Rows[dt.Rows.Count - 1]["Fee"] = TotalFee;
        //        dt.Rows[dt.Rows.Count - 1]["Practice"] = TotalPractice;
        //        dt.Rows[dt.Rows.Count - 1]["PayCode"] = "";
        //        dt.Rows[dt.Rows.Count - 1]["Note"] = "";
        //    }
        //    ExportToExcel2("BaoCaoHocPhi" + DateTime.Today.ToString("dd-MM") + ".xls", dt);
        //}

        public void ExportToExcel(string fileName, DataTable dtb)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                //Tạo dòng tiêu để cho bảng tính
                for (int count = 0; count < dtb.Columns.Count; count++)
                {
                    if (dtb.Columns[count].ColumnName != null)
                        sb.Append('"' + dtb.Columns[count].Caption + '"');
                    if (count < dtb.Columns.Count - 1)
                    {
                        sb.Append("\t");
                    }
                }
                //Duyệt từng bản ghi 
                int soDem = 0;
                while (dtb.Rows.Count >= soDem + 1)
                {
                    sb.Append("\n");
                    for (int col = 0; col < dtb.Columns.Count; col++)
                    {
                        if (dtb.Rows[soDem][col] != null)
                            sb.Append('"' + dtb.Rows[soDem][col].ToString() + '"');
                        if (col < dtb.Columns.Count - 1)
                        {
                            sb.Append("\t");
                        }
                    }
                    soDem = soDem + 1;
                }
                using (StreamWriter outfile = new StreamWriter(Server.MapPath(@"~/Content/Documents/" + fileName), false, Encoding.Unicode))
                {
                    outfile.Write(sb);
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            dtb.Dispose();
        }

        public void ExportToExcel2(string fileName, DataTable dtb)
        {
            Response.Charset = "";
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.ContentEncoding = Encoding.Unicode;
            Response.ContentType = "application/ms-excel";
            Response.BinaryWrite(Encoding.Unicode.GetPreamble());
            try
            {
                StringBuilder sb = new StringBuilder();
                //Tạo dòng tiêu để cho bảng tính
                for (int count = 0; count < dtb.Columns.Count; count++)
                {
                    if (dtb.Columns[count].ColumnName != null)
                        sb.Append(dtb.Columns[count].Caption);
                    if (count < dtb.Columns.Count - 1)
                    {
                        sb.Append("\t");
                    }
                }
                Response.Write(sb.ToString() + "\n");
                Response.Flush();
                //Duyệt từng bản ghi 
                int soDem = 0;
                while (dtb.Rows.Count >= soDem + 1)
                {
                    sb = new StringBuilder();

                    for (int col = 0; col < dtb.Columns.Count - 1; col++)
                    {
                        if (dtb.Rows[soDem][col] != null)
                            sb.Append(dtb.Rows[soDem][col].ToString());
                        sb.Append("\t");
                    }
                    if (dtb.Rows[soDem][dtb.Columns.Count - 1] != null)
                        sb.Append(dtb.Rows[soDem][dtb.Columns.Count - 1].ToString());

                    Response.Write(sb.ToString() + "\n");
                    Response.Flush();
                    soDem = soDem + 1;
                }

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            dtb.Dispose();
            Response.End();
        }

        public bool Get(string functionId, string action)
        {
            List<NMPermissionsWSI> Permissions = (List<NEXMI.NMPermissionsWSI>)Session["Permissions"];
            bool isOK = false;

            if (Permissions != null)
            {
                var Permission = Permissions.Select(i => i).Where(i => i.FunctionId == functionId).FirstOrDefault();
                if (Permission != null)
                {
                    if (action == "")
                    {
                        if (Permission.PSelect == "Y" || Permission.PInsert == "Y" || Permission.PUpdate == "Y" || Permission.PDelete == "Y"
                            || Permission.Approval == "Y" || Permission.ViewAll == "Y" || Permission.Calculate == "Y" || Permission.History == "Y")
                        {
                            isOK = true;
                        }
                    }
                    else
                    {
                        switch (action)
                        {
                            case "Select": if (Permission.PSelect == "Y") isOK = true; break;
                            case "Insert": if (Permission.PInsert == "Y") isOK = true; break;
                            case "Update": if (Permission.PUpdate == "Y") isOK = true; break;
                            case "Delete": if (Permission.PDelete == "Y") isOK = true; break;
                            case "Approval": if (Permission.Approval == "Y") isOK = true; break;
                            case "ViewAll": if (Permission.ViewAll == "Y") isOK = true; break;
                            case "Calculate": if (Permission.Calculate == "Y") isOK = true; break;
                            case "History": if (Permission.History == "Y") isOK = true; break;

                            case "ViewPrice": if (Permission.ViewPrice == "Y") isOK = true; break;
                            case "Export": if (Permission.Export == "Y") isOK = true; break;
                            case "PPrint": if (Permission.PPrint == "Y") isOK = true; break;
                            case "Duplicate": if (Permission.Duplicate == "Y") isOK = true; break;
                        }
                    }
                }
            }
            return isOK;
        }

        public bool GetSelect(string functionId)
        {
            return Get(functionId, "Select");
        }

        public bool GetInsert(string functionId)
        {
            return Get(functionId, "Insert");
        }

        public bool GetUpdate(string functionId)
        {
            return Get(functionId, "Update");
        }

        public bool GetDelete(string functionId)
        {
            return Get(functionId, "Delete");
        }

        public bool GetApproval(string functionId)
        {
            return Get(functionId, "Approval");
        }

        public bool GetViewAll(string functionId)
        {
            return Get(functionId, "ViewAll");
        }

        public bool GetCalculate(string functionId)
        {
            return Get(functionId, "Calculate");
        }

        public bool GetHistory(string functionId)
        {
            return Get(functionId, "History");
        }

        public bool GetViewPrice(string functionId)
        {
            return Get(functionId, "ViewPrice");
        }

        public bool GetExport(string functionId)
        {
            return Get(functionId, "Export");
        }

        public bool GetPPrint(string functionId)
        {
            return Get(functionId, "PPrint");
        }

        public bool GetDuplicate(string functionId)
        {
            return Get(functionId, "Duplicate");
        }
    }
}
