// PlanningController.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NEXMI;

namespace nexmiStore.Controllers
{
    public class PlanningController : Controller
    {
        //
        // GET: /Planning/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Plans()
        {
            NMMasterPlanningsBL BL = new NMMasterPlanningsBL();
            NMMasterPlanningsWSI WSI = new NMMasterPlanningsWSI();
            WSI.Mode = "SRC_OBJ";
            ViewData["Plans"] = BL.callListBL(WSI);                

            return PartialView();
        }

        public ActionResult PlanningByProducts(string id, string mode)
        {
            NMMasterPlanningsBL BL = new NMMasterPlanningsBL();
            NMMasterPlanningsWSI WSI = new NMMasterPlanningsWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Planning.Id = id;
            ViewData["Plan"] = BL.callSingleBL(WSI);

            if (string.IsNullOrEmpty(id))
            {
                WSI.Planning.BeginDate = DateTime.Today.AddDays(1-DateTime.Today.Day);
                WSI.Planning.EndDate = DateTime.Today.AddDays(DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month) - DateTime.Today.Day);
                Session["SalesPLN"] = new List<MasterPlanningDetails>();
                Session["PurchsesPLN"] = new List<MasterPlanningDetails>();
                ViewData["Product"] = "";
                ViewData["Amount"] = 0;
            }
            else
            {
                Session["SalesPLN"] = WSI.Planning.Details.Where(i=>i.AccountId == "5111").ToList();
                Session["PurchsesPLN"] = WSI.Planning.Details.Where(i => i.AccountId == "1561").ToList();
                string product = WSI.Planning.Details.ToList()[0].ProductId;
                ViewData["Product"] = product;
                ViewData["Amount"] = WSI.Planning.Details.Where(i => i.AccountId == "1561" & i.ProductId == product).Sum(s => s.DebitAmount);
            }

            if (mode == "detail")
            {
                //lấy danh sách khách hàng
                //NMCustomersBL CBL = new NMCustomersBL();
                //NMCustomersWSI CWSI = new NMCustomersWSI();
                //CWSI.Mode = "SRC_OBJ";
                
                //ViewData["Partners"] = CBL.callListBL(CWSI);

                //NMMonthlyGeneralJournalsBL mgjBL = new NMMonthlyGeneralJournalsBL();
                //NMMonthlyGeneralJournalsWSI mgjWSI = new NMMonthlyGeneralJournalsWSI();
                //mgjWSI.Mode = "SRC_OBJ";
                //mgjWSI.MGJ.AccountId = "632";
                //mgjWSI.Filter.FromDate = WSI.Planning.BeginDate.ToString();
                //mgjWSI.Filter.ToDate = WSI.Planning.EndDate.ToString();

                //ViewData["Sales"] = mgjBL.callListBL(mgjWSI);

                //mgjWSI.Mode = "SRC_OBJ";
                //mgjWSI.MGJ.AccountId = "1561";
                //mgjWSI.Filter.FromDate = WSI.Planning.BeginDate.ToString();
                //mgjWSI.Filter.ToDate = WSI.Planning.EndDate.ToString();

                //ViewData["Purchases"] = mgjBL.callListBL(mgjWSI);

                return PartialView("PlanningByProductsDetail");
            }

            return PartialView();
        }

        public ActionResult PurchaseByProductDetail(string product, string begin, string to)
        {
            ViewData["Product"] = product;
            //lấy danh sách khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SRC_OBJ";

            ViewData["Partners"] = CBL.callListBL(CWSI);

            NMMonthlyGeneralJournalsBL mgjBL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI mgjWSI = new NMMonthlyGeneralJournalsWSI();
            mgjWSI.Mode = "SRC_OBJ";
            mgjWSI.MGJ.AccountId = "1561";
            mgjWSI.Filter.FromDate = NMCommon.convertDate(begin).ToString();
            mgjWSI.Filter.ToDate = NMCommon.convertDate(to).ToString();

            ViewData["Purchases"] = mgjBL.callListBL(mgjWSI);

            return PartialView();
        }

        public ActionResult SaleByProductDetail(string product, string begin, string to)
        {
            ViewData["Product"] = product;
            //lấy danh sách khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SRC_OBJ";

            ViewData["Partners"] = CBL.callListBL(CWSI);

            NMMonthlyGeneralJournalsBL mgjBL = new NMMonthlyGeneralJournalsBL();
            NMMonthlyGeneralJournalsWSI mgjWSI = new NMMonthlyGeneralJournalsWSI();
            mgjWSI.Mode = "SRC_OBJ";
            mgjWSI.MGJ.AccountId = "632";
            mgjWSI.Filter.FromDate = NMCommon.convertDate(begin).ToString();
            mgjWSI.Filter.ToDate = NMCommon.convertDate(to).ToString();

            ViewData["Sales"] = mgjBL.callListBL(mgjWSI);

            return PartialView();
        }

        public ActionResult PurchaseByProductsUC(string amount, string area, string productId)
        {
            //lấy danh sách khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SRC_OBJ";
            CWSI.Customer.AreaId = area;
            CWSI.Customer.GroupId = NMConstant.CustomerGroups.Supplier;

            ViewData["Partners"] = CBL.callListBL(CWSI);
            ViewData["Amount"] = amount;
            ViewData["Product"] = productId;

            return PartialView();
        }

