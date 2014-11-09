// NMConstant.cs

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
using System.Text;

namespace NEXMI
{
    
    public enum CreateMode
    {
        NEW,
        COPY
    }

    public class NMConstant
    {
        public static class CustomerGroups
        {
            public static String Customer = "G001";
            public static String Supplier = "G002";
            public static String Manufacturer = "G003";
            public static String User = "G004";
        }

        public static class CustomerTypes
        {
            public static string Individual = "CUS01";
            public static string Enterprise = "CUS02";
            public static string ContactPerson = "CUS03";
        }

        public static class SOrderGroups
        {
            public static string Sale = "GSO01";
            public static string Lease = "GSO02";
        }

        public static class ProductGroups
        {
            public static string Product = "PG01";
            public static string Material = "PG02";
            public static string SemiProduct = "PG03";
            public static string Tool = "PG04";
            public static string Property = "PG05";
            public static string Devices = "PG06";
        }

        public static class ProductTypes
        {
            public static string Consumable = "PT01";
            public static string Service = "PT02";
        }

        public static class MessageTypes
        {
            public static string Message = "MES01";
            public static string SysLog = "MES02";
            public static string UserNote = "MES03";
            public static string Comment = "MES04";
            public static string SMS = "MES05";
        }

        public static class PurchaseInvoiceType
        {
            public static String Purchase = "PIS01";
            public static String Semi_Product = "PIS02";
            public static String Promotion = "PIS03";
        }

        public static class SalesInvoiceType
        {
            public static String Sales = "SIS01";
            public static String Semi_Product = "SIS02";
            public static String Promotion = "SIS03";
        }

        public static class ImportType
        {
            public static String Direct = "IMP01"; //Nhập trực tiếp từ nhà cung cấp
            public static String Indirect = "IMP02"; //Nhập gián tiếp từ nhà môi giới
            public static String Return = "IMP03"; //Khách trả hàng
            public static String Transfer = "IMP04"; //Chuyển kho
        }

        public static class ExportType
        {
            public static String ForCustomers = "EXP01"; //Xuất cho khách hàng
            public static String ForAgents = "EXP02"; //Xuất cho đại lý
            public static String ForProduce = "EXP03"; //Xuất sản xuất
            public static String Return = "EXP04"; //Trả hàng cho nhà cung cấp
            public static String Transfer = "EXP05"; //Chuyển kho
            public static String Destroy = "EXP06"; //Xuất hủy
            public static String Lend = "EXP07"; //Xuất hủy
        }

        public static class PaymentType
        {
            public static String Purchase = "PAY01"; //Trả tiền mua hàng
            public static String Repay = "PAY02"; //Trả nợ cho nhà cung cấp
            public static String Advance = "PAY03"; //Ứng tiền mua hàng
            public static String Refund = "PAY04"; //Hoàn tiền cho khách
        }

        public static class ReceiptStatuses
        {
            public static String Draft = "RECST01";
            public static String Approved = "RECST02";
            public static String Done = "RECST03";
            public static String Cancelled = "RECST00";
        }

        public static class PaymentMethod
        {
            public static String Cash = "PAY01"; 
            public static String Bank = "PAY02"; 
        }

        public static class ReceiptType
        {
            public static String Sales = "REC01"; //Thu tiền bán hàng
            public static String Debt = "REC02"; //Thu nợ của khách hàng
            public static String Deposit = "REC03"; //Thu đặt cọc mua hàng
        }

        public static class PaymentStatuses
        {
            public static String Draft = "PAYST01";
            public static String Approved = "PAYST02";
            public static String Done = "PAYST03";
            public static String Cancelled = "PAYST00";
        }

        public static class StockType
        {
            public static String Retail = "STO01";//Kho bán lẻ
            public static String Whole = "STO02";//Kho bán sỉ
        }

        public static class DocumentTypes
        {
            public static String Introduce = "DOC01";
            public static String News = "DOC02";
            public static String Rules = "DOC03";
            public static String Supports = "DOC04";
            public static String Recruits = "DOC05";
            public static String WorkGuide = "DOC06";
            public static String Marketings = "DOC07";
        }

