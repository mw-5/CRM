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
    public class ModelBaseTests
    {
        ModelBase mb;

        [TestInitialize]
        public void Init()
        {
            mb = new ModelBase();
        }

        [TestMethod()]
        public void LoadTableTest()
        {
            DataView dv = null;
            mb.LoadTable(out dv, "SELECT * FROM customers");
            Assert.IsTrue(dv != null);
        }

        [TestMethod()]
        public void ExecuteScalarTest()
        {
            object o = mb.ExecuteScalar("SELECT Count(*) FROM customers;");
            Assert.IsTrue((long)o >= 0);
        }

        [TestMethod()]
        public void ExecuteActionQueryTest()
        {
            string sqlLastId = "SELECT Max(cid) AS last_id FROM customers";

            long cid = 0;
            long.TryParse(mb.ExecuteScalar(sqlLastId).ToString(), out cid);            
            cid++;
            
            mb.ExecuteActionQuery("INSERT INTO customers(cid) VALUES(" + cid.ToString("0") + ")");
            Assert.IsTrue(cid == long.Parse(mb.ExecuteScalar(sqlLastId).ToString()));
            
            mb.ExecuteActionQuery("DELETE FROM customers WHERE cid = " + cid.ToString("0"));
            cid--;
            Assert.IsTrue(cid == long.Parse(mb.ExecuteScalar(sqlLastId).ToString()));
        }
    }
}