using Microsoft.VisualStudio.TestTools.UnitTesting;
using CRM.Pages.Cockpit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRMTests.TestCases;

namespace CRM.Pages.Cockpit.Tests
{
    [TestClass()]
    public class CockpitVMTests
    {
        CockpitVM vm;

        [TestInitialize]
        public void init()
        {
            vm = new CockpitVM();
        }

        [TestMethod()]
        public void UpdateCockpitTest()
        {
            TestCase.PopulateTestData();

            // simulate user input
            vm.CidSearchBox = TestCustomer.cid;
            vm.UpdateCockpit();

            System.Threading.Thread.Sleep(1000); // wait for async operations
            
            Assert.AreEqual(vm.CidSearchBox, TestCustomer.cid);
            Assert.AreEqual(vm.Cid, TestCustomer.cid);
            Assert.AreEqual(vm.Company, TestCustomer.company);
            Assert.AreEqual(vm.Address, TestCustomer.address);
            Assert.AreEqual(vm.Zip, TestCustomer.zip);
            Assert.AreEqual(vm.City, TestCustomer.city);
            Assert.AreEqual(vm.Country, TestCustomer.country);
            Assert.AreEqual(vm.ContractId, TestCustomer.contractId);
            Assert.AreEqual(vm.ContractDate, TestCustomer.contractDate);
                     
            Assert.IsTrue(vm.Notes.Count == 1);
            Assert.IsTrue(vm.ContactPersons.Count == 1);            

            TestCase.CleanUp();
        }

        [TestMethod()]
        public void SearchCommandTest()
        {
            TestCase.PopulateTestData();

            vm.Cid = TestCustomer.cid;
            vm.SearchCommand.Execute(null);
            System.Threading.Thread.Sleep(1000);

            Assert.IsTrue(vm.Notes.Count == 1);
            Assert.IsTrue(vm.ContactPersons.Count == 1);

            TestCase.CleanUp();
        }    
    }
}