        public static class SOType
        {
            public static String Quotation = "SO01"; //Báo giá
            public static String SalesOrder = "SO02"; //Đơn hàng
            public static String POS = "SO03"; //PointOfSale
        }

        public static class SOStatuses
        {
            public static String Canceled = "SO000"; //Đã hủy
            public static String Draft = "SO001"; //Bản nháp
            public static String Sent = "SO002"; //Đã gởi cho khách hàng            
            public static String Order = "SO003"; //Là đơn hàng
            public static String Delivery = "SO004"; // Giao hàng
            public static String Invoice = "SO005"; // Xuất hóa đơn
            public static String Done = "SO006"; //Xong
            public static String Approval = "SO007"; //Duyệt
        }

        public static class LOStatuses
        {
            public static String Canceled = "LO000"; //Đã hủy
            public static String Draft = "LO001"; //Bản nháp
            public static String Approval = "LO002"; //Duyệt
            public static String Sent = "LO003"; //Đã gởi cho khách hàng            
            public static String Order = "LO004"; //Là đơn hàng
            public static String Setup = "LO005"; // Giao hàng
            public static String Maintain = "LO006"; // Xuất hóa đơn
            public static String Done = "LO007"; //Xong
            
        }

        public static class POType
        {
            public static String Quotation = "PO01"; //Báo giá
            public static String PurchaseOrder = "PO02"; //Đơn hàng
        }

        public static class POStatuses
        {
            public static String Canceled = "PO000"; //Đã hủy
            public static String Draft = "PO001"; //Bản nháp
            public static String Sent = "PO002"; //Đã gởi cho khách hàng            
            public static String Order = "PO003"; //Là đơn hàng
            public static String Inventory = "PO004"; //nhập kho
            public static String Invoice = "PO005"; // Xuất hóa đơn
            public static String Done = "PO006"; //Xong
        }

        public static string ProjectStages = "PJST01";

        public static class EXStatuses
        {
            public static String Draft = "EX001"; //Nháp
            public static String Ready = "EX002"; //Sẵn sàng để giao
            public static String Delivered = "EX003"; //Đã giao
            public static String Canceled = "EX000"; //Đã hủy
        }

        public static class IMStatuses
        {
            public static String Draft = "IM001"; //Nháp
            public static String Ready = "IM002"; //Sẵn sàng để nhập
            public static String Imported = "IM003"; //Đã nhập
            public static String Canceled = "IM000"; //Đã hủy
        }

        public static class SIStatuses
        {
            public static String Canceled = "SI000"; //Hủy
            public static String Draft = "SI001"; //Mới
            public static String Debit = "SI002"; //Đang mở
            public static String Paid = "SI003"; //Đã thanh toán
            public static String Finish = "SI004"; //Đã ghi sổ nợ
        }

        public static class PIStatuses
        {
            public static String Canceled = "PI000"; //Hủy
            public static String Draft = "PI001"; //Mới
            public static String Debit = "PI002"; //Đang mở chưa thanh toán
            public static String Paid = "PI003"; //Đã thanh toán
            public static String Finish = "PI004"; //Đã ghi sổ nợ
        }

        public static class ProjectStatuses
        {
            public static String InProgress = "PJ001";
            public static String Pending = "PJ002";
            public static String Cancelled = "PJ003";
            public static String Closed = "PJ004";
        }

        public static class TaskStatuses
        {
            public static String InProgress = "TASK01";
            public static String Done = "TASK02";
            public static String Blocked = "TASK03"; 
        }

        public static class IssueStatuses
        {
            public static String InProgress = "ISSU01";
            public static String Done = "ISSU02";
            public static String Blocked = "ISSU03";
        }

        public static class StageStatuses
        {
            public static String New = "STG01";
            public static String InProgress = "STG02";
            public static String Pending = "STG03";
            public static String Done = "STG04";
            public static String Cancelled = "STG05";
        }

        public static class RequirementStatuses
        {
            public static String Draft = "RQS01";
            public static String Sent = "RQS02";
            public static String Approved = "RQS03";
            public static String NotApproved = "RQS04";
            public static String Cancelled = "RQS00";
        }

