using Microsoft.VisualStudio.TestTools.UnitTesting;
using CRM.Windows.FrmNote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CRMTests.TestCases;
using System.Threading;
using CRM.Model;

namespace CRM.Windows.FrmNote.Tests
{
    [TestClass()]
    public class FrmNoteVMTests
    {
        [TestMethod()]
        public void NewSubmitCommandTest()
        {
            Model.Model m = Model.Model.GetModel();
            DefTblNotes def = new DefTblNotes();

            // set up model state
            m.ExecuteActionQuery(String.Format("DELETE FROM {0} WHERE {1} = {2};", def.TblName, def.Cid.Name, TestNote.cid));
            TestCase.PopulateTestData();
            m.Cid = TestNote.cid;
            m.LoadCustomers();
            m.LoadNotes(TestNote.cid);
            Thread.Sleep(1000);
            m.Id4Edit = null;

            // create view model
            FrmNoteVM vm = new FrmNoteVM(null);

            // check that no data has been loaded
            Assert.AreEqual(vm.Id, 0);
            Assert.AreEqual(vm.Cid, TestNote.cid);
            Assert.AreEqual(vm.CreatedBy, null);
            Assert.AreEqual(vm.EntryDate, null);
            Assert.AreEqual(vm.Memo, null);
            Assert.AreEqual(vm.Category, null);
            Assert.AreEqual(vm.Attachment, null);
            Assert.AreEqual(vm.PathSrcAttachment, null);

            // enter data
            vm.Memo = TestNote.memo;
            vm.Category = TestNote.category;
            // attachment is not included to avoid copying which is tested separately

            vm.SubmitCommand.Execute(null);

            Thread.Sleep(1000);
            m.LoadNotes(TestNote.cid);
            Thread.Sleep(1000);

            // check new entries
            int lastEntry = m.Notes.Count - 1;
            Assert.AreEqual(m.Notes[lastEntry][def.Cid.Name], TestNote.cid);
            Assert.AreEqual(m.Notes[lastEntry][def.CreatedBy.Name], System.Environment.UserName);
            Assert.AreEqual(((DateTime)m.Notes[lastEntry][def.EntryDate.Name]).Date, DateTime.Now.Date);
            Assert.AreEqual(m.Notes[lastEntry][def.Memo.Name], TestNote.memo);
            Assert.AreEqual(m.Notes[lastEntry][def.Category.Name], TestNote.category);            
            Assert.AreEqual(m.Notes[lastEntry][def.Attachment.Name].ToString(), "");

            // clean up
            m.ExecuteActionQuery(String.Format("DELETE FROM {0} WHERE {1} = {2};", def.TblName, def.Cid.Name, TestNote.cid));
            TestCase.CleanUp();
        }

        [TestMethod()]
        public void EditSubmitCommandTest()
        {
            Model.Model m = Model.Model.GetModel();
            DefTblNotes def = new DefTblNotes();

            // set up model state
            m.ExecuteActionQuery(String.Format("DELETE FROM {0} WHERE {1} = {2};", def.TblName, def.Cid.Name, TestNote.cid));
            TestCase.PopulateTestData();
            m.Cid = TestNote.cid;
            m.LoadCustomers();
            m.LoadNotes(TestNote.cid);
            Thread.Sleep(1000);
            m.Id4Edit = TestNote.cid.ToString();

            // create view model
            FrmNoteVM vm = new FrmNoteVM(null);

            // check if data has been loaded correctly
            Assert.AreEqual(vm.Id, TestNote.id);
            Assert.AreEqual(vm.Cid, TestNote.cid);
            Assert.AreEqual(vm.CreatedBy, TestNote.createdBy);
            Assert.AreEqual(vm.EntryDate, TestNote.entryDate);
            Assert.AreEqual(vm.Memo, TestNote.memo);
            Assert.AreEqual(vm.Category, TestNote.category);
            Assert.AreEqual(vm.Attachment, TestNote.attachment);

            // edit data
            vm.Memo = TestNote.memo + "2";
            vm.Category = TestNote.category + "2";
            // attachment is not edited to avoid copying which is tested separately

            vm.SubmitCommand.Execute(null);

            Thread.Sleep(1000);
            m.LoadNotes(TestNote.cid);
            Thread.Sleep(1000);

            // check modified entries
            Assert.AreEqual(m.Notes[0][def.Id.Name], TestNote.id);
            Assert.AreEqual(m.Notes[0][def.CreatedBy.Name], TestNote.createdBy);
            Assert.AreEqual(m.Notes[0][def.EntryDate.Name], TestNote.entryDate);
            Assert.AreEqual(m.Notes[0][def.Memo.Name], TestNote.memo + "2");
            Assert.AreEqual(m.Notes[0][def.Category.Name], TestNote.category + "2");
            Assert.AreEqual(m.Notes[0][def.Attachment.Name], TestNote.attachment);

            TestCase.CleanUp();
        }

        [TestMethod()]
        public void CopyAttachmentTest()
        {
            // set up
            String srcPath = Config.GetConfig().PathCustomerFolders + "test";
            String srcPathFile = srcPath + @"\" + TestNote.attachment;
            String dstPathFile = Config.GetConfig().PathCustomerFolders + TestNote.cid + @"\" + TestNote.attachment;
            // set up source file
            if (!Directory.Exists(srcPath))
            {
                Directory.CreateDirectory(srcPath);
            }            
            if (!File.Exists(srcPathFile))
            {
                File.Create(srcPathFile);
            }

            // clean up destination folder            
            if (File.Exists(dstPathFile))
            {
                File.Delete(dstPathFile);
            }

            // set up view model and set input
            FrmNoteVM vm = new FrmNoteVM(null);
            vm.Cid = TestNote.cid;
            vm.PathSrcAttachment = srcPath + @"\";
            vm.Attachment = TestNote.attachment;
            
            vm.CopyAttachment();
            Thread.Sleep(1000);

            Assert.IsTrue(File.Exists(dstPathFile));

            // clean up
            if (File.Exists(dstPathFile))
            {
                File.Delete(dstPathFile);
            }
            if (!Directory.Exists(srcPath))
            {
                Directory.Delete(srcPath);
            }
            if (!File.Exists(srcPathFile))
            {
                File.Delete(srcPathFile);
            }
        }

        [TestMethod()]
        public void RemoveAttachmentCommandTest()
        {
            FrmNoteVM vm = new FrmNoteVM(null);
            vm.Attachment = "file.txt";
            vm.PathSrcAttachment = "path/test";
            vm.RemoveAttachmentCommand.Execute(null);
            Assert.AreEqual(vm.Attachment, null);
            Assert.AreEqual(vm.PathSrcAttachment, null);
        }
    }
}