// UserControlController.cs

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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using iTextSharp.text.html;
using System.Text;
using NEXMI;
using LinqToExcel;

namespace nexmiStore.Controllers
{
    public class UserControlController : Controller
    {
        #region Call Service

        #endregion

        public ActionResult Funtions()
        {
            return PartialView();
        }

        #region Manufacturers
        public ActionResult ManufacturerList()
        {
            return PartialView();
        }
        public ActionResult slManufacturers()
        {
            return PartialView();
        }
        #endregion

        #region Suppliers
        public ActionResult SupplierList()
        {
            return PartialView();
        }
        public ActionResult slSuppliers()
        {
            return PartialView();
        }
        #endregion
        
        #region Customers

        public ActionResult cbbCustomers(string cgroupId, string customerId, string customerName, string elementId, string windowTitle, string holderText)
        {
            if (elementId == null) elementId = "";
            if (cgroupId == null)
                cgroupId = "";
            ViewData["WindowTitle"] = windowTitle;
            ViewData["GroupId"] = cgroupId;
            ViewData["CustomerId"] = customerId;
            ViewData["CustomerName"] = customerName;
            ViewData["ElementId"] = elementId;
            ViewData["HolderText"] = holderText;
            
            return PartialView();
        }

        public ActionResult CustomerList(string parentId, string groupId, string windowId)
        {
            ViewData["ParentId"] = parentId;
            ViewData["GroupId"] = groupId;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }
        #endregion

        #region Products
        public ActionResult ProductList(string parentId, string windowId)
        {
            ViewData["ParentId"] = parentId;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }
        public ActionResult slProducts()
        {
            return PartialView();
        }
        #endregion