        public static class MOStatuses
        {
            public static String Cancelled = "MO00";
            public static String Draft = "MO01";
            public static String InProgress = "MO02";
            //public static String Pending = "PJ002";
            public static String Closed = "MO03";
        }

        public static class DocStatuses
        {
            public static String Cancelled = "DOC00";
            public static String Draft = "DOC01";
            public static String InProgress = "DOC02";
            public static String Finished = "DOC03";
        }

        public static class Functions
        {
            public static string Customer = "AR001";
            public static string SI = "AR002";
            public static string SIB = "AR003";
            public static string AccountReceivable = "AR004";
            public static string Quotation = "AR005";
            public static string SalesOrder = "AR006";
            public static string LeaseOrder = "AR014";

            public static string PI = "AP001";
            public static string Suppilers = "AP002";
            public static string PIB = "AP003";
            public static string AccountPayable = "AP004";
            public static string PQuotation = "AP005";
            public static string PO = "AP006";
            public static string Requirement = "AP008";

            public static string ProductCategories = "DR001";
            public static string Products = "DR002";
            public static string Manufacturers = "DR003";
            public static string Currencies = "DR004";
            public static string Areas = "DR005";
            public static string Shifts = "DR006";
            public static string Units = "DR007";
            public static string Banks = "DR008";

            public static string Receipts = "GL001";
            public static string Payments = "GL002";
            public static string CloseMonth = "GL006";
            public static string CashBalances = "GL004";

            public static string Users = "MG001";
            public static string Permission = "MG002";

            public static string Stock = "ST001";
            public static string Import = "ST002";
            public static string Export = "ST003";
            public static string ProductInStock = "ST004";
            public static string TransferProduct = "ST005";
            public static string UpdateStock = "ST006";
            public static string CloseInventory = "ST010";

            public static string Project = "PM001";
            public static string Task = "PM002";
            public static string Issue = "PM003";
            public static string Jobs = "PM004";
            public static string TaskCard = "PM005";

            public static string EmailTemplate = "UT001";
            public static string Images = "UT002";
            public static string Documents = "UT003";
            public static string SendEmail = "UT004";

            public static string Introduces = "CMS01";
            public static string News = "CMS02";
            public static string Rule = "CMS03";
            public static string Interface = "CMS04";
            public static string Message = "CMS05";

            public static string IEIReport = "RP014";

            public static string Plans = "PL001";

            public static string ManufactureOrders = "MRP01";

        }

        public static class Permissions
        {
            public static string Select = "Select";
            public static string Insert = "Insert";
            public static string Update = "Update";
            public static string Delete = "Delete";
            public static string Approval = "Approval";
            public static string ViewAll = "ViewAll";
            public static string Calculate = "Calculate";
            public static string History = "History";

            public static string ViewPrice = "ViewPrice";
            public static string Export = "Export";
            public static string PPrint = "PPrint";
            public static string Duplicate = "Duplicate";
        }

        public static class CreateInvoice
        {
            public static string OnDemand = "CI01";
            public static string OnDeliveryOrder = "CI02";
            public static string BeforeDelivery = "CI03";
        }

        public static class FileTypes
        {
            public static string Images = "IMG";
            public static string Docs = "DOC";
            public static string Videos = "VID";
        }

        public static class SystemParamType
        {
            public static string ByActions = "BYACT";
            public static string Birthdays = "BIRTH";
            public static string Holidays = "HOL";
            public static string Announcements = "ANN";
            public static string Services = "SERV";
            public static string DailyWorks = "DAILY";
        }

        public static class ObjectNames
        {
            public static string Marketing = "MKT";
            public static string SalesOrders = "SalesOrders";
            public static string KindOfIndustry = "KOI";
            public static string KindOfEnterprise = "KOE";
        }

        public static class ShippingPolicy
        {
            public static string None = "SP00";
            public static string Once = "SP01";
            public static string Many = "SP02";
        }

        public static class DeliveryMethod
        {
            public static string None = "DM00";
            public static string Once = "DM01";
            public static string Many = "DM02";
        }