        public ActionResult SalesByProductsUC(string amount, string area, string productId)
        {
            //lấy danh sách khách hàng
            NMCustomersBL CBL = new NMCustomersBL();
            NMCustomersWSI CWSI = new NMCustomersWSI();
            CWSI.Mode = "SRC_OBJ";
            CWSI.Customer.AreaId = area;
            CWSI.Customer.GroupId = NMConstant.CustomerGroups.Customer;

            ViewData["Partners"] = CBL.callListBL(CWSI);
            ViewData["Amount"] = amount;
            ViewData["Product"] = productId;

            return PartialView();
        }

        public JsonResult GetPurchaseAmount(string productId)
        {
            if (Session["PurchsesPLN"] != null)
            {
                List<MasterPlanningDetails> List = (List<MasterPlanningDetails>)Session["PurchsesPLN"];
                double total = List.Where(s => s.ProductId == productId).Sum(t => t.DebitAmount);
                return Json(total);
            }

            return Json("0");
        }

        public JsonResult AddPbPToList(string partnerId, string productId, string amount)
        {
            List<MasterPlanningDetails> List = new List<MasterPlanningDetails>();
            if (Session["PurchsesPLN"] != null)
                List = (List<MasterPlanningDetails>)Session["PurchsesPLN"];

            if (!string.IsNullOrEmpty(amount))
            {
                try
                {
                    List.Where(i => i.PartnerId == partnerId & i.ProductId == productId).FirstOrDefault().DebitAmount = double.Parse(amount);
                }
                catch
                {
                    MasterPlanningDetails pln = new MasterPlanningDetails();
                    pln.AccountId = "1561";
                    pln.PartnerId = partnerId;
                    pln.ProductId = productId;
                    pln.DebitAmount = double.Parse(amount);
                    pln.CreditAmount = 0;

                    pln.Descriptions = "Kế hoạch mua hàng.";
                    pln.CreatedBy = User.Identity.Name;
                    pln.CreatedDate = DateTime.Now;

                    List.Add(pln);
                }

                Session["PurchsesPLN"] = List;
            }
            double total = List.Where(s=>s.ProductId == productId).Sum(t => t.DebitAmount);

            return Json(new { total = total, rate = double.Parse(amount) / total });
        }

        public JsonResult AddSbPToList(string partnerId, string productId, string amount)
        {
            List<MasterPlanningDetails> List = new List<MasterPlanningDetails>();
            if (Session["SalesPLN"] != null)
                List = (List<MasterPlanningDetails>)Session["SalesPLN"];

            if (!string.IsNullOrEmpty(amount))
            {
                try
                {
                    List.Where(i => i.PartnerId == partnerId & i.ProductId == productId).FirstOrDefault().CreditAmount = double.Parse(amount);
                }
                catch
                {
                    MasterPlanningDetails pln = new MasterPlanningDetails();
                    pln.AccountId = "5111";
                    pln.PartnerId = partnerId;
                    pln.ProductId = productId;
                    pln.CreditAmount = double.Parse(amount);
                    pln.DebitAmount = 0;

                    pln.Descriptions = "Kế hoạch bán hàng.";                    
                    pln.CreatedBy = User.Identity.Name;
                    pln.CreatedDate = DateTime.Now;

                    List.Add(pln);
                }

                Session["SalesPLN"] = List;
            }
            
            double total = List.Where(s => s.ProductId == productId).Sum(t => t.CreditAmount);

            return Json(new { total = total, rate = double.Parse(amount) / total });
        }

        public JsonResult SaveSalesPlanningCustomerByProduct(string id, string begin, string end, string title, string description, string salesAmount)
        {
            NMMasterPlanningsBL BL = new NMMasterPlanningsBL();
            NMMasterPlanningsWSI wsi = new NMMasterPlanningsWSI();

            wsi.Mode = "SAV_OBJ";
            wsi.Planning.Id = id;
            wsi.Planning.BeginDate = DateTime.Parse(begin);
            wsi.Planning.EndDate = DateTime.Parse(end);
            wsi.Planning.Title = title;
            wsi.Planning.Descriptions = description;
            //wsi.Planning.SalesAmount = double.Parse(salesAmount);
            wsi.Planning.Status = "PLN01";
            wsi.Filter.ActionBy = User.Identity.Name;
            
            if (Session["SalesPLN"] != null & Session["PurchsesPLN"] != null)
            {
                List<MasterPlanningDetails> List = new List<MasterPlanningDetails>();
                List = (List<MasterPlanningDetails>)Session["SalesPLN"];
                List.AddRange((List<MasterPlanningDetails>)Session["PurchsesPLN"]);

                wsi.Planning.Details = ((List<MasterPlanningDetails>)Session["SalesPLN"]).ToList<MasterPlanningDetails>();

                wsi = BL.callSingleBL(wsi);

                if (wsi.WsiError == "")
                {
                    Session["SalesPLN"] = null;
                    Session["PurchsesPLN"] = null;
                }

                return Json(new { id = wsi.Planning.Id, error = wsi.WsiError });
            }
            else
            {
                return Json(new { id = "", error = "Bạn chưa có dữ liệu!" });
            }
        }

        public JsonResult DeletePlan(string id)
        {
            NMMasterPlanningsBL BL = new NMMasterPlanningsBL();
            NMMasterPlanningsWSI wsi = new NMMasterPlanningsWSI();

            wsi.Mode = "DEL_OBJ";
            wsi.Planning.Id = id;
            wsi = BL.callSingleBL(wsi);

            return Json(wsi.WsiError);
        }

        public ActionResult Results()
        {
            return PartialView();
        }

    }
}