        public ActionResult slUsers(string elementId, string userId)
        {
            NEXMI.NMCustomersBL BL = new NEXMI.NMCustomersBL();
            NEXMI.NMCustomersWSI WSI = new NEXMI.NMCustomersWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Customer = new Customers();
            WSI.Customer.GroupId = NMConstant.CustomerGroups.User;
            List<NEXMI.NMCustomersWSI> WSIs = BL.callListBL(WSI);
            ViewData["WSIs"] = new SelectList(WSIs.Select(i => i.Customer), "CustomerId", "CompanyNameInVietnamese", userId);
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public ActionResult ChangePassword(string windowId)
        {
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public ActionResult ChangeInfo(string windowId)
        {
            NEXMI.NMCustomersWSI WSI = (NEXMI.NMCustomersWSI)Session["UserInfo"];
            ViewData["id"] = WSI.Customer.CustomerId;
            ViewData["code"] = WSI.Customer.Code;
            ViewData["password"] = NMCryptography.ECBDecrypt(WSI.Customer.Password);
            ViewData["name"] = WSI.Customer.CompanyNameInVietnamese;
            ViewData["phoneNumber"] = WSI.Customer.Cellphone;
            ViewData["Email"] = WSI.Customer.EmailAddress;
            try
            {
                ViewData["EmailPassword"] = NMCryptography.ECBDecrypt(WSI.Customer.EmailPassword);
            }
            catch { }
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        #region SI
        public ActionResult SalesInvoiceBatchList()
        {
            return PartialView();
        }

        public ActionResult SaleInvoiceList(string windowId)
        {
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult GetSaleInvoiceDetails(string id)
        {
            ArrayList temp = new ArrayList();
            if (Session["ExportDetails"] != null)
            {
                temp = (ArrayList)Session["ExportDetails"];
            }
            NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
            NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Invoice = new SalesInvoices();
            WSI.Invoice.InvoiceId = id;
            WSI = BL.callSingleBL(WSI);
            NMProductsBL ProductBL = new NMProductsBL();
            NMProductsWSI ProductWSI;
            NMProductUnitsBL PUBL = new NMProductUnitsBL();
            NMProductUnitsWSI PUWSI;
            ArrayList objs = new ArrayList();
            ArrayList obj;
            //for (int i = 0; i < WSI.ProductIdList.Count; i++)
            foreach (SalesInvoiceDetails Item in WSI.Details)
            {
                ProductWSI = new NEXMI.NMProductsWSI();
                ProductWSI.Mode = "SEL_OBJ";
                ProductWSI.Product = new Products();
                ProductWSI.Product.ProductId = Item.ProductId;
                ProductWSI = ProductBL.callSingleBL(ProductWSI);
                PUWSI = new NEXMI.NMProductUnitsWSI();
                PUWSI.Mode = "SEL_OBJ";
                PUWSI.Id = ProductWSI.Product.ProductUnit;
                PUWSI = PUBL.callSingleBL(PUWSI);
                obj = new ArrayList();
                obj.Add(Item.ProductId);
                obj.Add((ProductWSI.Translation == null) ? "" : ProductWSI.Translation.Name);
                obj.Add(Item.Quantity);
                obj.Add(PUWSI.Name);
                objs.Add(obj);
            }
            ArrayList data = new ArrayList();
            ArrayList item1 = null, item2 = null;
            bool flag = true;
            for (int i = 0; i < objs.Count; i++)
            {
                item1 = (ArrayList)objs[i];
                item2 = null;
                data.Add(item1);
                for (int j = 0; j < temp.Count; j++)
                {
                    item2 = (ArrayList)temp[j];
                    if (item1[0] == item2[0])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag && item2 != null)
                {
                    data.Add(item2);
                }
            }
            Session["ExportDetails"] = objs;
            return Json(objs);
        }
        #endregion

        #region PI
        public ActionResult PurchaseInvoiceBatchList()
        {
            return PartialView();
        }

        public ActionResult PurchaseInvoiceList(string windowId)
        {
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult GetPurchaseInvoiceDetails(string id)
        {
            ArrayList temp = new ArrayList();
            if (Session["ImportDetails"] != null)
            {
                temp = (ArrayList)Session["ImportDetails"];
            }
            NMPurchaseInvoicesBL BL = new NMPurchaseInvoicesBL();
            NMPurchaseInvoicesWSI WSI = new NMPurchaseInvoicesWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Invoice = new NEXMI.PurchaseInvoices();
            WSI.Invoice.InvoiceId = id;
            WSI = BL.callSingleBL(WSI);
            NMProductsBL ProductBL = new NMProductsBL();
            NMProductsWSI ProductWSI;
            ArrayList objs = new ArrayList();
            ArrayList obj;
            foreach (PurchaseInvoiceDetails Item in WSI.Details)
            {
                ProductWSI = new NEXMI.NMProductsWSI();
                ProductWSI.Mode = "SEL_OBJ";
                ProductWSI.Product = new Products();
                ProductWSI.Product.ProductId = Item.ProductId;
                ProductWSI = ProductBL.callSingleBL(ProductWSI);
                obj = new ArrayList();
                obj.Add(ProductWSI.Product.ProductId);
                obj.Add((ProductWSI.Translation == null) ? "" : ProductWSI.Translation.Name);
                obj.Add(Item.Quantity.ToString("N3"));
                obj.Add(0.ToString("N3"));
                obj.Add((ProductWSI.Unit == null) ? "" : ProductWSI.Unit.Name);
                objs.Add(obj);
            }
            ArrayList data = new ArrayList();
            ArrayList item1 = null, item2 = null;
            bool flag = true;
            for (int i = 0; i < objs.Count; i++)
            {
                item1 = (ArrayList)objs[i];
                item2 = null;
                data.Add(item1);
                for (int j = 0; j < temp.Count; j++)
                {
                    item2 = (ArrayList)temp[j];
                    if (item1[0] == item2[0])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag && item2 != null)
                {
                    data.Add(item2);
                }
            }
            Session["ImportDetails"] = data;
            return Json(data);
        }
        #endregion

        public string Upload(string dataUrl, string filePath, string fileName)
        {
            string fileNameTemp = fileName.Split('.')[0];
            string fileExtension = fileName.Split('.')[1];
            int i = 1;
            while (System.IO.File.Exists(filePath + fileNameTemp + fileExtension))
            {
                fileNameTemp = fileName + i;
                i++;
            }
            string Ilist = dataUrl.Split(',')[1];
            string base64string = Ilist;
            byte[] imageByte = Convert.FromBase64String(base64string);
            MemoryStream ms = new MemoryStream(imageByte, 0, imageByte.Length);
            System.Drawing.Image imageStream = System.Drawing.Image.FromStream(ms, true);
            imageStream.Save(filePath + fileNameTemp + "." + fileExtension);
            return fileNameTemp + "." + fileExtension;
        }

        public ActionResult StatusBar(string objectName, string current, string clickable, string ownerId, string tableName)
        {
            NMStatusesBL BL = new NMStatusesBL();
            NMStatusesWSI WSI = new NMStatusesWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Status = new Statuses();
            WSI.Status.ObjectName = objectName;
            ViewData["WSIs"] = BL.callListBL(WSI);
            //ViewData["WSIs"] = GlobalValues.GetStatusByObjectName(objectName);
            if (current == null) current = "";
            ViewData["Current"] = current;
            if (clickable == null) clickable = "";
            ViewData["Clickable"] = clickable;
            ViewData["OwnerId"] = ownerId;
            ViewData["TableName"] = tableName;
            return PartialView();
        }

        public ActionResult slStatuses(string elementId, string objectName, string current)
        {
            NMStatusesBL BL = new NMStatusesBL();
            NMStatusesWSI WSI = new NMStatusesWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Status = new Statuses();
            WSI.Status.ObjectName = objectName;
            List<NMStatusesWSI> WSIs = BL.callListBL(WSI);
            //chen them thop Tất cả
            WSI.Status.Id = "";
            WSI.Status.Name = "Tất cả";
            WSI.Status.English = "All";
            WSI.Status.Korea = "All";
            WSIs.Insert(0,WSI);

            switch (Session["Lang"].ToString())
            {
                case "vi":
                    ViewData["WSIs"] = new SelectList(WSIs.Select(i => i.Status), "Id", "Name", current);
                    break;
                case "en":
                    ViewData["WSIs"] = new SelectList(WSIs.Select(i => i.Status), "Id", "English", current);
                    break;
                case "kr":
                    ViewData["WSIs"] = new SelectList(WSIs.Select(i => i.Status), "Id", "Korea", current);
                    break;
                default:
                    ViewData["WSIs"] = new SelectList(WSIs.Select(i => i.Status), "Id", "Name", current);
                    break;
            }

            //ViewData["WSIs"] = new SelectList(GlobalValues.GetStatusByObjectName(objectName).Select(i => i.Status), "Id", "Name", current);
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public ActionResult PrintPage(string mode)
        {
            ViewData["Mode"] = mode;
            return View();
        }

        public ActionResult SendEmailForm(string id, string type, string html, string windowId)
        {
            string langId = Session["Lang"].ToString();

            string email = "", subject = "", content = "";
            string attachment = "", attachmentName = "", attachmentSize = "/";
            NEXMI.NMCustomersWSI userWSI = (NEXMI.NMCustomersWSI)Session["UserInfo"];
            switch (type)
            {
                case "ARCMP":
                    subject = "[Đối chiếu công nợ]";
                    content = "<p> Đề nghị đối chiếu công nợ.</p>" + "<br />" + NMCryptography.base64Decode(html)
                                + "<br />Trân trọng cảm ơn quý khách!"
                                + "<br />" + userWSI.Customer.CompanyNameInVietnamese
                                + "<p><strong>" + NMCommon.GetCompany().Customer.CompanyNameInVietnamese + "</strong></p>"
                                + "<p>" + NEXMI.NMCommon.GetInterface("TELEPHONE", langId) + ": " + NMCommon.GetCompany().Customer.Telephone + "</p>"
                                + "<p>" + NEXMI.NMCommon.GetInterface("ADDRESS", langId) + ": " + NMCommon.GetCompany().Customer.Address + "</p>";
                    attachmentName = "DCCN_" + id.Replace('/', '_') + ".pdf";
                    attachment = CreatePDF(NMCryptography.base64Decode(html), "ORD", "", "");
                    break;
                case "PAYREQ":
                    subject = "[Đề nghị thanh toán]";
                    content = "<p> Đề nghị thanh toán công nợ cho nhà cung cấp.</p>" + "<br />" + NMCryptography.base64Decode(html)
                                + "<br />Trân trọng cảm ơn quý khách!"
                                + "<br />" + userWSI.Customer.CompanyNameInVietnamese
                                + "<p><strong>" + NMCommon.GetCompany().Customer.CompanyNameInVietnamese + "</strong></p>"
                                + "<p>" + NEXMI.NMCommon.GetInterface("TELEPHONE", langId) + ": " + NMCommon.GetCompany().Customer.Telephone + "</p>"
                                + "<p>" + NEXMI.NMCommon.GetInterface("ADDRESS", langId) + ": " + NMCommon.GetCompany().Customer.Address + "</p>";
                    attachmentName = "DNTT_" + id.Replace('/', '_') + ".pdf";
                    attachment = CreatePDF(NMCryptography.base64Decode(html), "ORD", "", "");
                    break;
                case "PQuotation":
                    NMPurchaseOrdersBL POBL = new NMPurchaseOrdersBL();
                    NMPurchaseOrdersWSI POWSI = new NMPurchaseOrdersWSI();
                    POWSI.Mode = "SEL_OBJ";
                    POWSI.Order.Id = id;
                    POWSI = POBL.callSingleBL(POWSI);
                    email = POWSI.Supplier.EmailAddress;
                    subject = "[Yêu cầu báo giá] " + NMCommon.GetCompany().Customer.CompanyNameInVietnamese;
                    content = "<p>Xin chào, " + POWSI.Supplier.CompanyNameInVietnamese + "</p>"
                        + "<p>Đây là bảng yêu cầu báo giá từ " + NMCommon.GetCompany().Customer.CompanyNameInVietnamese + ".</p>"
                        + "<table border='1' cellpadding='3' cellspacing='0' style='width: 500px;'><tbody>"
                        + "<tr><td>" + NEXMI.NMCommon.GetInterface("ORDER_NO", langId) + ":</td><td>" + id + "</td></tr>"
                        + "<tr><td>" + NEXMI.NMCommon.GetInterface("ORDER_DATE", langId) + ":</td><td>" + POWSI.Order.OrderDate.ToString("dd/MM/yyyy") + "</td></tr>"
                        + "<tr><td>" + NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) + ":</td><td>" + POWSI.Order.TotalAmount.ToString("N3") + "</td></tr>"
                        + "</tbody></table>"
                        + "<p>Vui lòng tải về tập tin đính kèm để xem chi tiết!</p>"
                        + "<br />"
                        + "Trân trọng cảm ơn!"
                        + userWSI.Customer.CompanyNameInVietnamese
                        + "<p><strong>" + NMCommon.GetCompany().Customer.CompanyNameInVietnamese + "</strong></p>"
                        + "<p>" + NEXMI.NMCommon.GetInterface("TELEPHONE", langId) + ": " + NMCommon.GetCompany().Customer.Telephone + "</p>"
                        + "<p>" + NEXMI.NMCommon.GetInterface("ADDRESS", langId) + ": " + NMCommon.GetCompany().Customer.Address + "</p>";
                    attachmentName = "Quotation_" + id.Replace('/', '_') + ".pdf";
                    attachment = CreatePDF(NMCryptography.base64Decode(html), "ORD", "", "");
                    ViewData["Status"] = NEXMI.NMConstant.POStatuses.Sent;
                    break;
                default:
                    NMSalesOrdersBL SOBL = new NMSalesOrdersBL();
                    NMSalesOrdersWSI SOWSI = new NMSalesOrdersWSI();
                    SOWSI.Mode = "SEL_OBJ";
                    SOWSI.Order.OrderId = id;
                    SOWSI = SOBL.callSingleBL(SOWSI);
                    email = SOWSI.Customer.EmailAddress;
                    subject = "[ " + NEXMI.NMCommon.GetInterface("QUOTATION_TITLE", langId) + " ] " + NMCommon.GetCompany().Customer.CompanyNameInVietnamese;
                    content = "<p>Xin chào, " + SOWSI.Customer.CompanyNameInVietnamese + "</p>"
                        + "<p>Đây là bảng báo giá từ " + NMCommon.GetCompany().Customer.CompanyNameInVietnamese + ".</p>"
                        + "<table border='1' cellpadding='3' cellspacing='0' style='width: 500px;'><tbody>"
                        + "<tr><td>" + NEXMI.NMCommon.GetInterface("ORDER_NO", langId) + "</td><td>" + id + "</td></tr>"
                        + "<tr><td>" + NEXMI.NMCommon.GetInterface("ORDER_DATE", langId) + ":</td><td>" + SOWSI.Order.OrderDate.ToString("dd/MM/yyyy") + "</td></tr>"
                        + "<tr><td>" + NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) + "</td><td>" + SOWSI.Order.TotalAmount.ToString("N3") + "</td></tr>"
                        + "</tbody></table>"
                        + "<p>Vui lòng tải về tập tin đính kèm để xem chi tiết!</p>"
                        + "<br />Trân trọng cảm ơn quý khách!<br />"
                        + userWSI.Customer.CompanyNameInVietnamese
                        + "<p><strong>" + NMCommon.GetCompany().Customer.CompanyNameInVietnamese + "</strong></p>"
                        + "<p>" + NEXMI.NMCommon.GetInterface("TELEPHONE", langId) + ": " + NMCommon.GetCompany().Customer.Telephone + "</p>"
                        + "<p>" + NEXMI.NMCommon.GetInterface("ADDRESS", langId) + ": " + NMCommon.GetCompany().Customer.Address + "</p>";
                    attachmentName = "Quotation_" + id.Replace('/', '_') + ".pdf";
                    attachment = CreatePDF(NMCryptography.base64Decode(html), "ORD", "", "");
                    ViewData["Status"] = NEXMI.NMConstant.SOStatuses.Sent;

                    // file dinh kem khac
                    NMImagesBL ImageBL = new NMImagesBL();
                    NMImagesWSI ImageWSI = new NMImagesWSI();
                    ImageWSI.Mode = "SRC_OBJ";

                    if (!String.IsNullOrEmpty(id))
                    {
                        ImageWSI.Owner = id;
                    }
                    List<NMImagesWSI> ImageWSIs = ImageBL.callListBL(ImageWSI);
                    
                    Session["ImageWSIs"] = ImageWSIs;

                    break;
            }
            ViewData["Id"] = id;
            ViewData["Type"] = type;
            ViewData["Email"] = email;
            ViewData["Subject"] = subject;
            ViewData["Content"] = content;
            ViewData["AttachmentName"] = attachmentName;
            ViewData["AttachmentSize"] = attachmentSize;
            ViewData["Attachment"] = attachment;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SendEmail(string emails, string subject, string content, string attachmentNames, string attachmentDatas)
        {
            string error = "";
            string email = "";
            string emailPassword = "";
            try
            {
                string companyName = NMCommon.GetCompany().Customer.CompanyNameInVietnamese;
                NMCustomersBL CustomerBL = new NMCustomersBL();
                NMCustomersWSI CustomerWSI = (NEXMI.NMCustomersWSI)Session["UserInfo"];
                if (CustomerWSI.Customer != null)
                {
                    email = CustomerWSI.Customer.EmailAddress;
                    emailPassword = NMCryptography.ECBDecrypt(CustomerWSI.Customer.EmailPassword);
                }
                if (email != "" && emailPassword != "")
                {
                    MailMessage oMail = new MailMessage();
                    SmtpClient oSmtp = new SmtpClient();
                    string host = email.Split('@')[1];
                    oSmtp.Host = "smtp." + host;
                    oMail.IsBodyHtml = true;
                    //oSmtp.UseDefaultCredentials = true;
                    oSmtp.Credentials = new NetworkCredential(email, emailPassword);
                    oSmtp.EnableSsl = true;
                    oSmtp.Port = 587;
                    oMail.From = new MailAddress(email, companyName, System.Text.Encoding.UTF8);
                    oMail.Subject = subject;
                    oMail.Body = NMCryptography.base64Decode(content) + "<br /><br /><div>Sent by <a href=\"http://www.nexmi.com\" target=\"_blank\" style=\"text-decoration: none; color: #f7941d\">NEXMI IBM Solutions</a></div>";
                    
                    string[] nameTemps = Regex.Split(attachmentNames, "_NEXMI_");
                    string[] dataTemps = Regex.Split(attachmentDatas, "_NEXMI_");
                    MemoryStream ms;
                    for (int i = 0; i < dataTemps.Length - 1; i++)
                    {
                        byte[] data = Convert.FromBase64String(dataTemps[i]);
                        ms = new MemoryStream(data, 0, data.Length);
                        oMail.Attachments.Add(new Attachment(ms, nameTemps[i]));
                    }

                    if (Session["ImageWSIs"] != null)
                    {
                        List<NEXMI.NMImagesWSI> ImageWSIs = (List<NEXMI.NMImagesWSI>)Session["ImageWSIs"];
                        foreach (NEXMI.NMImagesWSI Item in ImageWSIs)
                        {
                            string filePathOriginal = Server.MapPath(@"~/uploads/files/" + Item.Location);
                            if (System.IO.File.Exists(filePathOriginal))
                            {
                                byte[] filedata = System.IO.File.ReadAllBytes(filePathOriginal);
                                ms = new MemoryStream(filedata, 0, filedata.Length);
                                oMail.Attachments.Add(new Attachment(ms, Item.Name));
                            }
                        }
                    }
                    
                    if (!String.IsNullOrEmpty(emails))
                    {
                        string[] recipients = emails.Split(',');
                        foreach (string item in recipients)
                        {
                            if (item != "")
                            {
                                oMail.To.Clear();
                                oMail.To.Add(item);
                                oSmtp.Send(oMail);
                            }
                        }
                    }
                    else
                    {
                        NMCustomersBL BL = new NMCustomersBL();
                        NMCustomersWSI WSI = new NMCustomersWSI();
                        WSI.Mode = "SRC_OBJ";
                        List<NMCustomersWSI> WSIs = BL.callListBL(WSI);
                        foreach (NMCustomersWSI Item in WSIs)
                        {
                            if (!String.IsNullOrEmpty(Item.Customer.EmailAddress))
                            {
                                oMail.To.Clear();
                                oMail.To.Add(Item.Customer.EmailAddress);
                                oSmtp.Send(oMail);
                            }
                        }
                    }

                    Session["ImageWSIs"] = null;
                }
                else
                {
                    error = "Vui lòng cập nhật email cá nhân để thực hiện thao tác này!";
                }
            }
            catch (Exception ex)
            {
                error = "Không thực hiện được.\nVui lòng liên hệ người quản trị để khắc phục.\nChi tiết lỗi: " + ex.Message;
            }
            return Json(error);
        }

        public JsonResult GetDataUrl(string html, string mode, string orient, string size)
        {
            Session["DataUrl"] = CreatePDF(NMCryptography.base64Decode(html), mode, orient, size);
            return Json("");
        }

        public string CreatePDF(string html, string mode, string orient, string size)
        {
            MemoryStream msOutput = new MemoryStream();
            Document document ;
            if (size == "A5")
            {
                document = new Document(PageSize.A5.Rotate(), 20, 20, 30, 30);
            }
            else
            {
                if (!string.IsNullOrEmpty(orient))
                    document = new Document(PageSize.A4.Rotate(), 20, 20, 30, 30);
                else
                    document = new Document(PageSize.A4, 20, 20, 30, 30);
            }

            FontFactory.Register(Server.MapPath(@"~/Content/Data/ARIALUNI.TTF"));
            StyleSheet styles = new StyleSheet();
            styles.LoadTagStyle("body", HtmlTags.FACE, "Arial Unicode MS");
            styles.LoadTagStyle("body", HtmlTags.ENCODING, BaseFont.IDENTITY_H);
            //styles.LoadStyle("STT", "width", "30px");
            HTMLWorker hw = new HTMLWorker(document);
            hw.SetStyleSheet(styles);
            PdfWriter writer = PdfWriter.GetInstance(document, msOutput);
            document.Open();
            writer.PageEvent = new Footer();
            //if ((mode == "ORD") | (mode == "PAYREQ") | (mode == "QUOTE" && (NMCommon.GetSettingValue("PRINT_QUOTATION_FORM") == 0)))
            //{
            //    var logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/avatars/" + NMCommon.GetCompany().Customer.Avatar));
            //    logo.SetAbsolutePosition(document.GetLeft(0), document.GetTop(60));
            //    document.Add(logo);
            //}
            hw.Parse(new StringReader(html));
            document.Close();

            return Convert.ToBase64String(msOutput.ToArray());
        }

        public partial class Footer : PdfPageEventHelper
        {

            public override void OnEndPage(PdfWriter writer, Document doc)
            {
                Paragraph footer = new Paragraph("Printed from NEXMI IBMs", FontFactory.GetFont("Arial Unicode MS", 9, iTextSharp.text.Font.NORMAL));
                footer.Alignment = Element.ALIGN_RIGHT;
                PdfPTable footerTbl = new PdfPTable(1);
                footerTbl.TotalWidth = 210;
                footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell cell = new PdfPCell(footer);
                cell.Border = 0;
                cell.PaddingLeft = 10;
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 450, 20, writer.DirectContent);
            }

        }

        public ActionResult ExportList(string orderId)
        {
            ViewData["OrderId"] = orderId;
            return PartialView();
        }

        public ActionResult MainMenu()
        {
            NEXMI.NMModulesWSI ModuleWSI = new NEXMI.NMModulesWSI();
            NEXMI.NMModulesBL ModuleBL = new NEXMI.NMModulesBL();
            ModuleWSI.Mode = "SRC_OBJ";
            ModuleWSI.Active = true.ToString();
            ViewData["WSIs"] = ModuleBL.callListBL(ModuleWSI);

            //NEXMI.NMCustomersBL UserBL = new NEXMI.NMCustomersBL();
            //NEXMI.NMCustomersWSI UserWSI = new NEXMI.NMCustomersWSI();
            //UserWSI.Mode = "SEL_OBJ";
            //UserWSI.Customer = new Customers();
            //UserWSI.Customer.CustomerId = User.Identity.Name;
            //UserWSI = UserBL.callSingleBL(UserWSI);
            if (Session["UserInfo"] != null)
            {
                ViewData["UserName"] = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.CompanyNameInVietnamese;    //UserInfoCache.User.Customer.CompanyNameInVietnamese;
                if (!string.IsNullOrEmpty(((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.Avatar))
                    ViewData["Avatar"] = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.Avatar;  //UserInfoCache.User.Customer.Avatar;
                else
                    ViewData["Avatar"] = "personal.png";
            }
            return PartialView();
        }

        public ActionResult SubMenu(string moduleId)
        {
            List<NMFunctionsWSI> WSIs = new List<NMFunctionsWSI>();
            if (!string.IsNullOrEmpty(moduleId))
            {
                NMFunctionsBL BL = new NMFunctionsBL();
                NMFunctionsWSI WSI = new NMFunctionsWSI();
                WSI.Mode = "SRC_OBJ";
                WSI.ModuleId = moduleId;
                WSI.Active = true.ToString();
                WSIs = BL.callListBL(WSI);
            }
            ViewData["WSIs"] = WSIs;
            return PartialView();
        }

        #region BankOfCustomers
        public ActionResult BankOfCustomerForm(string id)
        {
            ViewData["Id"] = id;
            return PartialView();
        }
        #endregion

        #region Bank
        public ActionResult BankForm(string id)
        {
            ViewData["Id"] = id;
            return PartialView();
        }

        public JsonResult SaveBank(string bankId, string bankCode, string bankName, string bankAddress, string areaId)
        {
            NMBanksBL BL = new NMBanksBL();
            NMBanksWSI WSI = new NMBanksWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Bank = new Banks();
            WSI.Bank.Id = bankId;
            WSI.Bank.Code = bankCode;
            WSI.Bank.Name = bankName;
            WSI.Bank.Address = bankAddress;
            WSI.Bank.AreaId = areaId;
            WSI = BL.callSingleBL(WSI);
            return Json(new { result = WSI.Bank.Id, error = WSI.WsiError });
        }
        #endregion

        //#region slAccountNumbers
        //public ActionResult slAccountNumbers()
        //{
        //    return PartialView();
        //}
        //#endregion

        #region Payments
        public ActionResult PaymentForm()
        {
            return PartialView();
        }
        #endregion

        #region Receipts
        public ActionResult ReceiptForm(string id, string invoiceId, string type, string viewName, string windowId, string lang)
        {
            DateTime receiptDate = DateTime.Now;
            string receiptAmount = "", status = NMConstant.ReceiptStatuses.Draft, description = "", customerId = "", customerName = "", receiptTypeId = "";
            string createBy = User.Identity.Name;

            if(!String.IsNullOrEmpty(type))
                ViewData["TypeId"] = type;

            if (!string.IsNullOrEmpty(id))
            {
                NEXMI.NMReceiptsBL BL = new NEXMI.NMReceiptsBL();
                NEXMI.NMReceiptsWSI WSI = new NEXMI.NMReceiptsWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Receipt = new NEXMI.Receipts();
                WSI.Receipt.ReceiptId = id;
                WSI = BL.callSingleBL(WSI);
                ViewData["WSI"] = WSI;
                if (WSI.Customer != null)
                {
                    customerId = WSI.Customer.CustomerId;
                    customerName = WSI.Customer.CompanyNameInVietnamese;
                }
                try
                {
                    receiptDate = WSI.Receipt.ReceiptDate;
                }
                catch { }
                invoiceId = WSI.Receipt.InvoiceId;
                receiptTypeId = WSI.Receipt.ReceiptTypeId;
                //amount = WSI.Receipt.Amount.ToString("N3");
                receiptAmount = WSI.Receipt.ReceiptAmount.ToString("N3");
                status = WSI.Receipt.ReceiptStatus;
                description = WSI.Receipt.DescriptionInVietnamese;
                if (WSI.Customer != null)
                {
                    customerId = WSI.Customer.CustomerId;
                    customerName = WSI.Customer.CompanyNameInVietnamese;
                }
                if (WSI.PaymentMethod != null)
                {
                    ViewData["PaymentMethodId"] = WSI.PaymentMethod.Id;
                    ViewData["PaymentMethodName"] = WSI.PaymentMethod.Name;
                }
                ViewData["TypeId"] = WSI.Receipt.ReceiptTypeId;
                ViewData["TypeName"] = GlobalValues.GetACCType(WSI.Receipt.ReceiptTypeId).AccountNumber.NameInVietnamese;
                createBy = WSI.Receipt.CreatedBy;
            }
            else
            {
                if (!string.IsNullOrEmpty(invoiceId))
                {
                    NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
                    NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
                    WSI.Mode = "SEL_OBJ";
                    WSI.Invoice = new SalesInvoices();
                    WSI.Invoice.InvoiceId = invoiceId;
                    WSI = BL.callSingleBL(WSI);
                    if (WSI.Customer != null)
                    {
                        customerId = WSI.Customer.CustomerId;
                        customerName = WSI.Customer.CompanyNameInVietnamese;
                    }

                    ViewData["TypeId"] = "131";//WSI.Invoice.InvoiceTypeId;
                    ViewData["TotalAmount"] = WSI.Invoice.TotalAmount.ToString("N3");
                    ViewData["Balance"] = (WSI.Invoice.TotalAmount - WSI.Receipts.Sum(i => i.ReceiptAmount) + WSI.Payments.Sum(i => i.PaymentAmount)).ToString("N3");
                    ViewData["disabled"] = "disabled";
                }
            }
            
            ViewData["ReceiptId"] = id;
            ViewData["InvoiceId"] = invoiceId;
            ViewData["CustomerId"] = customerId;
            ViewData["CustomerName"] = customerName;
            ViewData["Description"] = description;
            ViewData["ReceiptAmount"] = receiptAmount;
            if (string.IsNullOrEmpty(status)) status = NMConstant.ReceiptStatuses.Draft;
            ViewData["Status"] = status;
            ViewData["WindowId"] = windowId;
            if (!string.IsNullOrEmpty(viewName))
                ViewData["ReceiptDate"] = receiptDate.ToString("dd/MM/yyyy");
            else
                ViewData["ReceiptDate"] = receiptDate.ToString("yyyy-MM-dd");
            ViewData["CreateBy"] = createBy;
            ViewData["Lang"] = lang;

            return PartialView(viewName);
        }

        public JsonResult SaveReceipt(string txtReceiptId, string cbbCustomers, string txtInvoiceId, string txtReceiptDate,
            string txtTotalAmount, string txtDescription, string slReceiptTypes, string slPaymentMethods, string txtBalance,
            string txtPaidAmount, string status, string bank)
        {
            NMReceiptsBL BL = new NMReceiptsBL();
            NMReceiptsWSI WSI = new NMReceiptsWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Receipt = new Receipts();
            WSI.Receipt.ReceiptId = txtReceiptId;
            WSI.Receipt.ReceiptDate = DateTime.Parse(txtReceiptDate);
            WSI.Receipt.CustomerId = cbbCustomers;
            if (!string.IsNullOrEmpty(txtInvoiceId))
                WSI.Receipt.InvoiceId = txtInvoiceId;
            WSI.Receipt.ReceiptStatus = status;
            
            double totalAmount = 0, paidAmount = 0, balance = 0;

            try
            {
                totalAmount = double.Parse(txtTotalAmount);
                balance = double.Parse(txtBalance);
            }
            catch { }
            paidAmount = double.Parse(txtPaidAmount);
            WSI.Receipt.CurrencyId = "VND";
            WSI.Receipt.ExchangeRate = 1;
            WSI.Receipt.Amount = paidAmount;
            //WSI.Receipt.ReceiptAmount = paidAmount;
            WSI.Receipt.DescriptionInVietnamese = txtDescription;
            if (!String.IsNullOrEmpty(slReceiptTypes)) WSI.Receipt.ReceiptTypeId = slReceiptTypes;
            if (!String.IsNullOrEmpty(slPaymentMethods)) WSI.Receipt.PaymentMethodId = slPaymentMethods;
            if (!String.IsNullOrEmpty(bank)) WSI.Receipt.ReceivedBankAccount = bank;
            
            WSI.ActionBy = User.Identity.Name;
            WSI.Receipt.StockId = ((NMCustomersWSI)Session["UserInfo"]).Customer.StockId;

            WSI = BL.callSingleBL(WSI);
                        
            //if (WSI.WsiError == "")
            //{
            //    if (totalAmount <= paidAmount + (totalAmount - balance) && status == NMConstant.ReceiptStatuses.Done)
            //    {
            //        WSI.WsiError = NMCommon.UpdateObjectStatus("SalesInvoices", txtInvoiceId, NMConstant.SIStatuses.Paid, "", User.Identity.Name, "Trạng thái: <%= NEXMI.NMCommon.GetInterface("PAID", langId) %>");
            //    }
            //}
            return Json(new { id = WSI.Receipt.ReceiptId, error = WSI.WsiError });
        }

        public JsonResult ApproveReceipt(string id, string statusId)
        {
            NMReceiptsBL BL = new NMReceiptsBL();
            NMReceiptsWSI WSI = new NMReceiptsWSI();
            WSI.Mode = "APP_OBJ";
            WSI.Receipt.ReceiptId = id;
            WSI.Receipt.ReceiptStatus = statusId;
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            
            return Json(WSI.WsiError);
        }
        #endregion

        #region Refund
        public ActionResult RefundForm(string invoiceId, string typeId, string windowId)
        {
            DateTime paymentDate = DateTime.Now;
            if (!string.IsNullOrEmpty(invoiceId))
            {
                NMSalesInvoicesBL BL = new NMSalesInvoicesBL();
                NMSalesInvoicesWSI WSI = new NMSalesInvoicesWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Invoice = new SalesInvoices();
                WSI.Invoice.InvoiceId = invoiceId;
                WSI = BL.callSingleBL(WSI);
                ViewData["InvoiceId"] = invoiceId;
                if (WSI.Customer != null)
                {
                    ViewData["CustomerId"] = WSI.Customer.CustomerId;
                    ViewData["CustomerName"] = WSI.Customer.CompanyNameInVietnamese;
                }
                ViewData["TotalAmount"] = WSI.Invoice.TotalAmount.ToString("N3");
                ViewData["Paid"] = (WSI.Receipts.Sum(i => i.ReceiptAmount) - WSI.Payments.Sum(i => i.PaymentAmount)).ToString("N3");
                ViewData["disabled"] = "disabled";
            }
            ViewData["slPaymentTypes"] = typeId;
            ViewData["PaymentDate"] = paymentDate.ToString("yyyy-MM-dd");
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult SaveRefund(string txtPaymentId, string cbbCustomers, string txtInvoiceId, string txtPaymentDate, 
            string txtTotalAmount, string txtDescription, string slPaymentTypes, string slPaymentMethods, string txtPaid, string txtPayAmount)
        {
            NMPaymentsBL BL = new NMPaymentsBL();
            NMPaymentsWSI WSI = new NMPaymentsWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Payment = new Payments();
            WSI.Payment.PaymentId = txtPaymentId;
            WSI.Payment.PaymentDate = DateTime.Parse(txtPaymentDate);
            WSI.Payment.SupplierId = cbbCustomers;
            WSI.Payment.InvoiceId = txtInvoiceId;
            double totalAmount = 0;
            double payAmount = 0;
            try
            {
                totalAmount = double.Parse(txtTotalAmount);                
            }
            catch { }
            try
            {
                payAmount = double.Parse(txtPayAmount);
            }
            catch
            {
                return Json(new { id = "", error = "Chưa nhập số tiền thanh toán!" });
            }
            WSI.Payment.Amount = payAmount;
            WSI.Payment.PaymentAmount = payAmount;
            WSI.Payment.DescriptionInVietnamese = txtDescription;
            if (!String.IsNullOrEmpty(slPaymentTypes)) WSI.Payment.PaymentTypeId = slPaymentTypes;
            if (!String.IsNullOrEmpty(slPaymentMethods)) WSI.Payment.PaymentMethodId = slPaymentMethods;
            WSI.Payment.PaymentStatus = NMConstant.PaymentStatuses.Done;
            WSI.ActionBy = User.Identity.Name;
            WSI = BL.callSingleBL(WSI);
            return Json(new { id = txtInvoiceId, error = WSI.WsiError });
        }
        #endregion

        public ActionResult cbbCategories(string currentId, string currentName, string elementId, string objectName)
        {
            ViewData["CurrentId"] = currentId;
            ViewData["CurrentName"] = currentName;
            ViewData["ElementId"] = elementId;
            ViewData["ObjectName"] = objectName;
            return PartialView();
        }

        #region SO
        public ActionResult cbbSalesOrders(string elementId)
        {
            ViewData["elementId"] = elementId;
            return PartialView();
        }

        public ActionResult SalesOrderList(string windowId)
        {
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public JsonResult GetPurchaseOrderDetails(string id)
        {
            ArrayList temp = new ArrayList();
            if (Session["ImportDetails"] != null)
            {
                temp = (ArrayList)Session["ImportDetails"];
            }
            NMPurchaseOrdersBL BL = new NMPurchaseOrdersBL();
            NMPurchaseOrdersWSI WSI = new NMPurchaseOrdersWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Order = new PurchaseOrders();
            WSI.Order.Id = id;
            WSI = BL.callSingleBL(WSI);
            NMProductsBL ProductBL = new NMProductsBL();
            NMProductsWSI ProductWSI;
            ArrayList objs = new ArrayList();
            ArrayList obj;
            foreach (PurchaseOrderDetails Item in WSI.Details)
            {
                ProductWSI = new NEXMI.NMProductsWSI();
                ProductWSI.Mode = "SEL_OBJ";
                ProductWSI.Product = new Products();
                ProductWSI.Product.ProductId = Item.ProductId;
                ProductWSI = ProductBL.callSingleBL(ProductWSI);
                obj = new ArrayList();
                obj.Add(ProductWSI.Product.ProductId);
                obj.Add((ProductWSI.Translation == null) ? "" : ProductWSI.Translation.Name);
                obj.Add(Item.Quantity.ToString("N3"));
                obj.Add(0.ToString("N3"));
                obj.Add((ProductWSI.Unit == null) ? "" : ProductWSI.Unit.Name);
                objs.Add(obj);
            }
            ArrayList data = new ArrayList();
            ArrayList item1 = null, item2 = null;
            bool flag = true;
            for (int i = 0; i < objs.Count; i++)
            {
                item1 = (ArrayList)objs[i];
                item2 = null;
                data.Add(item1);
                for (int j = 0; j < temp.Count; j++)
                {
                    item2 = (ArrayList)temp[j];
                    if (item1[0] == item2[0])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag && item2 != null)
                {
                    data.Add(item2);
                }
            }
            Session["ImportDetails"] = data;
            return Json(data);
        }
        #endregion

        public ActionResult cbbPurchaseOrders(string elementId)
        {
            ViewData["elementId"] = elementId;
            return PartialView();
        }

        public ActionResult PurchaseOrderList(string windowId)
        {
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public ActionResult cbbSalesInvoices()
        {
            return PartialView();
        }

        public ActionResult cbbPurchaseInvoice()
        {
            return PartialView();
        }

        public ActionResult CategoryList(string parentId, string objectName, string windowId)
        {
            ViewData["ParentId"] = parentId;
            ViewData["ObjectName"] = objectName;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public ActionResult slPaymentMethods(string paymentMethod)
        {
            if (paymentMethod == null) paymentMethod = "";
            NEXMI.NMParametersBL BL = new NEXMI.NMParametersBL();
            NEXMI.NMParametersWSI WSI = new NEXMI.NMParametersWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.ObjectName = "PaymentMethods";
            List<NEXMI.NMParametersWSI> WSIs = BL.callListBL(WSI);
            //WSI = new NEXMI.NMParametersWSI();
            //WSI.Id = ""; WSI.Name = "";
            //WSIs.Insert(0, WSI);
            ViewData["WSIs"] = new SelectList(WSIs, "Id", "Name", paymentMethod);
            return PartialView();
        }

        public ActionResult slBankAccounts(string bankAccount, string elementId)
        {
            if (bankAccount == null) bankAccount = "";
            NEXMI.NMBanksBL BL = new NMBanksBL();
            NEXMI.NMBanksWSI WSI = new NMBanksWSI();
            WSI.Mode = "SRC_OBJ";
            //WSI.ObjectName = "PaymentMethods";
            List<NMBanksWSI> WSIs = BL.callListBL(WSI);
            WSI = new NMBanksWSI();
            WSI.Bank.Id = ""; WSI.Bank.Name = "";
            WSIs.Insert(0, WSI);
            ViewData["WSIs"] = new SelectList(WSIs, "Bank.Id", "Bank.Name", bankAccount);

            if (!string.IsNullOrEmpty(elementId))
                ViewData["ElementId"] = elementId;
            else
                ViewData["ElementId"] = "slBankAccounts";

            return PartialView();
        }

        public ActionResult slStocks(string elementId, string stockId)
        {
            NEXMI.NMStocksBL BL = new NEXMI.NMStocksBL();
            NEXMI.NMStocksWSI WSI = new NEXMI.NMStocksWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.LanguageId = Session["Lang"].ToString();
            List<NEXMI.NMStocksWSI> WSIs = BL.callListBL(WSI);
            var data = WSIs.Select(i => new
            {
                Id = i.Id,
                Name = (i.Translation == null) ? "" : i.Translation.Name
            });
            ViewData["WSIs"] = new SelectList(data, "Id", "Name", stockId);
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public ActionResult cbbAreas(string areaId, string areaName, string elementId, string holderText)
        {
            ViewData["AreaId"] = areaId;
            ViewData["AreaName"] = areaName;
            ViewData["ElementId"] = elementId;
            ViewData["HolderText"] = holderText;
            return PartialView();
        }

        public ActionResult AreaList(string parentId, string windowId)
        {
            ViewData["ParentId"] = parentId;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public ActionResult slEmailTemplates(string elementId, string value)
        {
            NMSystemParamsBL ParamBL = new NMSystemParamsBL();
            NMSystemParamsWSI ParamWSI = new NMSystemParamsWSI();
            ParamWSI.Mode = "SRC_OBJ";
            ParamWSI.SystemParam.ObjectName = NMConstant.ObjectNames.Marketing;
            List<SystemParams> list = ParamBL.callListBL(ParamWSI);
            ViewData["List"] = new SelectList(list, "Id", "Name", value);
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public ActionResult slTypes(string elementId, string objectName, string value)
        {
            NMTypesBL TypeBL = new NMTypesBL();
            NMTypesWSI TypeWSI = new NMTypesWSI();
            TypeWSI.Mode = "SRC_OBJ";
            TypeWSI.ObjectName = objectName;
            ViewData["List"] = new SelectList(TypeBL.callListBL(TypeWSI), "Id", "Name", value);
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public ActionResult slAccountTypes(string elementId, string objectName, string value)
        {
            NMAccountNumbersBL BL = new NMAccountNumbersBL();
            NMAccountNumbersWSI WSI = new NMAccountNumbersWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.AccountNumber.ForReceipt = objectName;
            WSI.AccountNumber.IsDiscontinued = false;
            if (!String.IsNullOrEmpty(value))
            {
                if (value.Contains("711"))
                    WSI.AccountNumber.ParentId = "711";
                else if (value.Contains("811"))
                    WSI.AccountNumber.ParentId = "811";
            }
            List<NMAccountNumbersWSI> list = BL.callListBL(WSI);

            //if(!String.IsNullOrEmpty(value))
            //    if(value == "711")
            //    foreach(NMAccountNumbersWSI itm in list)

            ViewData["List"] = new SelectList(list, "AccountNumber.Id", "AccountNumber.NameInVietnamese", value);
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public ActionResult slAccountForPayments(string elementId, string objectName, string value)
        {
            NMAccountNumbersBL BL = new NMAccountNumbersBL();
            NMAccountNumbersWSI WSI = new NMAccountNumbersWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.AccountNumber.ForPayment = objectName;
            WSI.AccountNumber.IsDiscontinued = false;
            if (!String.IsNullOrEmpty(value))
            {
                if (value.Contains("711"))
                    WSI.AccountNumber.ParentId = "711";
                else if (value.Contains("811"))
                    WSI.AccountNumber.ParentId = "811";
            }
            List<NMAccountNumbersWSI> list = BL.callListBL(WSI);

            //if(!String.IsNullOrEmpty(value))
            //    if(value == "711")
            //    foreach(NMAccountNumbersWSI itm in list)

            ViewData["List"] = new SelectList(list, "AccountNumber.Id", "AccountNumber.NameInVietnamese", value);
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public ActionResult slInvoiceTypes(string elementId, string objectName, string value)
        {
            NMAccountNumbersBL BL = new NMAccountNumbersBL();
            NMAccountNumbersWSI WSI = new NMAccountNumbersWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.AccountNumber.ForInvoice = objectName;
            WSI.AccountNumber.IsDiscontinued = false;
            ViewData["List"] = new SelectList(BL.callListBL(WSI), "AccountNumber.Id", "AccountNumber.NameInVietnamese", value);
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public ActionResult slGenders(string elementId)
        {
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public ActionResult ContentLanguagesForm(string mode, string data, string windowId)
        {
            NMTypesBL BL = new NMTypesBL();
            NMTypesWSI WSI = new NMTypesWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.ObjectName = "Languages";
            ViewData["Languages"] = BL.callListBL(WSI);
            ViewData["Mode"] = mode;
            ViewData["WindowId"] = windowId;
            ViewData["Data"] = data;
            return PartialView();
        }

        public ActionResult TranslationForm(string mode, string ownerId, string languageId, string windowId)
        {
            NMTranslationsBL BL = new NMTranslationsBL();
            NMTranslationsWSI WSI = new NMTranslationsWSI();
            WSI.Mode = "SEL_OBJ";
            WSI.Translation = new Translations();
            WSI.Translation.OwnerId = ownerId;
            WSI.Translation.LanguageId = languageId;
            ViewData["WSI"] = BL.callSingleBL(WSI);
            ViewData["Mode"] = mode;
            ViewData["LanguageId"] = languageId;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public ActionResult ImportFromExcelForm(string mode)
        {
            ViewData["Mode"] = mode;
            return PartialView();
        }

        public string ReadExcel(string mode, HttpPostedFileBase excelUpload)
        {
            string error = "";
            if (excelUpload != null)
            {
                try
                {
                    string path = "";
                    NMTranslationsBL TranslationBL = new NMTranslationsBL();
                    NMTranslationsWSI TranslationWSI;
                    NMCategoriesBL CategoryBL = new NMCategoriesBL();
                    NMCategoriesWSI CategoryWSI;
                    NMCategoriesWSI pcWSI;
                    NMProductsBL ProductBL = new NMProductsBL();
                    NMProductsWSI ProductWSI;
                    switch (mode)
                    {
                        case "Products":
                            path = Server.MapPath(@"~/Content/Data/ProductList.xls");
                            excelUpload.SaveAs(path);
                            var excel = new ExcelQueryFactory(path);
                            var data = from c in excel.Worksheet(0) select c;
                            
                            foreach (Row row in data)
                            {
                                ProductWSI = new NMProductsWSI();
                                ProductWSI.Mode = "SAV_OBJ";
                                ProductWSI.Product = new Products();
                                ProductWSI.Product.ProductCode = row["Mã sản phẩm"].Value.ToString();
                                //ProductWSI.Product.BarCode = row["Mã vạch"].Value.ToString();
                                if (!string.IsNullOrEmpty(row["Mã loại SP"].Value.ToString()))
                                    ProductWSI.Product.CategoryId = row["Mã loại SP"].Value.ToString();
                                //WSI.Product.TypeId = row["Mã kiểu sản phẩm"].Value.ToString();
                                //WSI.Product.GroupId = row["Mã nhóm"].Value.ToString();
                                if (!string.IsNullOrEmpty(row["Mã nhà sản xuất"].Value.ToString()))
                                    ProductWSI.Product.ManufactureId = row["Mã nhà sản xuất"].Value.ToString();
                                if (!string.IsNullOrEmpty(row["Mã ĐVT"].Value.ToString()))
                                    ProductWSI.Product.ProductUnit = row["Mã ĐVT"].Value.ToString();
                                try
                                {
                                    ProductWSI.Product.CostPrice = (double)row["Giá bán"].Value;
                                }
                                catch { }
                                try
                                {
                                    ProductWSI.Product.VATRate = (int)row["Thuế (%)"].Value;
                                }
                                catch { }
                                //ProductWSI.Product.Discount = (double)row["<%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %>"].Value;
                                try
                                {
                                    ProductWSI.Product.Discontinued = (bool)row["Ngưng bán"].Value;
                                }
                                catch { }
                                try
                                {
                                    ProductWSI.Product.IsNew = (bool)row["Mới?"].Value;
                                }
                                catch { }
                                try
                                {
                                    ProductWSI.Product.IsEmpty = (bool)row["Hết hàng?"].Value;
                                }
                                catch { }
                                ProductWSI.ActionBy = User.Identity.Name;
                                ProductWSI = ProductBL.callSingleBL(ProductWSI);
                                if (ProductWSI.WsiError == "")
                                {
                                    TranslationWSI = new NMTranslationsWSI();
                                    TranslationWSI.Mode = "SAV_OBJ";
                                    TranslationWSI.Translation = new Translations();
                                    TranslationWSI.Translation.OwnerId = ProductWSI.Product.ProductId;
                                    TranslationWSI.Translation.LanguageId = NMCommon.GetLanguageDefault();
                                    TranslationWSI.Translation.Name = row["Tên sản phẩm"].Value.ToString();
                                    TranslationWSI.Translation.Description = row["Mô tả"].Value.ToString();
                                    TranslationWSI.Translation.Warranty = row["Bảo hành"].Value.ToString();
                                    TranslationWSI = TranslationBL.callSingleBL(TranslationWSI);
                                }
                            }
                            break;
                        case "Categories":
                            path = Server.MapPath(@"~/Content/Data/CategoryList.xls");
                            excelUpload.SaveAs(path);
                            excel = new ExcelQueryFactory(path);
                            data = from c in excel.Worksheet(0) select c;
                            foreach (Row row in data)
                            {
                                CategoryWSI = new NMCategoriesWSI();
                                CategoryWSI.Mode = "SAV_OBJ";
                                CategoryWSI.Category = new Categories();
                                if (!string.IsNullOrEmpty(row["Thuộc loại"].Value.ToString()))
                                    CategoryWSI.Category.ParentId = row["Thuộc loại"].Value.ToString();
                                CategoryWSI.Category.ObjectName = "Products";
                                CategoryWSI.ActionBy = User.Identity.Name;
                                CategoryWSI = CategoryBL.callSingleBL(CategoryWSI);
                                if (CategoryWSI.WsiError == "")
                                {
                                    TranslationWSI = new NMTranslationsWSI();
                                    TranslationWSI.Mode = "SAV_OBJ";
                                    TranslationWSI.Translation = new Translations();
                                    TranslationWSI.Translation.OwnerId = CategoryWSI.Category.Id;
                                    TranslationWSI.Translation.LanguageId = NMCommon.GetLanguageDefault();
                                    TranslationWSI.Translation.Name = row["Tên loại"].Value.ToString();
                                    TranslationWSI.Translation.Description = row["Mô tả"].Value.ToString();
                                    TranslationWSI = TranslationBL.callSingleBL(TranslationWSI);
                                }
                            }
                            break;
                        case "Mix":
                            path = Server.MapPath(@"~/Content/Data/mix.xls");
                            excelUpload.SaveAs(path);
                            excel = new ExcelQueryFactory(path);
                            data = from c in excel.Worksheet(0) select c;
                            string code ="", name ="";                            
                            foreach (Row row in data)
                            {
                                 code = row["Mã"].Value.ToString();
                                 name =row["Tên hàng"].Value.ToString();
                                 if (code == "")
                                     break;
                                 if (code.Length != 12)
                                 {
                                     CategoryWSI = new NMCategoriesWSI();
                                     CategoryWSI.Mode = "SAV_OBJ";
                                     CategoryWSI.Category = new Categories();
                                     CategoryWSI.Category.CustomerCode = code;
                                     if (code.Length >= 3)
                                     {
                                         pcWSI = new NMCategoriesWSI();
                                         pcWSI.Mode = "SRC_OBJ";
                                         pcWSI.Category = new Categories();
                                         pcWSI.Category.CustomerCode = code.Substring(0, code.Length - 2);
                                         pcWSI.Category.ObjectName = "Products";
                                         pcWSI = CategoryBL.callListBL(pcWSI).FirstOrDefault();
                                         CategoryWSI.Category.ParentId = pcWSI.Category.Id;
                                     }
                                     CategoryWSI.Category.ObjectName = "Products";
                                     CategoryWSI.ActionBy = User.Identity.Name;
                                     CategoryWSI = CategoryBL.callSingleBL(CategoryWSI);
                                     if (CategoryWSI.WsiError == "")
                                     {
                                         TranslationWSI = new NMTranslationsWSI();
                                         TranslationWSI.Mode = "SAV_OBJ";
                                         TranslationWSI.Translation = new Translations();
                                         TranslationWSI.Translation.OwnerId = CategoryWSI.Category.Id;
                                         TranslationWSI.Translation.LanguageId = NMCommon.GetLanguageDefault();
                                         TranslationWSI.Translation.Name = name;
                                         //TranslationWSI.Translation.Description = row["Mô tả"].Value.ToString();
                                         TranslationWSI = TranslationBL.callSingleBL(TranslationWSI);
                                     }
                                 }
                                 else
                                 {
                                     ProductWSI = new NMProductsWSI();
                                     ProductWSI.Mode = "SAV_OBJ";
                                     ProductWSI.Product = new Products();
                                     ProductWSI.Product.ProductCode = code;

                                     pcWSI = new NMCategoriesWSI();
                                     pcWSI.Mode = "SRC_OBJ";
                                     pcWSI.Category = new Categories();
                                     pcWSI.Category.CustomerCode = code.Substring(0, code.Length - 5);
                                     pcWSI.Category.ObjectName = "Products";
                                     pcWSI = CategoryBL.callListBL(pcWSI).FirstOrDefault();

                                     ProductWSI.Product.CategoryId = pcWSI.Category.Id;
                                     if (!string.IsNullOrEmpty(row["Đv Vlsphh"].Value.ToString()))
                                         ProductWSI.Product.ProductUnit = row["Đv Vlsphh"].Value.ToString();
                                     ProductWSI.Product.TypeId = "1561";

                                     ProductWSI.ActionBy = User.Identity.Name;
                                     ProductWSI = ProductBL.callSingleBL(ProductWSI);
                                     if (ProductWSI.WsiError == "")
                                     {
                                         TranslationWSI = new NMTranslationsWSI();
                                         TranslationWSI.Mode = "SAV_OBJ";
                                         TranslationWSI.Translation = new Translations();
                                         TranslationWSI.Translation.OwnerId = ProductWSI.Product.ProductId;
                                         TranslationWSI.Translation.LanguageId = NMCommon.GetLanguageDefault();
                                         TranslationWSI.Translation.Name = name;
                                         //TranslationWSI.Translation.Description = row["Mô tả"].Value.ToString();
                                         //TranslationWSI.Translation.Warranty = row["Bảo hành"].Value.ToString();
                                         TranslationWSI = TranslationBL.callSingleBL(TranslationWSI);
                                     }
                                 }
                            }
                            break;

                    }
                }
                catch (Exception ex)
                {
                    error = "Lỗi!\n" + ex.Message;
                }
            }
            else
                error = "Không có tập tin nào được chọn!";
            return error;
        }

        public ActionResult OrderObjects()
        {
            NMCategoriesBL BL = new NMCategoriesBL();
            NMCategoriesWSI WSI = new NMCategoriesWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Category = new Categories();
            WSI.Category.ObjectName = "Products";
            ViewData["WSIs"] = BL.callListBL(WSI);

            return PartialView();
        }

        public JsonResult SaveOrderObjects(string data)
        {
            string error = "";
            string[] arr = data.Split(',');
            NMCategoriesBL BL = new NMCategoriesBL();
            NMCategoriesWSI WSI;
            for (int i = 0; i < arr.Length; i++)
            {
                try
                {
                    WSI = new NMCategoriesWSI();
                    WSI.Mode = "SEL_OBJ";
                    WSI.Category = new Categories();
                    WSI.Category.Id = arr[i];
                    WSI = BL.callSingleBL(WSI);
                    if (WSI.WsiError == "")
                    {
                        WSI.Mode = "SAV_OBJ";
                        WSI.Category.Ordinal = i;
                        WSI.ActionBy = User.Identity.Name;
                        WSI = BL.callSingleBL(WSI);
                    }
                }
                catch (Exception ex) { error = "Lỗi!\n" + ex.Message; }
            }
            return Json(error);
        }

        public JsonResult GetProductInfo(string barCode)
        {
            NMProductsBL BL = new NMProductsBL();
            NMProductsWSI WSI = new NMProductsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Product = new Products();
            WSI.Product.BarCode = barCode;
            WSI.StockId = ((NMCustomersWSI)Session["UserInfo"]).Customer.StockId;
            WSI.LanguageId = Session["Lang"].ToString();
            List<NMProductsWSI> WSIs = BL.callListBL(WSI);
            if (WSIs.Count > 0)
            {
                WSI = WSIs[0];
                return Json(new
                {
                    productId = WSI.Product.ProductId,
                    productName = (WSI.Translation == null) ? "" : NMCryptography.base64Encode(WSI.Translation.Name),
                    price = (WSI.PriceForSales == null) ? 0 : WSI.PriceForSales.Price,
                    quantity = 1,
                    tax = WSI.Product.VATRate,
                    discount = (WSI.PriceForSales == null) ? 0 : WSI.PriceForSales.Discount,
                    mode = "+",
                    error = ""
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = barCode + " không tồn tại!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult MasterImportForm()
        {
            return PartialView();
        }

        public ActionResult DetailImportForm()
        {
            return PartialView();
        }

        public ActionResult CostPriceHistory(string productId, string windowId)
        {
            ViewData["ProductId"] = productId;
            ViewData["WindowId"] = windowId;
            return PartialView();
        }

        public ActionResult Pagination(string currentPage, string totalPage)
        {
            ViewData["CurrentPage"] = currentPage;
            ViewData["TotalPage"] = totalPage;
            return PartialView();
        }

        public ActionResult PaginationJS(string id)
        {
            ViewData["Id"] = id;
            return PartialView();
        }

        public ActionResult slMultiUsers(string elementId)
        {
            NMCustomersBL BL = new NMCustomersBL();
            NMCustomersWSI WSI = new NMCustomersWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Customer = new Customers();
            WSI.Customer.GroupId = NMConstant.CustomerGroups.User;
            ViewData["WSIs"] = BL.callListBL(WSI);
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        //public JsonResult AddUserToSession(string id)
        //{
        //    List<string> UserIds = new List<string>();
        //    if (Session["UserIds"] != null)
        //    {
        //        UserIds = (List<string>)Session["UserIds"];
        //    }
        //    UserIds.Add(id);
        //    Session["UserIds"] = UserIds;
        //    return Json("");
        //}

        //public JsonResult RemoveUserFromSession(string id)
        //{
        //    List<string> UserIds = new List<string>();
        //    if (Session["UserIds"] != null)
        //    {
        //        UserIds = (List<string>)Session["UserIds"];
        //    }
        //    UserIds.Remove(id);
        //    Session["UserIds"] = UserIds;
        //    return Json("");
        //}

        public ActionResult cbbProjects(string id, string label, string elementId, string windowTitle)
        {
            if (elementId == null) elementId = "";
            ViewData["WindowTitle"] = windowTitle;
            ViewData["Id"] = id;
            ViewData["Label"] = label;
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public ActionResult cbbTasks(string id, string label, string elementId, string mode, string windowTitle)
        {
            if (elementId == null) elementId = "";
            ViewData["WindowTitle"] = windowTitle;
            ViewData["Id"] = id;
            ViewData["Label"] = label;
            if (string.IsNullOrEmpty(mode)) mode = "select";
            ViewData["Mode"] = mode;
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public ActionResult slMultiStages(string elementId)
        {
            NMStagesBL BL = new NMStagesBL();
            NMStagesWSI WSI = new NMStagesWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Stage = new Stages();
            WSI.Stage.TypeId = NMConstant.ProjectStages;
            ViewData["WSIs"] = BL.callListBL(WSI);
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public ActionResult cbbJobs(string id, string label, string elementId, string windowTitle)
        {
            if (elementId == null) elementId = "";
            ViewData["WindowTitle"] = windowTitle;
            ViewData["Id"] = id;
            ViewData["Label"] = label;
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public JsonResult GetJobsForCBB(string keyword, string mode)
        {
            string langId = Session["Lang"].ToString();

            NMJobsBL BL = new NMJobsBL();
            List<NMJobsWSI> WSIs = new List<NMJobsWSI>();
            NMJobsWSI WSI;
            WSI = new NMJobsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Filter.Keyword = keyword;
            try
            {
                WSI.Filter.Limit = int.Parse(Request.Params["limit"]);
            }
            catch
            {
                WSI.Filter.Limit = 0;
            }
            WSIs = BL.callListBL(WSI);
            if (mode == "select")
            {
                WSI = new NMJobsWSI();
                WSI.Job.Id = "Search";
                WSI.Job.Name = NEXMI.NMCommon.GetInterface("SEARCH", langId);
                WSIs.Add(WSI);
                
                WSI = new NMJobsWSI();                
                WSI.Job.Id = "CreateOrEdit";
                WSI.Job.Name = NEXMI.NMCommon.GetInterface("ADD_NEW", langId) + " ...";
                WSIs.Add(WSI);
            }
            return Json(WSIs.Select(i => new
            {
                Id = i.Job.Id,
                Name = i.Job.Name,
                Purpose = i.Job.Purpose,
                Criteria = i.Job.Criteria,
                WorkGuids = i.Job.WorkGuids
            }), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult AddStageToSession(string id)
        //{
        //    List<string> UserIds = new List<string>();
        //    if (Session["StageIds"] != null)
        //    {
        //        UserIds = (List<string>)Session["StageIds"];
        //    }
        //    UserIds.Add(id);
        //    Session["StageIds"] = UserIds;
        //    return Json("");
        //}

        //public JsonResult RemoveStageFromSession(string id)
        //{
        //    List<string> UserIds = new List<string>();
        //    if (Session["StageIds"] != null)
        //    {
        //        UserIds = (List<string>)Session["StageIds"];
        //    }
        //    UserIds.Remove(id);
        //    Session["StageIds"] = UserIds;
        //    return Json("");
        //}

        public ActionResult StageBar(string projectId, string current)
        {
            if (String.IsNullOrEmpty(projectId))
            {
                NMStagesBL BL = new NMStagesBL();
                NMStagesWSI WSI = new NMStagesWSI();
                WSI.Mode = "SRC_OBJ";
                ViewData["Stages"] = BL.callListBL(WSI).Select(i => i.Stage).ToList();
            }
            else
            {
                NMProjectsBL BL = new NMProjectsBL();
                NMProjectsWSI WSI = new NMProjectsWSI();
                WSI.Mode = "SEL_OBJ";
                WSI.Project = new Projects();
                WSI.Project.ProjectId = projectId;
                ViewData["Stages"] = BL.callSingleBL(WSI).Stages.ToList();
            }
            ViewData["Current"] = current;
            return PartialView();
        }

        public ActionResult slStages(string elementId, string current, string projectStage)
        {
            if (projectStage == null) projectStage = "";
            NMStagesBL BL = new NMStagesBL();
            NMStagesWSI WSI = new NMStagesWSI();
            WSI.Mode = "SRC_OBJ";
            if (projectStage != "")
                ViewData["Stages"] = new SelectList(BL.callListBL(WSI).Select(i => i.Stage).Where(i => !i.Folded && projectStage.IndexOf(i.StageId) != -1), "StageId", "StageName", current);
            else
                ViewData["Stages"] = new SelectList(BL.callListBL(WSI).Select(i => i.Stage).Where(i => !i.Folded), "StageId", "StageName", current);
            ViewData["ElementId"] = elementId;
            return PartialView();
        }

        public ActionResult slParameters(string elementId, string objectName, string current)
        {
            NMParametersBL BL = new NMParametersBL();
            NMParametersWSI WSI = new NMParametersWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.ObjectName = objectName;
            List<NMParametersWSI> WSIs = BL.callListBL(WSI);
            if (string.IsNullOrEmpty(current))
            {
                try
                {
                    current = WSIs.Select(i => i).Where(i => i.Description != null).FirstOrDefault().Id;
                }
                catch { }
            }
            ViewData["WSIs"] = new SelectList(WSIs, "Id", "Name", current);
            ViewData["ElementId"] = elementId;
            ViewData["ObjectName"] = objectName;

            return PartialView();
        }

        public ActionResult cbbProducts(string elementId, string value, string label)
        {
            ViewData["Value"] = value;
            ViewData["Label"] = label;
            ViewData["ElementId"] = elementId;
            return PartialView();
        }


        public ActionResult FilterUC(string objectName, string holderText)
        {
            ViewData["ObjectName"] = objectName;
            ViewData["HolderText"] = holderText;
            return PartialView();
        }

        public ActionResult cbbProduct4Report(string elementId, string value, string label)
        {
            ViewData["Value"] = value;
            ViewData["Label"] = label;
            ViewData["ElementId"] = elementId;
            return PartialView();
        }
    }
}
