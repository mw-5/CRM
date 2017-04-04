using Microsoft.VisualStudio.TestTools.UnitTesting;
using CRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CRM.Model.Tests
{
    [TestClass()]
    public class ModelTests
    {
        [TestMethod()]
        public void LoadCustomersTest()
        {
            Model m = Model.GetModel();
            DataView dv = null;
            dv = m.Customers;
            Assert.IsTrue(dv != null);
        }

        [TestMethod()]
        public void LoadContactPersonsTest()
        {
            Model m = Model.GetModel();
            DataView dv = null;
            dv = m.ContactPersons;
            Assert.IsTrue(dv != null);
        }

        [TestMethod()]
        public void LoadNotesTest()
        {
            Model m = Model.GetModel();
            DataView dv = null;
            dv = m.Notes;
            Assert.IsTrue(dv != null);
        }
    }
}