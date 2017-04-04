using Microsoft.VisualStudio.TestTools.UnitTesting;
using CRM.Windows.FrmContactPerson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRMTests.TestCases;
using CRM.Model;
using CRM.Pages.Cockpit;
using System.Windows;
using System.Threading;
using System.Data;

namespace CRM.Windows.FrmContactPerson.Tests
{
    [TestClass()]
    public class FrmContactPersonVMTests
    {    
        [TestMethod()]
        public void NewSubmitCommandTest()
        {
            Model.Model m = Model.Model.GetModel();
            DefTblContactPersons def = new DefTblContactPersons();

            // set up model state    
            TestCase.CleanUp();        
            m.ExecuteActionQuery(String.Format("DELETE FROM {0} WHERE {1} = {2};", def.TblName, def.Cid.Name, TestContactPerson.cid));
            TestCase.PopulateTestData();
            m.Cid = TestContactPerson.cid;
            m.LoadCustomers();
            m.LoadContactPersons(TestCustomer.cid);
            Thread.Sleep(1000);
            m.Id4Edit = null;

            // create view model
            FrmContactPersonVM vm = new FrmContactPersonVM(null);

            // check that no data has been loaded
            Assert.AreEqual(vm.Id, 0);
            Assert.AreEqual(vm.Cid, TestContactPerson.cid);
            Assert.AreEqual(vm.Forename, null);
            Assert.AreEqual(vm.Surname, null);
            Assert.AreEqual(vm.Gender, null);
            Assert.AreEqual(vm.Email, null);
            Assert.AreEqual(vm.Phone, null);
            Assert.AreEqual(vm.MainContact, false);

            // enter data
            vm.Forename = TestContactPerson.forename;
            vm.Surname = TestContactPerson.surname;
            vm.Gender = TestContactPerson.gender;
            vm.Email = TestContactPerson.email;
            vm.Phone = TestContactPerson.phone;
            vm.MainContact = TestContactPerson.mainContact;

            vm.SubmitCommand.Execute(null);

            Thread.Sleep(1000);
            m.LoadContactPersons(TestCustomer.cid);
            Thread.Sleep(1000);

            // check new entries
            int lastEntry = 0;
            int max = 0;
            for (int i = 0; i < m.ContactPersons.Count; i++)
            {
                if (int.Parse(m.ContactPersons[i][def.Id.Name].ToString()) > max)
                {
                    max = int.Parse(m.ContactPersons[i][def.Id.Name].ToString());
                    lastEntry = i;
                }            
            }
            Assert.AreEqual(m.ContactPersons[lastEntry][def.Cid.Name], TestContactPerson.cid);
            Assert.AreEqual(m.ContactPersons[lastEntry][def.Forename.Name], TestContactPerson.forename);
            Assert.AreEqual(m.ContactPersons[lastEntry][def.Surname.Name], TestContactPerson.surname);
            Assert.AreEqual(m.ContactPersons[lastEntry][def.Gender.Name], TestContactPerson.gender);
            Assert.AreEqual(m.ContactPersons[lastEntry][def.Email.Name], TestContactPerson.email);
            Assert.AreEqual(m.ContactPersons[lastEntry][def.Phone.Name], TestContactPerson.phone);
            Assert.AreEqual(m.ContactPersons[lastEntry][def.MainContact.Name], TestContactPerson.mainContact);

            // clean up
            m.ExecuteActionQuery(String.Format("DELETE FROM {0} WHERE {1} = {2};", def.TblName, def.Cid.Name, TestContactPerson.cid));
            TestCase.CleanUp();
        }

        [TestMethod()]
        public void EditSubmitCommandTest()
        {
            Model.Model m = Model.Model.GetModel();
            DefTblContactPersons def = new DefTblContactPersons();

            // set up model state
            TestCase.PopulateTestData();
            m.Cid = TestContactPerson.cid;
            m.LoadCustomers();
            m.LoadContactPersons(TestCustomer.cid);
            Thread.Sleep(1000);
            m.Id4Edit = TestContactPerson.id.ToString();

            // create view model
            FrmContactPersonVM vm = new FrmContactPersonVM(null);

            // check if data has been loaded correctly
            Assert.AreEqual(vm.Id, TestContactPerson.id);
            Assert.AreEqual(vm.Cid, TestContactPerson.cid);            
            Assert.AreEqual(vm.Forename, TestContactPerson.forename);
            Assert.AreEqual(vm.Surname, TestContactPerson.surname);
            Assert.AreEqual(vm.Gender, TestContactPerson.gender);
            Assert.AreEqual(vm.Email, TestContactPerson.email);
            Assert.AreEqual(vm.Phone, TestContactPerson.phone);
            Assert.AreEqual(vm.MainContact, TestContactPerson.mainContact);

            // edit data
            vm.Forename = TestContactPerson.forename + "2";
            vm.Surname = TestContactPerson.surname + "2";
            vm.Gender = "f";
            vm.Email = TestContactPerson.email + "2";
            vm.Phone = TestContactPerson.phone + "2";
            vm.MainContact = false;

            vm.SubmitCommand.Execute(null);

            Thread.Sleep(1000);
            m.LoadContactPersons(TestCustomer.cid);
            Thread.Sleep(1000);

            // check modified entries
            Assert.AreEqual(m.ContactPersons[0][def.Forename.Name], TestContactPerson.forename + "2");
            Assert.AreEqual(m.ContactPersons[0][def.Surname.Name], TestContactPerson.surname + "2");
            Assert.AreEqual(m.ContactPersons[0][def.Gender.Name], "f");
            Assert.AreEqual(m.ContactPersons[0][def.Email.Name], TestContactPerson.email + "2");
            Assert.AreEqual(m.ContactPersons[0][def.Phone.Name], TestContactPerson.phone + "2");
            Assert.AreEqual(m.ContactPersons[0][def.MainContact.Name], false);

            TestCase.CleanUp();
        }

    }
}