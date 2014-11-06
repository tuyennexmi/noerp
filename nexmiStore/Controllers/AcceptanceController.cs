// AcceptanceController.cs

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
    public class AcceptanceController : Controller
    {
        //
        // GET: /Acceptance/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Acceptance/Details/5

        public ActionResult Details(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                NMAcceptanceBL BL = new NMAcceptanceBL();
                BL.WSI.Acceptance.Id = id;

                BL.SelectObject();

                ViewData["WSI"] = BL.WSI;
                Session["Details"] = BL.WSI.Details;
            }

            ViewData["Mode"] = "Detail";

            return PartialView();
        }

        //
        // GET: /Acceptance/Create

        public ActionResult Create(string soId)
        {
            NMAcceptanceBL BL = new NMAcceptanceBL();
            NMSalesOrdersWSI SOWSI = new NMSalesOrdersWSI();

            if (!string.IsNullOrEmpty(soId))
            {
                NMSalesOrdersBL SOBL = new NMSalesOrdersBL();

                SOWSI.Mode = "SEL_OBJ";
                SOWSI.Order.OrderId = soId;
                SOWSI = SOBL.callSingleBL(SOWSI);

                BL.FromSalesOrder(SOWSI);
            }

            ViewData["SOWSI"] = SOWSI;
            ViewData["WSI"] = BL.WSI;
            Session["Details"] = BL.WSI.Details;
            ViewData["Mode"] = "Create";

            return PartialView();
        } 

        //
        // POST: /Acceptance/Create

        [HttpPost]
        public ActionResult SaveOrUpdateAcceptance(Acceptance acceptance)
        {
            NMAcceptanceBL BL = new NMAcceptanceBL();
            try
            {
                // TODO: Add insert logic here
                BL.Filter.ActionBy = User.Identity.Name;
                BL.WSI.Acceptance = acceptance;
                BL.WSI.Details = (List<AcceptanceDetails>)Session["Details"];

                BL.SaveObject();

                //ViewData["WSI"] = BL.WSI;
            }
            catch
            {   
            }

            //return PartialView("Details");
            return Json(new { id = BL.WSI.Acceptance.Id, error = BL.WsiError});
        }

        public ActionResult ConfirmFinish(string id)
        {
            NMAcceptanceBL BL = new NMAcceptanceBL();
            if (!string.IsNullOrEmpty(id))
            {   
                BL.WSI.Acceptance.Id = id;
                BL.SelectObject();

                BL.WSI.Acceptance.StatusId = AcceptanceStatuses.Done;
                BL.Filter.ActionBy = User.Identity.Name;

                BL.SaveObject();
            }

            return Json(new { error = BL.WsiError });
        }
        
        //
        // GET: /Acceptance/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Acceptance/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Acceptance/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Acceptance/Delete/5

        [HttpPost]
        public ActionResult Delete(string id)
        {
            NMAcceptanceBL BL = new NMAcceptanceBL();
            if (!string.IsNullOrEmpty(id))
            {
                BL.WSI.Acceptance.Id = id;

                BL.DeleteObject();
            }
            else
            {
                BL.WsiError = "Không nhận được ID của biên bản!";
            }

            return Json(new { error = BL.WsiError });
        }

        public ActionResult DetailLineForm(string productId)
        {
            List<AcceptanceDetails> Details = new List<AcceptanceDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<AcceptanceDetails>)Session["Details"];
            }

            ViewData["Detail"] = Details.Find(p=>p.ProductId == productId);

            return PartialView();
        }

        public ActionResult SaveOrUpdateLine(   string Quantity, string Price, string Discount, string Tax,
                                                string ProductId, string UnitId, string Description, string AcceptanceId    )
        {
            List<AcceptanceDetails> Details = new List<AcceptanceDetails>();
            if (Session["Details"] != null)
            {
                Details = (List<AcceptanceDetails>)Session["Details"];
            }
            AcceptanceDetails line = Details.Find(p => p.ProductId == ProductId);
            
            line.Quantity = double.Parse(Quantity);
            line.Price = double.Parse(Price);
            line.Discount = double.Parse(Discount);
            line.Tax = double.Parse(Tax);
            line.Description = Description;
            
            ViewData["Detail"] = line;

            return PartialView("DetailLines");
        }

        public ActionResult BySalesOrder(string soId)
        {
            if (!string.IsNullOrEmpty(soId))
            {
                NMAcceptanceBL BL = new NMAcceptanceBL();
                BL.WSI.Acceptance.SalesOrderId = soId;

                ViewData["WSIs"] = BL.SearchObject();
            }

            return PartialView();
        }
    }
}
