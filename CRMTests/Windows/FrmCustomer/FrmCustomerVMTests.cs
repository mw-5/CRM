using Microsoft.VisualStudio.TestTools.UnitTesting;
using CRM.Windows.FrmCustomer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRMTests.TestCases;
using CRM.Model;
using System.Threading;
using System.Data;

namespace CRM.Windows.FrmCustomer.Tests
{
    [TestClass()]
    public class FrmCustomerVMTests
    {        
        [TestMethod()]
        public void NewSubmitCommandTest()
        {
            Model.Model m = Model.Model.GetModel();
            DefTblCustomers def = new DefTblCustomers();

            // set up model state
            TestCase.PopulateTestData();
            m.LoadCustomers();
            Thread.Sleep(1000);
            m.Id4Edit = null;

            // create view model
            FrmCustomerVM vm = new FrmCustomerVM(null);

            // check that no data has been loaded
            Assert.AreEqual(vm.Cid, 0);
            Assert.AreEqual(vm.Company, null);
            Assert.AreEqual(vm.Address, null);
            Assert.AreEqual(vm.Zip, null);
            Assert.AreEqual(vm.City, null);
            Assert.AreEqual(vm.Country, null);
            Assert.AreEqual(vm.ContractId, null);
            Assert.AreEqual(vm.ContractDate, null);

            // enter data
            vm.Company = TestCustomer.company;
            vm.Address = TestCustomer.address;
            vm.Zip = TestCustomer.zip;
            vm.City = TestCustomer.city;
            vm.Country = TestCustomer.country;
            vm.ContractId = TestCustomer.contractId;
            vm.ContractDate = TestCustomer.contractDate;

            vm.SubmitCommand.Execute(null);

            Thread.Sleep(1000);
            m.LoadCustomers();
            Thread.Sleep(1000);

            int cid = int.Parse(m.ExecuteScalar(String.Format("SELECT Max({0}) FROM {1};", def.Cid.Name, def.TblName)).ToString());

            DataRow dr = (from d in m.Customers.Table.AsEnumerable()
                              where d[def.Cid.Name].Equals(cid)
                              select d).First();

            // check new entries
            Assert.AreEqual(dr[def.Cid.Name], cid);
            Assert.AreEqual(dr[def.Company.Name], TestCustomer.company);
            Assert.AreEqual(dr[def.Address.Name], TestCustomer.address);
            Assert.AreEqual(dr[def.Zip.Name], TestCustomer.zip);
            Assert.AreEqual(dr[def.City.Name], TestCustomer.city);
            Assert.AreEqual(dr[def.Country.Name], TestCustomer.country);
            Assert.AreEqual(dr[def.ContractId.Name], TestCustomer.contractId);
            Assert.AreEqual(dr[def.ContractDate.Name], TestCustomer.contractDate);

            // clean up
            m.ExecuteActionQuery(String.Format("DELETE FROM {0} WHERE {1} = {2};", def.TblName, def.Cid.Name, cid));
            TestCase.CleanUp();
        }

        [TestMethod()]
        public void EditSubmitCommandTest()
        {
            Model.Model m = Model.Model.GetModel();
            DefTblCustomers def = new DefTblCustomers();

            // set up model state
            TestCase.PopulateTestData();
            m.LoadCustomers();
            Thread.Sleep(1000);
            m.Id4Edit = TestCustomer.cid.ToString();

            // create view model
            FrmCustomerVM vm = new FrmCustomerVM(null);

            // check if data has been loaded correctly
            Assert.AreEqual(vm.Cid, TestCustomer.cid);
            Assert.AreEqual(vm.Company, TestCustomer.company);
            Assert.AreEqual(vm.Address, TestCustomer.address);
            Assert.AreEqual(vm.Zip, TestCustomer.zip);
            Assert.AreEqual(vm.City, TestCustomer.city);
            Assert.AreEqual(vm.Country, TestCustomer.country);
            Assert.AreEqual(vm.ContractId, TestCustomer.contractId);
            Assert.AreEqual(vm.ContractDate, TestCustomer.contractDate);

            // edit data
            vm.Company = TestCustomer.company + "2";
            vm.Address = TestCustomer.address + "2";
            vm.Zip = TestCustomer.zip + "2";
            vm.City = TestCustomer.city + "2";
            vm.Country = TestCustomer.country + "2";
            vm.ContractId = TestCustomer.contractId + "2";
            vm.ContractDate = TestCustomer.contractDate.Value.AddDays(2);

            vm.SubmitCommand.Execute(null);

            Thread.Sleep(1000);
            m.LoadCustomers();
            Thread.Sleep(1000);

            DataRow dr = (from d in m.Customers.Table.AsEnumerable()
                          where d[def.Cid.Name].Equals(TestCustomer.cid)
                          select d).First();

            // check modified entries
            Assert.AreEqual(dr[def.Cid.Name], TestCustomer.cid);
            Assert.AreEqual(dr[def.Company.Name], TestCustomer.company + "2");
            Assert.AreEqual(dr[def.Address.Name], TestCustomer.address + "2");
            Assert.AreEqual(dr[def.Zip.Name], TestCustomer.zip + "2");
            Assert.AreEqual(dr[def.City.Name], TestCustomer.city + "2");
            Assert.AreEqual(dr[def.Country.Name], TestCustomer.country + "2");
            Assert.AreEqual(dr[def.ContractId.Name], TestCustomer.contractId + "2");
            Assert.AreEqual(dr[def.ContractDate.Name], TestCustomer.contractDate.Value.AddDays(2));

            TestCase.CleanUp();
        }
    }
}