        public static class Settings
        {
            // cho tinh ton kho
            public static string INVENTORY_COST = "INVENTORY_COST";
            public static string BY_NAME = "BY_NAME";
            public static string FIFO = "FIFO";
            public static string LIFO = "LIFO";
            public static string WA = "WA";

            // cho tinh gia tri thanh toan
            public static string DISCOUNT_ON_INVOICE = "DISCOUNT_ON_INVOICE";

            // cho xuat kho 
            public static string EXPORT_FROM_ORDER = "EXPORT_FROM_ORDER";
            public static string EXPORT_ON_INVOICE = "EXPORT_ON_INVOICE";
            public static string SELECT_STORE_BY_USER_4_EXPORT = "SELECT_STORE_BY_USER_4_EXPORT";
                        
            // hoa don ban hang
            public static string INVOICE_FROM_EXPORT = "INVOICE_FROM_EXPORT";
            public static string INVOICE_FROM_ORDER = "INVOICE_FROM_ORDER";
            public static string MANUAL_INPUT_PRICE = "MANUAL_INPUT_PRICE";
            public static string RECEIPT_ON_INVOICE = "RECEIPT_ON_INVOICE";

            // Cho phép hạch toán chi phí mua hàng vào giá trị hàng tồn kho
            public static string PURCHASE_COST_TO_PRICE = "PURCHASE_COST_TO_PRICE";
            
            // quan ly so serial
            public static string SERIALNUMBER = "SERIALNUMBER";            

            // cho nhap kho
            public static string IMPORT_FROM_ORDER = "IMPORT_FROM_ORDER";
            public static string IMPORT_ON_PI = "IMPORT_ON_PI";
            public static string SELECT_STORE_BY_USER_4_IMPORT = "SELECT_STORE_BY_USER_4_IMPORT";

            // hoa don mua hang
            public static string PI_FROM_IMPORT = "PI_FROM_IMPORT";
            public static string PI_FROM_ORDER = "PI_FROM_ORDER";

            // thanh toan theo hoa don
            public static string PAYMENT_ON_PINVOICE = "PAYMENT_ON_PINVOICE";

            // thu tien theo hoa don
            public static string RECEIPT_ON_SINVOICE = "RECEIPT_ON_SINVOICE";

            // gia cho cua hang
            public static string PRICE_BY_STORE = "PRICE_BY_STORE";

            // lua chọn bán hàng
            // cho phep nhap gia ban
            public static string PRICE_BY_USER = "PRICE_BY_USER";
            public static string SELECT_STORE_BY_USER = "SELECT_STORE_BY_USER";
            public static string SELECT_SALER_BY_USER = "SELECT_SALER_BY_USER";
            public static string APPROVAL_SALE_QUOTE = "APPROVAL_SALE_QUOTE";


            // lựa chọn cho bán hàng
            public static string SELECT_BUYER_BY_USER = "SELECT_BUYER_BY_USER";
            public static string SELECT_STORE_BY_USER_4_PO = "SELECT_STORE_BY_USER_4_PO";
            public static string APPROVAL_PO = "APPROVAL_PO";


            // kiểu lập đơn hàng
            public static string MULTI_LINE_DETAIL_ORDER = "MULTI_LINE_DETAIL_ORDER";

            // cách hiển thị danh sách khách hàng
            public static string SHOW_COMPANY_ONLY = "SHOW_COMPANY_ONLY";

            //số ngày mặc định để lấy danh sách
            public static string DF_LOAD_LIST_DAYS = "DF_LOAD_LIST_DAYS";

            //số ngày giao hàng mặc định 
            public static string DF_DELIVERY_DAYS = "DF_DELIVERY_DAYS";

            //số ngày thanh toán mặc định
            public static string DF_PAYMENT_DAYS = "DF_PAYMENT_DAYS";
        }

        public static class JobsStatus
        {
            public static string Cancel = "JOB00";
            public static string Draft = "JOB01";
            public static string Update = "JOB02";
            public static string Finish = "JOB03";
        }
    }
    
}
