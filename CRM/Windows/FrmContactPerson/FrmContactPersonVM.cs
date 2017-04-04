using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Windows.Input;

namespace CRM.Windows.FrmContactPerson
{
    public class FrmContactPersonVM : FrmBase
    {
        public FrmContactPersonVM(Window owner)
        {
            Owner = owner;
            Mode = EntryMode.New;
            LoadFields();
        }

        private void LoadFields()
        {
            if (model.Id4Edit != null)
            {
                Mode = EntryMode.Edit;
                try
                {
                    DataRow dr = (from d in model.ContactPersons.Table.AsEnumerable()
                                  where d[tblDef.Id.Name].ToString().Equals(model.Id4Edit)
                                  select d).First();

                    Id = int.Parse(dr[tblDef.Id.Name].ToString());
                    Cid = int.Parse(dr[tblDef.Cid.Name].ToString());
                    Forename = dr[tblDef.Forename.Name].ToString();
                    Surname = dr[tblDef.Surname.Name].ToString();
                    Gender = dr[tblDef.Gender.Name].ToString();
                    Email = dr[tblDef.Email.Name].ToString();
                    Phone = dr[tblDef.Phone.Name].ToString();
                    MainContact = (bool)dr[tblDef.MainContact.Name];
                }
                catch(Exception e)
                {
                    MessageBox.Show("Could not load entry.\n" + e.Message + "\n" + e.StackTrace);
                }
            }
            else
            {
                Cid = model.Cid;
            }
            model.Id4Edit = null;
        }

        #region Commands

        public ICommand SubmitCommand
        {
            get { return new RelayCommand(p => Submit()); }
        }

        #endregion // Commands

        #region Properties & fields

        Model.DefTblContactPersons tblDef = Model.Model.GetModel().TblContactPersons;

        private int id;
        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
                UpdateMap(tblDef.Id, value);
                OnPropertyChanged("Id");
            }
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
                UpdateMap(tblDef.Cid, value);
                OnPropertyChanged("Cid");
            }
        }

        private String forename;
        public String Forename
        {
            get
            {
                return forename;
            }

            set
            {
                forename = value;
                UpdateMap(tblDef.Forename, value);
                OnPropertyChanged("Forename");
            }
        }

        private String surname;
        public String Surname
        {
            get
            {
                return surname;
            }

            set
            {
                surname = value;
                UpdateMap(tblDef.Surname, value);
                OnPropertyChanged("Surname");
            }
        }

        private String gender;
        public String Gender
        {
            get
            {
                return gender;
            }

            set
            {
                if ( !(value.ToString().Equals("m") || value.ToString().Equals("f")) )
                {
                    value = "";
                }
                gender = value;
                UpdateMap(tblDef.Gender, value);
                OnPropertyChanged("Gender");
            }
        }

        private String email;
        public String Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
                UpdateMap(tblDef.Email, value);
                OnPropertyChanged("Email");
            }
        }

        private String phone;
        public string Phone
        {
            get
            {
                return phone;
            }

            set
            {
                phone = value;
                UpdateMap(tblDef.Phone, value);
                OnPropertyChanged("Phone");
            }
        }

        private bool mainContact;
        public bool MainContact
        {
            get
            {
                return mainContact;
            }

            set
            {
                mainContact = value;
                UpdateMap(tblDef.MainContact, value);
                OnPropertyChanged("MainContact");
            }
        }

        #endregion // Properties & fields

        public void Submit()
        {                        
            Submit(tblDef.TblName, new Tuple<Model.ColDef, object>(tblDef.Id, Id));
            this.CloseWindow();
        }
    }
}
