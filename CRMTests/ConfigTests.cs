using Microsoft.VisualStudio.TestTools.UnitTesting;
using CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Windows;

namespace CRM.Tests
{
    [TestClass()]
    public class ConfigTests
    {
        [TestMethod()]
        public void GetConfigTest()
        {
            Config config = Config.GetConfig();            
            Assert.IsTrue(config != null);
        }
    }
}