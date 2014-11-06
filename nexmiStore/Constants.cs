using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.IO;
using System.Drawing.Imaging;
using System.Configuration;

namespace NEXMI
{
    //public class Constants
    //{
    //    public static string defaultLanguage;
    //    public static int PageSize = 15;

    //    public static NMCustomersWSI Company;

    //    public Constants()
    //    {
    //        NMCustomersBL BL = new NMCustomersBL();
    //        NMCustomersWSI WSI = new NMCustomersWSI();
    //        WSI.Mode = "SEL_OBJ";
    //        WSI.Customer = new Customers();
    //        WSI.Customer.CustomerId = "COMPANY";
    //        WSI = BL.callSingleBL(WSI);
    //        if (WSI.Customer != null)
    //        {
    //            //companyCode = WSI.Customer.Code;
    //            //companyName = WSI.Customer.CompanyNameInVietnamese;
    //            //companyAddress = WSI.Customer.Address;
    //            //companyPhoneNumber = WSI.Customer.Telephone;
    //            //faxNumber = WSI.Customer.FaxNumber;
    //            //companyEmail = WSI.Customer.EmailAddress;
    //            //emailPassword = WSI.Customer.EmailPassword;
    //            //taxCode = WSI.Customer.TaxCode;
    //            //website = WSI.Customer.Website;
    //            //logo = WSI.Customer.Avatar;
    //            //if (WSI.ContactPersons.Count > 0)
    //            //{
    //            //    managerName = WSI.ContactPersons[0].CompanyNameInVietnamese;
    //            //    managerTitle = WSI.ContactPersons[0].JobPosition;
    //            //}
    //            Company = WSI;
    //        }
    //        PageSize = 15;
    //        defaultLanguage = NEXMI.NMCommon.GetLanguageDefault();
    //    }
    //}
}
