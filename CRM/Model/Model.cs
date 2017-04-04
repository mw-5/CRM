using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Model
{
    public class Model : ModelBase
    {
        public Model()
        {
            TblCustomers = new DefTblCustomers();
            TblNotes = new DefTblNotes();
            TblContactPersons = new DefTblContactPersons();
            Sql = new SqlStatements();
            Tasks = new List<IAsyncResult>();
        }

        private static Model model;
        public static Model GetModel()
        {
            if (model == null)
            {
                model = new CRM.Model.Model();
            }
            return model;
        }

        public DefTblCustomers TblCustomers { get; private set; }
        public DefTblNotes TblNotes { get; private set; }
        public DefTblContactPersons TblContactPersons { get; private set; }
        public SqlStatements Sql { get; private set; }

        private DataView customers;
        public DataView Customers
        {
            get
            {
                if (customers == null)
                {
                    LoadCustomers();
                }
                return customers;
            }
            private set
            {
                customers = value;
            }
        }
        public void LoadCustomers()
        {
            LoadTable(out customers, Sql.Customers);
        }

        private DataView contactPersons;
        public DataView ContactPersons
        {
            get
            {
                if (contactPersons == null)
                {
                    LoadContactPersons(0);
                }
                return contactPersons;
            }
            set
            {
                contactPersons = value;
            }
        }
        public void LoadContactPersons(int cid)
        {
            LoadTable(out contactPersons, Sql.ContactPersonsById.Replace("XXX", cid.ToString()));
        }

        private DataView notes;
        public DataView Notes
        {
            get
            {
                if (notes == null)
                {
                    LoadNotes(0);
                }
                return notes;
            }            
            private set
            {
                notes = value;
            }
        }
        public void LoadNotes(int cid)
        {
            LoadTable(out notes, Sql.NotesById.Replace("XXX", cid.ToString()));
        }


        public void LoadAllTablesAsync()
        {
            new Task(LoadCustomers).Start();            
        }

        private int cid;
        public int Cid
        {
            get
            {
                return cid;
            }
            set
            {
                cid = value;                
            }
        }

        /// <summary>
        /// Used to pass id of record to be edited to a form.
        /// If new entry is desired pass null.
        /// </summary>
        public String Id4Edit { get; set; }

        public List<IAsyncResult> Tasks { get; set; }
    }
}
