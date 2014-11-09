using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NEXMI;
using NHibernate;

namespace TestDTO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ISession Session = SessionFactory.GetNewSession();
            ITransaction tx = Session.BeginTransaction();
            ProductBOMsAccesser Accesser = new ProductBOMsAccesser(Session);
            ProductBOMs obj = new ProductBOMs();
            obj.ParentId = "P13-001";
            obj.ProductId = "P1013-001";
            obj.Quantity = 1;
            Accesser.InsertProductBOMs(obj);
            tx.Commit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //NMUsersBL BL = new NMUsersBL();
            //NMUsersWSI wsi = new NMUsersWSI();
            //wsi.Mode = "SRC_OBJ";
            //List<NMUsersWSI> List = BL.callListBL(wsi);
            //MessageBox.Show(List.Count.ToString());

            ISession session = SessionFactory.GetNewSession();
            SettingsAccesser Accesser = new SettingsAccesser(session);
            MessageBox.Show(Accesser.GetAllSettings(true).Count.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            NMProductUnitsWSI WSI = new NMProductUnitsWSI();
            NMProductUnitsBL BL = new NMProductUnitsBL();
            WSI.Mode = "SRC_OBJ";
            List<NMProductUnitsWSI> WSIs = BL.callListBL(WSI);
            MessageBox.Show(WSIs.Count.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //NMShiftsBL BL = new NMShiftsBL();
            //NMShiftsWSI WSI = new NMShiftsWSI();
            //WSI.Mode = "SRC_OBJ";
            //List<NMShiftsWSI> WSIs = BL.callListBL(WSI);
            //MessageBox.Show(WSIs.Count.ToString());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            NMProductsBL BL = new NMProductsBL();
            NMProductsWSI WSI = new NMProductsWSI();
            WSI.Mode = "SRC_OBJ";
            //WSI.CategoryId = "";
            List<NMProductsWSI> WSIs = BL.callListBL(WSI);
            MessageBox.Show(WSIs.Count.ToString());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            NMHandingOverBL BL = new NMHandingOverBL();
            NMHandingOverWSI WSI = new NMHandingOverWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.DeliverId = "U00001";
            WSI.ReceiveId = "U00002";
            WSI.DateAndTime = DateTime.Now.ToString();
            WSI = BL.callSingleBL(WSI);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            NMProjectsBL BL = new NMProjectsBL();
            NMProjectsWSI WSI = new NMProjectsWSI();
            WSI.Mode = "SRC_OBJ";
            List<NMProjectsWSI> WSIs = BL.callListBL(WSI);
            MessageBox.Show(WSIs.Count.ToString());
        }

        private void button8_Click(object sender, EventArgs e)
        {
            NMProjectStagesBL BL = new NMProjectStagesBL();
            NMProjectStagesWSI WSI = new NMProjectStagesWSI();
            WSI.Mode = "SRC_OBJ";
            List<NMProjectStagesWSI> WSIs = BL.callListBL(WSI);
            MessageBox.Show(WSIs.Count.ToString());
        }

        private void button9_Click(object sender, EventArgs e)
        {
            NMProjectsBL BL = new NMProjectsBL();
            NMProjectsWSI WSI = new NMProjectsWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Project = new Projects();
            WSI.Project.ProjectId = ""; WSI.Project.ProjectName = "aaaaaaaa";
            WSI.Project.ManagedBy = "U0001"; WSI.Project.CustomerId = "C000013";
            //WSI.Project.PlannedTime = DateTime.Parse("01/18/2013");
            //WSI.Project.TotalTime = DateTime.Parse("01/18/2013");
            //WSI.Project.TimeSpent = DateTime.Parse("01/18/2013");
            //WSI.Project.Progress = 0; WSI.Project.Status = 0; WSI.Project.StartDate = DateTime.Parse("01/18/2013");
            WSI.Project.EndDate = DateTime.Parse("01/19/2013");
            WSI.Project.Escalation = "";
            WSI.Project.TimeSheets = true;
            WSI.ActionBy = "U0001";
            WSI = BL.callSingleBL(WSI);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //NMTasksBL BL = new NMTasksBL();
            //NMTasksWSI WSI = new NMTasksWSI();
            //WSI.Mode = "SAV_OBJ";
            //WSI.Task = new Tasks();
            //WSI.Task.TaskId = ""; WSI.Task.TaskName = "aaaaaaaa";
            //WSI.Task.AssignedTo = "U0001"; WSI.Task.CustomerId = "C000013";
            //WSI.Task.EndDate = DateTime.Parse("01/18/2013");
            //WSI.Task.StartDate = DateTime.Parse("01/18/2013");
            //WSI.Task.Progress = 0;
            //WSI.ActionBy = "U0001";
            //WSI.Details = new List<TaskDetails>();
            //WSI = BL.callSingleBL(WSI);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            AutomaticValuesAccesser Accesser = new AutomaticValuesAccesser(SessionFactory.GetNewSession());
            MessageBox.Show(Accesser.AutoGenerateId("SalesOrders"));
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //textBox2.Text = NMCryptography.base64Decode(textBox1.Text);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            InterfaceAccesser acc = new InterfaceAccesser();
            MessageBox.Show(acc.GetAllInterface(true).Count.ToString());
        }

        private void button14_Click(object sender, EventArgs e)
        {
            MessageBox.Show(DateTime.Parse("12/05/2013").AddDays(-3).Day.ToString());
        }

        private void button15_Click(object sender, EventArgs e)
        {
            
            NMLocationsWSI LocationWSI = new NMLocationsWSI();
            NMLocationsBL LocationBL = new NMLocationsBL();
            LocationWSI.Mode = "SRC_OBJ";
            List<NMLocationsWSI> LocationWSIs = LocationBL.callListBL(LocationWSI);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            NMDocumentsBL BL = new NMDocumentsBL();
            NMDocumentsWSI WSI = new NMDocumentsWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.Document = new Documents();
            WSI.Document.PredefinedRFID = "new";
            List<NMDocumentsWSI> lst = BL.callListBL(WSI);
        }

        private void btDocClass_Click(object sender, EventArgs e)
        {
            List<string> c = NMCommon.GetDocumentClass();
            string str ="";
            foreach (var t in c)
                str += t + "\n";

            MessageBox.Show(str);

        }

        private void btDepartment_Click(object sender, EventArgs e)
        {
            NMDepartmentsBL BL = new NMDepartmentsBL();
            NMDepartmentsWSI WSI = new NMDepartmentsWSI();
            WSI.Mode = "SAV_OBJ";
            WSI.Department.Name = "ABC";
            WSI = BL.callSingleBL(WSI);
        }

        private void btPermissionByUGroup_Click(object sender, EventArgs e)
        {
            NMPermissionsBL BL = new NMPermissionsBL();
            NMPermissionsWSI wsi = new NMPermissionsWSI();

            wsi.UserGroupId = "BP-02";
            wsi.Mode = "SRC_OBJ";
            List<NMPermissionsWSI> lst = BL.callListBL(wsi);

        }

        private void btn_PIS_Click(object sender, EventArgs e)
        {
            NMProductInStocksBL BL = new NMProductInStocksBL();
            NMProductInStocksWSI WSI = new NMProductInStocksWSI();
            WSI.Mode = "SRC_OBJ";
            WSI.PIS = new ProductInStocks();
            WSI.PIS.StockId = "ST-09";
            List<NMProductInStocksWSI> WSIs = BL.callListBL(WSI);
            List<ProductInStocks> list = new List<ProductInStocks>();
            foreach (var itm in WSIs)
                list.Add(itm.PIS);
            dataGridView1.DataSource = list;
        }
    }
}
