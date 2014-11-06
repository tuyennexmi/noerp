// ReportController.cs

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
using NEXMI;

namespace nexmiStore.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Reports/

        public ActionResult SalesByTime()
        {
            return PartialView();
        }

        public ActionResult SalesByHoursOfDayReport(String fromDate, String toDate)
        {
            if (string.IsNullOrEmpty(fromDate))
            {
                fromDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-1";
            }

            if (string.IsNullOrEmpty(toDate))
            {
                toDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("CompanyNameInVietnamese");
            dt.Columns.Add("soluong");
            dt.Columns.Add("tong");

            NMCustomersBL cBL = new NMCustomersBL();
            NMCustomersWSI cWSI = new NMCustomersWSI();
            cWSI.Mode = "SRC_OBJ";
            List<NEXMI.NMCustomersWSI> Customers = cBL.callListBL(cWSI);
            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            List<NMMonthlyGeneralJournalsWSI> list;
            WSI.Mode = "SRC_OBJ";
            WSI.MGJ.AccountId = "131";
            WSI.Filter.FromDate = fromDate;
            WSI.Filter.ToDate = toDate;
            System.Data.DataRow row;
            foreach (NMCustomersWSI c in Customers)
            {
                WSI.MGJ.PartnerId = c.Customer.CustomerId;
                list = BL.callListBL(WSI);
                if (list.Count > 0)
                {
                    row = dt.NewRow();
                    row["CompanyNameInVietnamese"] = c.Customer.CompanyNameInVietnamese;
                    row["soluong"] = list.Count(i => i.MGJ.IsBegin == false & i.MGJ.DebitAmount != 0);
                    double t = list.Where(i => i.MGJ.DebitAmount != 0).Sum(i => i.MGJ.DebitAmount);
                    if (t > 0)
                    {
                        row["tong"] = t;
                        dt.Rows.Add(row);
                    }
                }
            }

            ViewData["dt"] = dt;
            
            return PartialView();
        }

        public ActionResult SalesByDayOfMonthReport(String thang, String nam, String width = "1090px", String height = "400px", String IsTable = "")
        {
            ViewData["width"] = width;
            ViewData["height"] = height;
            ViewData["IsTable"] = IsTable;

            if (thang == null)
            {
                thang = DateTime.Now.Month.ToString();
            }
            if (nam == null)
            {
                nam = DateTime.Now.Year.ToString();
            }

            ArrayList cols = new ArrayList();
            cols.Add("ngay");
            cols.Add("thang");
            cols.Add("nam");
            cols.Add("soluong");
            cols.Add("tong");
            NMDataTableWSI WSI = new NMDataTableWSI();
            NMDataTableBL BL = new NMDataTableBL();
            WSI.QueryString = "select DAY(InvoiceDate) as ngay,MONTH(InvoiceDate) as thang,YEAR(InvoiceDate) as nam, COUNT(InvoiceId) as dem, SUM(TotalAmount) as tong from SalesInvoices where MONTH(InvoiceDate)= '" + thang + "' and YEAR(InvoiceDate) = '" + nam + "' group by DAY(InvoiceDate),MONTH(InvoiceDate),YEAR(InvoiceDate) order by DAY(InvoiceDate) DESC";
            WSI.Columns = cols;
            WSI = BL.GetData(WSI);
            System.Data.DataTable dt = WSI.Data;
            ViewData["dt"] = dt;

            return PartialView();
        }

        public ActionResult SalesByMonthOfYearReport(String nam)
        {
            if (nam == null)
            {
                nam = DateTime.Now.Year.ToString();
            }
            ArrayList cols = new ArrayList();
            cols.Add("thang");
            cols.Add("nam");
            cols.Add("soluong");
            cols.Add("tong");
            NMDataTableWSI WSI = new NMDataTableWSI();
            NMDataTableBL BL = new NMDataTableBL();
            WSI.QueryString = "select MONTH(InvoiceDate) thang,YEAR(InvoiceDate) nam, COUNT(InvoiceId) dem, SUM(TotalAmount) from SalesInvoices where  YEAR(InvoiceDate) = '" + nam + "' group by MONTH(InvoiceDate), YEAR(InvoiceDate) order by MONTH(InvoiceDate) DESC";
            WSI.Columns = cols;
            WSI = BL.GetData(WSI);
            System.Data.DataTable dt = WSI.Data;
            ViewData["dt"] = dt;

            return PartialView();
        }

        public ActionResult SalesTopProducts()
        {
            return PartialView();
        }

        public ActionResult SalesTopProductsUC(String top, String fromDate, String toDate, String sort)
        {
            if (string.IsNullOrEmpty(fromDate))
            {
                fromDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-1";
            }

            if (string.IsNullOrEmpty(toDate))
            {
                toDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            }

            //ArrayList cols = new ArrayList();
            //cols.Add("ProductId");
            //cols.Add("ProductNameInVietnamese");
            //cols.Add("soluong");
            //cols.Add("tong");
            //NMDataTableWSI WSI = new NMDataTableWSI();
            //NMDataTableBL BL = new NMDataTableBL();
            //WSI.QueryString = "select top(" + top + ") d.ProductId, p.ProductNameInVietnamese,COUNT(d.ProductId) soluong,SUM(d.TotalAmount) tong from SalesInvoiceDetails d LEFT JOIN Products p on d.ProductId = p.ProductId where d.CreatedDate between '" + fromdate + " " + fromtime + ".000' and '" + todate + " " + totime + ".999' group by d.ProductId, p.ProductNameInVietnamese order by " + sort + " DESC";
            //WSI.Columns = cols;
            //WSI = BL.GetData(WSI);
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("ProductId");
            dt.Columns.Add("ProductNameInVietnamese");
            dt.Columns.Add("soluong");
            dt.Columns.Add("tong");
            NMProductsBL pBL = new NMProductsBL();
            NMProductsWSI pwsi = new NMProductsWSI();
            pwsi.Mode = "SRC_OBJ";
            List<NMProductsWSI> pList = pBL.callListBL(pwsi);

            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            List<NMMonthlyGeneralJournalsWSI> list;
            WSI.Mode = "SRC_OBJ";
            WSI.MGJ.AccountId = "5111";
            WSI.Filter.FromDate = fromDate;
            WSI.Filter.ToDate = toDate;
            //WSI.Filter.GroupBy = "ProductId";
            list = BL.callListBL(WSI);
            list.GroupBy(i => i.MGJ.ProductId);

            System.Data.DataRow row;
            List<NMMonthlyGeneralJournalsWSI>  p;
            foreach (NMProductsWSI c in pList)
            {
                p = list.Where(i => i.MGJ.ProductId == c.Product.ProductId).ToList();
                row = dt.NewRow();
                row["ProductId"] = c.Product.ProductId;
                row["ProductNameInVietnamese"] = c.Translation.Name;
                row["soluong"] = p.Sum(i=>i.MGJ.ExportQuantity);

                row["tong"] = p.Sum(i => i.MGJ.CreditAmount);
                dt.Rows.Add(row);
            }
            ViewData["dt"] = dt;

            return PartialView();
        }

        public ActionResult SalesBySaler()
        {
            return PartialView();
        }
        
        public ActionResult SalesBySalerUC(String fromDate, String toDate, String user)
        {   
            if (user == null || user == "")
            {
                NMCustomersWSI CWSI = new NMCustomersWSI();
                NMCustomersBL CBL = new NMCustomersBL();
                CWSI.Mode = "SRC_OBJ";
                CWSI.Customer = new Customers();
                CWSI.Customer.GroupId = "G004";
                List<NMCustomersWSI> list = CBL.callListBL(CWSI);
                ViewData["User"] = list;
                
                user = list[0].Customer.CustomerId;
            }
            
            ArrayList cols = new ArrayList();
            cols.Add("thang");
            cols.Add("nam");
            cols.Add("soluong");
            cols.Add("tong");
            NMDataTableWSI WSI = new NMDataTableWSI();
            NMDataTableBL BL = new NMDataTableBL();
            WSI.QueryString = "select MONTH(InvoiceDate) thang,YEAR(InvoiceDate) nam, COUNT(InvoiceId) dem, SUM(TotalAmount) from SalesInvoices where SalesPersonId = '" + user + "' and InvoiceDate BETWEEN '" + fromDate + "' and '" + toDate + "'  group by MONTH(InvoiceDate), YEAR(InvoiceDate) order by MONTH(InvoiceDate) DESC";
            WSI.Columns = cols;
            WSI = BL.GetData(WSI);
            System.Data.DataTable dt = WSI.Data;
            ViewData["dt"] = dt;
            ViewData["frommonth"] = DateTime.Parse(fromDate).Month;
            ViewData["fromyear"] = DateTime.Parse(fromDate).Year;
            ViewData["tomonth"] = DateTime.Parse(toDate).Month;
            ViewData["toyear"] = DateTime.Parse(toDate).Year;
            ViewData["fromDate"] = fromDate;
            ViewData["toDate"] = toDate;
            ViewData["selected"] = user;
            return PartialView();
        }

        public ActionResult SalesByAllStaff()
        {
            return PartialView();
        }

        public ActionResult SalesByAllStaffUC(String fromDate, String toDate)
        {   
            ArrayList cols = new ArrayList();
            cols.Add("CompanyNameInVietnamese");
            cols.Add("soluong");
            cols.Add("tong");
            NMDataTableWSI WSI = new NMDataTableWSI();
            NMDataTableBL BL = new NMDataTableBL();
            WSI.QueryString = "select c.CompanyNameInVietnamese, COUNT(countinv) soluong, ISNULL(SUM(suminv),0) tong from Customers c LEFT JOIN ( select s.CreatedBy, s.CreatedDate, COUNT(s.InvoiceId), SUM(s.TotalAmount) from SalesInvoices s where s.CreatedDate between '" + fromDate+ "' and '" + toDate + "' group by s.CreatedBy,s.CreatedDate) temp(createby, createdate, countinv, suminv) on c.CustomerId = createby where c.GroupId='G004' group by c.CompanyNameInVietnamese";
            WSI.Columns = cols;
            WSI = BL.GetData(WSI);
            System.Data.DataTable dt = WSI.Data;
            ViewData["dt"] = dt;

            return PartialView();
        }

        // doanh số theo khách hàng
        public ActionResult SalesByCustomer()
        {
            return PartialView();
        }

        public ActionResult SalesByCustomerUC(String area, String fromDate, String toDate)
        {            
            if (string.IsNullOrEmpty(fromDate))
            {
                fromDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-1";
            }
            
            if (string.IsNullOrEmpty(toDate))
            {
                toDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            }
            //ArrayList cols = new ArrayList();
            //cols.Add("CompanyNameInVietnamese");
            //cols.Add("soluong");
            //cols.Add("tong");
            //NMDataTableWSI WSI = new NMDataTableWSI();
            //NMDataTableBL BL = new NMDataTableBL();
            //WSI.QueryString = "select c.CompanyNameInVietnamese, COUNT(countinv) soluong, ISNULL(SUM(suminv),0) tong from Customers c ";
            //WSI.QueryString += " LEFT JOIN ( select s.CustomerId, s.CreatedDate, COUNT(s.InvoiceId), SUM(s.TotalAmount) from SalesInvoices s ";
            //WSI.QueryString += " where s.StockId = '" + stockid + "' and ";
            //WSI.QueryString += " s.CreatedDate between '" + fromdate + " " + fromtime + ".000' and '" + todate + " " + totime + ".999' group by s.CustomerId, s.CreatedDate) temp(createby, createdate, countinv, suminv) on c.CustomerId = createby where c.GroupId='G001' group by c.CompanyNameInVietnamese";
            //WSI.Columns = cols;
            //WSI = BL.GetData(WSI);
            //System.Data.DataTable dt = WSI.Data;
            
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("CompanyNameInVietnamese");
            dt.Columns.Add("soluong");
            dt.Columns.Add("tong");

            NMCustomersBL cBL = new NMCustomersBL();
            NMCustomersWSI cWSI = new NMCustomersWSI();
            cWSI.Mode = "SRC_OBJ";
            if (!string.IsNullOrEmpty(area))
            {
                cWSI.Customer.AreaId = area;
                //ViewData["cbbAreas"] = area;
                //ViewData["cbbAreaName"] = cWSI.AreaWSI.FullName;
            }
            List<NEXMI.NMCustomersWSI> Customers = cBL.callListBL(cWSI);
            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            List<NMMonthlyGeneralJournalsWSI> list;
            WSI.Mode = "SRC_OBJ";
            WSI.MGJ.AccountId = "131";
            WSI.MGJ.IsBegin = false;
            WSI.Filter.FromDate = fromDate;
            WSI.Filter.ToDate = toDate;
            System.Data.DataRow row;
            foreach (NMCustomersWSI c in Customers)
            {                
                WSI.MGJ.PartnerId = c.Customer.CustomerId;
                list = BL.callListBL(WSI);
                if (list.Count > 0)
                {
                    row = dt.NewRow();
                    row["CompanyNameInVietnamese"] = c.Customer.CompanyNameInVietnamese + " [" + c.Customer.Code + "]";
                    row["soluong"] = list.Count(i=>i.MGJ.IsBegin == false & i.MGJ.DebitAmount != 0);
                    double t = list.Where(i =>i.MGJ.IsBegin == false & i.MGJ.DebitAmount != 0).Sum(i => i.MGJ.DebitAmount);
                    if (t > 0)
                    {
                        row["tong"] = t;
                        dt.Rows.Add(row);
                    }
                }
            }

            ViewData["dt"] = dt;

            return View();
        }

        // sản lượng theo khách hàng
        public ActionResult QuantityByCustomer()
        {
            return PartialView();
        }

        public ActionResult QuantityByCustomerUC(String customerId, String fromDate, String toDate)
        {
            if (string.IsNullOrEmpty(fromDate))
            {
                fromDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-1";
            }
            if (string.IsNullOrEmpty(toDate))
            {
                toDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            }

            NMProductsBL pBL = new NMProductsBL();
            NMProductsWSI pwsi = new NMProductsWSI();
            pwsi.Mode = "SRC_OBJ";
            List<NMProductsWSI> pList = pBL.callListBL(pwsi);

            //ArrayList cols = new ArrayList();            
            //cols.Add("id");
            //cols.Add("tong");
            //cols.Add("soluong");
            //NMDataTableWSI WSI = new NMDataTableWSI();
            //NMDataTableBL BL = new NMDataTableBL();

            //WSI.QueryString = "SELECT sd.ProductId as id, sum(sd.Amount) as tong, sum(sd.Quantity) as soluong";
            //WSI.QueryString += " FROM SalesInvoices si INNER JOIN  SalesInvoiceDetails sd ON si.InvoiceId = sd.InvoiceId ";
            //WSI.QueryString += " WHERE (si.CustomerId = '" + customerId + "') and si.InvoiceDate between '" + fromdate + "' and '" + todate + "'" + " group by sd.ProductId";
            
            //WSI.Columns = cols;
            //WSI = BL.GetData(WSI);
            //System.Data.DataTable dt = WSI.Data;
            //ViewData["dt"] = dt;
            ViewData["products"] = pList;

            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("soluong");
            dt.Columns.Add("tong");

            NMMonthlyGeneralJournalsBL BL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI WSI = new NMMonthlyGeneralJournalsWSI();
            List<NMMonthlyGeneralJournalsWSI> list;
            WSI.Mode = "SRC_OBJ";
            WSI.MGJ.AccountId = "5111";
            WSI.Filter.FromDate = fromDate;
            WSI.Filter.ToDate = toDate;
            WSI.MGJ.PartnerId = customerId;
            System.Data.DataRow row;
            foreach (NMProductsWSI c in pList)
            {
                WSI.MGJ.ProductId = c.Product.ProductId;
                list = BL.callListBL(WSI);
                if (list.Count > 0)
                {
                    row = dt.NewRow();
                    row["id"] = c.Product.ProductId;
                    row["soluong"] = list.Count(i => i.MGJ.IsBegin == false & i.MGJ.ExportQuantity != 0);
                    double t = list.Where(i => i.MGJ.ExportQuantity != 0).Sum(i => i.MGJ.ExportQuantity);
                    if (t > 0)
                    {
                        row["tong"] = t;
                        dt.Rows.Add(row);
                    }
                }
            }

            ViewData["dt"] = dt;

            return PartialView();
        }

        public ActionResult SalesByStock()
        {
            return PartialView();
        }

        public ActionResult SalesByStockUC(string fromDate, string toDate, string stockId)
        {
            if (String.IsNullOrEmpty(fromDate))
                fromDate = DateTime.Today.ToString();
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Invoice = new SalesInvoices();
            WSI.FromDate = fromDate;
            WSI.ToDate = toDate;
            WSI.Invoice.StockId = stockId;
            ViewData["WSIs"] = BL.callListBL(WSI);
            return PartialView();
        }

        public ActionResult InventoryAlert()
        {
            NMStocksWSI SWSI = new NMStocksWSI();
            NMStocksBL SBL = new NMStocksBL();
            SWSI.Mode = "SRC_OBJ";
            ViewData["Stock"] = SBL.callListBL(SWSI);
            return PartialView();
        }

        public ActionResult InventoryAlertUC(String stockId, String categoryId, String top)
        {
            ArrayList cols = new ArrayList();
            cols.Add("ProductId");
            cols.Add("CategoryId");
            cols.Add("StockId");
            cols.Add("soluong");
            cols.Add("gia");
            cols.Add("donvi");
            cols.Add("trangthai");
            NMDataTableWSI WSI = new NMDataTableWSI();
            NMDataTableBL BL = new NMDataTableBL();
            if (top == null || top == "")
            {
                top = "10";
            }
            string strTemp = "";
            if (!string.IsNullOrEmpty(stockId))
                strTemp += " and s.Id='" + stockId + "'";
            if (!string.IsNullOrEmpty(categoryId))
                strTemp += " and c.Id = '" + categoryId + "'";
            WSI.QueryString = "select top(" + top + ") p.ProductId, c.Id CategoryId, s.Id StockId, i.BeginQuantity+i.ImportQuantity-i.ExportQuantity as GoodProductInStock, f.Price, u.Name donvi, p.Discontinued from ProductInStocks i, Products p, Categories c, Stocks s, PricesForSalesInvoice f, ProductUnits u where i.ProductId=p.ProductId and i.StockId=s.Id and p.CategoryId=c.Id and f.Id = (select top 1 Id from PricesForSalesInvoice where ProductId = p.ProductId order by DateOfPrice desc) and p.ProductUnit = u.Id" + strTemp + " order by GoodProductInStock";
            WSI.Columns = cols;
            WSI = BL.GetData(WSI);
            System.Data.DataTable dt = WSI.Data;
            ViewData["dt"] = dt;
            return PartialView();
        }

        public ActionResult SalesByAllStock()
        {
            return PartialView();
        }

        public ActionResult SalesByAllStockUC(string fromDate, string toDate)
        {   
            ArrayList cols = new ArrayList();
            cols.Add("Id");
            cols.Add("soluong");
            cols.Add("tong");
            NMDataTableWSI WSI = new NMDataTableWSI();
            NMDataTableBL BL = new NMDataTableBL();
            //WSI.QueryString = "select top(15) c.CompanyNameInVietnamese, COUNT(countinv) soluong, ISNULL(SUM(suminv),0) tong from Customers c LEFT JOIN ( select s.CreatedBy, s.CreatedDate, COUNT(s.InvoiceId), SUM(s.TotalAmount) from SalesInvoices s where s.CreatedDate between '" + fromdate + " " + fromtime + ".000' and '" + todate + " " + totime + ".999' group by s.CreatedBy,s.CreatedDate) temp(createby, createdate, countinv, suminv) on c.CustomerId = createby where c.GroupId='G004' group by c.CompanyNameInVietnamese";
            //WSI.QueryString = "select s.Name, COUNT(countinv) soluong , ISNULL(SUM(suminv),0) tong from Customers c RIGHT JOIN Stocks s on s.Id = c.StockId LEFT JOIN SalesInvoices i on i.CreatedBy = c.CustomerId LEFT JOIN ( select s.CreatedBy, s.CreatedDate, COUNT(s.InvoiceId), SUM(s.TotalAmount) from SalesInvoices s where s.CreatedDate between '" + fromyear + "-" + frommonth + "-" + fromday + " 00:00:00.000' and '" + toyear + "-" + tomonth + "-" + today + " 23:59:59.999' group by s.CreatedBy,s.CreatedDate) temp(createby, createdate, countinv, suminv) on c.CustomerId = createby group by s.Name";
            //WSI.QueryString = "select st.Name Name, " +
            //                   " (select COUNT(si.InvoiceId) from SalesInvoices si where si.CreatedBy in (select c.CustomerId from Customers c where c.StockId = st.Id) and si.InvoiceDate >= '" + from.ToShortDateString() + "' and si.InvoiceDate <= '" + to.ToShortDateString() + "') soluong," +
            //                   " (select ISNULL(SUM(si.TotalAmount),0) from SalesInvoices si where si.CreatedBy in (select c.CustomerId from Customers c where c.StockId = st.Id) and si.InvoiceDate >= '" + from.ToShortDateString() + "' and si.InvoiceDate <= '" + to.ToShortDateString() + "') tong from Stocks st";
            WSI.QueryString = "select st.Id, " +
                              " (select COUNT(si.InvoiceId) from SalesInvoices si where si.StockId = st.Id and si.InvoiceDate >= '" + fromDate + "' and si.InvoiceDate <= '" + toDate + "') soluong, " +
                              " (select ISNULL(SUM(si.TotalAmount),0) from SalesInvoices si where si.StockId = st.Id and si.InvoiceDate >= '" + fromDate + "' and si.InvoiceDate <= '" + toDate + "') tong from Stocks st";

            WSI.Columns = cols;
            WSI = BL.GetData(WSI);
            System.Data.DataTable dt = WSI.Data;
            ViewData["dt"] = dt;
            return View();
        }

        public ActionResult InventoryReport()
        {
            return PartialView();
        }

        public ActionResult InventoryReportUC(string categoryId)
        {
            NMStocksWSI swsi = new NMStocksWSI();
            NMStocksBL sbl = new NMStocksBL();
            swsi.Mode = "SRC_OBJ";
            List<NMStocksWSI> stock = sbl.callListBL(swsi);
            ViewData["stock"] = stock;

            if (!String.IsNullOrEmpty(categoryId))
            {
                NMProductsWSI pwsi = new NMProductsWSI();
                NMProductsBL pbl = new NMProductsBL();
                pwsi.Product = new Products();
                pwsi.Mode = "SRC_OBJ";
                pwsi.Product.CategoryId = categoryId;
                List<NMProductsWSI> product = pbl.callListBL(pwsi);
                ViewData["product"] = product;

                foreach (NMProductsWSI itp in product)
                {
                    NMProductInStocksBL ibl = new NMProductInStocksBL();
                    NMProductInStocksWSI iwsi = new NMProductInStocksWSI();
                    iwsi.Mode = "SRC_OBJ";
                    iwsi.PIS = new ProductInStocks();
                    iwsi.PIS.ProductId = itp.Product.ProductId;
                    ViewData[itp.Product.ProductId] = ibl.callListBL(iwsi);
                }
            }
            return PartialView();
        }

        public ActionResult InventoryByProductReport()
        {
            NMProductsWSI pwsi = new NMProductsWSI();
            NMProductsBL pbl = new NMProductsBL();
            pwsi.Product = new Products();
            pwsi.Mode = "SRC_OBJ";
            List<NMProductsWSI> product = pbl.callListBL(pwsi);

            ViewData["WSIs"] = new SelectList(product.Select(i => i.Product), "ProductId", "ProductNameInVietnamese");
            

            return PartialView();
        }

        public ActionResult InventoryByProductReportUC(string productId, string fromDate, string toDate)
        {
            NMStocksWSI swsi = new NMStocksWSI();
            NMStocksBL sbl = new NMStocksBL();
            swsi.Mode = "SRC_OBJ";
            List<NMStocksWSI> stock = sbl.callListBL(swsi);
            ViewData["stock"] = stock;

            if (!String.IsNullOrEmpty(productId))
            {
                NMProductInStocksBL ibl = new NMProductInStocksBL();
                NMProductInStocksWSI iwsi = new NMProductInStocksWSI();
                iwsi.Mode = "SRC_OBJ";
                iwsi.PIS.ProductId = productId;
                iwsi.Filter.FromDate = fromDate;
                iwsi.Filter.ToDate = toDate;
                ViewData["PIS"] = ibl.callListBL(iwsi);
            }

            return PartialView();
        }

        public ActionResult SalesByProduct()
        {
            return PartialView();
        }

        public ActionResult SalesByProductUC(string stockId, string categoryId, string fromDate, string toDate)
        {
            string strTemp = "";
            if (!string.IsNullOrEmpty(categoryId))
                strTemp += "and C.Id = '" + categoryId + "' ";
            if (!string.IsNullOrEmpty(stockId))
                strTemp += "and MSI.StockId = '" + stockId + "' ";
            try
            {
                DateTime from = DateTime.Parse(fromDate);
                DateTime to;
                try
                {
                    to = DateTime.Parse(toDate).AddDays(1);
                }
                catch
                {
                    to = from.AddDays(1);
                }
                strTemp += "and MSI.CreatedDate >= '" + from + "' and MSI.CreatedDate <= '" + to + "' ";
            }
            catch { }
            ArrayList cols = new ArrayList();
            cols.Add("ProductName");
            cols.Add("Quantity");
            cols.Add("DiscountAmount");
            cols.Add("TaxAmount");
            cols.Add("Amount");
            cols.Add("TotalAmount");
            cols.Add("CategoryName");
            cols.Add("StockName");
            NMDataTableBL BL = new NMDataTableBL();
            NMDataTableWSI WSI = new NMDataTableWSI();
            WSI.QueryString = "select (select Name from Translations where OwnerId = P.ProductId and LanguageId = 'vi') as ProductName, SUM(DSI.Quantity) as Quantity, SUM(DSI.DiscountAmount) as DiscountAmount, SUM(DSI.TaxAmount) as TaxAmount, SUM(DSI.Amount) as Amount, SUM(DSI.TotalAmount) as TotalAmount, (select Name from Translations where OwnerId = C.Id and LanguageId = 'vi') as CategoryName, (select Name from Translations where OwnerId = MSI.StockId and LanguageId = 'vi') as StockName "
            + "from SalesInvoices MSI, SalesInvoiceDetails DSI, Products P, Categories C where MSI.InvoiceId = DSI.InvoiceId and DSI.ProductId = P.ProductId and C.Id = P.CategoryId "
            + strTemp + "group by P.ProductId, C.Id, MSI.StockId";
            WSI.Columns = cols;
            WSI = BL.GetData(WSI);
            System.Data.DataTable dt = WSI.Data;
            ViewData["dt"] = dt;
            return PartialView();
        }

        public ActionResult IEIReport()
        {
            return PartialView();
        }

        public ActionResult IEIReportByShifts(string stockId, string categoryId, string fromDate, string toDate)
        {
            //NMCloseMonthInventoryControlBL BL = new NMCloseMonthInventoryControlBL();
            //NMCloseMonthInventoryControlWSI WSI = new NMCloseMonthInventoryControlWSI();
            //WSI.Mode = "SRC_OBJ";
            //WSI.StockId = stockId;
            ////WSI.CategoryId = categoryId;
            //WSI.Filter.FromDate = fromDate;
            //WSI.Filter.ToDate = toDate;
            //ViewData["WSIs"] = BL.callListBL(WSI);

            return PartialView();
        }

        public ActionResult IEIReportByProducts(string stockId, string categoryId, string fromDate, string toDate, string pageNum)
        {
            NMProductInStocksBL BL = new NMProductInStocksBL();
            NMProductInStocksWSI WSI = new NMProductInStocksWSI();
            WSI.Mode = "SRC_OBJ";
            
            WSI.PIS = new ProductInStocks();
            WSI.PIS.StockId = stockId;
            WSI.CategoryId = categoryId;
            try
            {
                WSI.PageNum = int.Parse(pageNum);
            }
            catch
            {
                WSI.PageNum = 0;
            }
            WSI.PageSize = NMCommon.PageSize();
            
            List<NMProductInStocksWSI> WSIs = BL.callListBL(WSI);
            ViewData["WSIs"] = WSIs;
            
            return PartialView();
        }
    }
}
