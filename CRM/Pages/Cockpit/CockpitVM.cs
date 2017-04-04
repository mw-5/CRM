using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using System.Windows;
using CRM.Windows.FrmNote;
using CRM.Windows.FrmContactPerson;

namespace CRM.Pages.Cockpit
{
    public class CockpitVM : ViewModelBase
    {
        public CockpitVM()
        {
            Navigator.RegisterUpdateMethod(() => { model.LoadNotes(model.Cid); Notes =  model.Notes; });
            Navigator.RegisterUpdateMethod(() => { model.LoadContactPersons(model.Cid); ContactPersons = model.ContactPersons; });
        }

        Model.Model model = Model.Model.GetModel();
        Config config = Config.GetConfig();
        
        #region Commands

        public ICommand SearchCommand
        {
            get { return new RelayCommand(p => this.UpdateCockpit()); }
        }
        public ICommand NavStartCommand
        {
            get { return new RelayCommand(p => Navigator.Start()); }
        }
        public ICommand NavListOfCustomers
        {
            get { return new RelayCommand(p => Navigator.ListOfCustomers()); }
        }
        public ICommand OpenAttachmentCommand
        {
            get { return new RelayCommand(p => new Action(this.OpenAttachment).BeginInvoke(null, null)); }
        }
        public ICommand OpenFolderCommand
        {
            get { return new RelayCommand(p => OpenFolder()); }
        }
        public ICommand NewNoteCommand
        {
            get { return new RelayCommand(p => NewNote()); }
        }
        public ICommand EditNoteCommand
        {
            get { return new RelayCommand(p => EditNote()); }
        }
        public ICommand NewContactPersonCommand
        {
            get { return new RelayCommand(p => NewContactPerson()); }
        }
        public ICommand EditContactPersonCommand
        {
            get { return new RelayCommand(p => EditContactPerson()); }
        }

        #endregion // Commands

        #region Properties & fields

        private int cidSearchBox;
        public int CidSearchBox
        {
            get
            {
                return cidSearchBox;
            }

            set
            {
                cidSearchBox = value;
                OnPropertyChanged("CidSearchBox");
            }
        }
        
        public int Cid
        {
            get
            {
                return model.Cid;
            }

            set
            {
                model.Cid = value;
                OnPropertyChanged("Cid");

                CidSearchBox = value;
            }
        }

        private String address;
        public string Address
        {
            get
            {
                return address;
            }

            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }

        private String zip;
        public string Zip
        {
            get
            {
                return zip;
            }

            set
            {
                zip = value;
                OnPropertyChanged("Zip");
            }
        }

        private String city;
        public string City
        {
            get
            {
                return city;
            }

            set
            {
                city = value;
                OnPropertyChanged("City");
            }
        }

        private String country;
        public string Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
                OnPropertyChanged("Country");
            }
        }

        private String contractId;
        public string ContractId
        {
            get
            {
                return contractId;
            }

            set
            {
                contractId = value;
                OnPropertyChanged("ContractId");
            }
        }

        private DateTime? contractDate;
        public DateTime? ContractDate
        {
            get
            {
                return contractDate;
            }

            set
            {
                contractDate = value;
                OnPropertyChanged("ContractDate");
            }
        }

        private String company;
        public string Company
        {
            get
            {
                return company;
            }

            set
            {
                company = value;
                OnPropertyChanged("Company");
            }
        }



        #endregion // Properties & fields

        public void UpdateCockpit()
        {
            Cid = CidSearchBox;

            UpdateMasterDataCustomer();

            // set tables to empty in order not to confuse user with old data from another customer
            Notes = new DataView();
            ContactPersons = new DataView();

            // Sync is used instead of async as this results in a better performance 
            // probably due to async calls being executed in a lower priority thread
            LoadNotesSync();

            LoadContactPersonsAsync();

        }        
        private void UpdateUI()
        {
            App.Current.MainWindow.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Input);
        }

        private void UpdateMasterDataCustomer()
        {
            try
            {
                DataRow masterData = (from md in model.Customers.Table.AsEnumerable()
                                      where md[model.TblCustomers.Cid.Name].ToString().Equals(Cid.ToString())
                                      select md).First();

                Address = masterData[model.TblCustomers.Address.Name].ToString();
                Zip = masterData[model.TblCustomers.Zip.Name].ToString();
                City = masterData[model.TblCustomers.City.Name].ToString();
                Country = masterData[model.TblCustomers.Country.Name].ToString();
                ContractId = masterData[model.TblCustomers.ContractId.Name].ToString();
                ContractDate = (DateTime?)masterData[model.TblCustomers.ContractDate.Name];
                Company = masterData[model.TblCustomers.Company.Name].ToString();
            }
            catch
            {
                Address = "";
                Zip = "";
                City = "";
                Country = "";
                ContractId = "";
                ContractDate = null;
                Company = "";
            }
        }

        #region Contact Persons

        private DataView contactPersons;
        public DataView ContactPersons
        {
            get
            {
                if (contactPersons == null)
                {
                    contactPersons = new DataView();
                }
                return contactPersons;
            }
            set
            {
                contactPersons = value;
                OnPropertyChanged("ContactPersons");
            }
        }
        private void LoadContactPersonsAsync()
        {
            new Action<int>(model.LoadContactPersons).BeginInvoke(Cid, p => { ContactPersons = model.ContactPersons; }, null);
        }

        private DataRowView selectedContactPerson;
        public DataRowView SelectedContactPerson
        {
            get
            {
                return selectedContactPerson;
            }
            set
            {
                selectedContactPerson = value;
                OnPropertyChanged("SelectedContactPerson");
            }
        }

        private void NewContactPerson()
        {
            model.Id4Edit = null;
            new FrmContactPerson().Show();
        }

        private void EditContactPerson()
        {
            if (SelectedContactPerson == null)
            {                
                MessageBox.Show(Strings["MsgSelectContactPerson"].ToString());
            }
            else
            {
                model.Id4Edit = SelectedContactPerson[model.TblContactPersons.Id.Name].ToString();
                new FrmContactPerson().Show();
            }
        }
        #endregion // Contact Persons

        #region Notes

        private DataView notes;
        public DataView Notes
        {
            get
            {
                if (notes == null)
                {
                    notes = new DataView();
                }
                return notes;
            }

            set
            {
                notes = value;
                OnPropertyChanged("Notes");
            }
        }

        private void LoadNotesSync()
        {
            model.LoadNotes(Cid);
            Notes = model.Notes;
            //UpdateUI();
        }

        private DataRowView selectedNote;
        public DataRowView SelectedNote
        {
            get
            {
                return selectedNote;
            }
            set
            {
                selectedNote = value;
                OnPropertyChanged("SelectedNote");
            }
        }

        /// <summary>
        /// Method shall be executed asynchronosuly
        /// to let TableControlDv which comes afterwards in the visual tree
        /// execute its event and change the selected row when clicked.
        /// </summary>
        private void OpenAttachment()
        {
            System.Threading.Thread.Sleep(200); // give change of selected row time to execute

            if (SelectedNote is DataRowView)
            {
                String msg = Strings["MsgFileDoesNotExist"].ToString();
                DataRowView r = (DataRowView)SelectedNote;
                String attachment = r[model.TblNotes.Attachment.Name].ToString();
                if (attachment.Length != 0)
                {
                    String pathfile = (config.PathCustomerFolders + Cid.ToString() + @"\" + attachment);
                    if (File.Exists(pathfile))
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(pathfile);
                        }
                        catch(Exception e)
                        {
                            MessageBox.Show("Unable to open: " + pathfile + "\n" + e.Message + "\n" + e.StackTrace);
                        }
                    }
                    else
                    {
                        MessageBox.Show(msg);
                    }
                }
                else
                {
                    MessageBox.Show(msg);
                }
            }
        }

        private void NewNote()
        {
            model.Id4Edit = null;
            new FrmNote().Show();
        }

        private void EditNote()
        {
            if (SelectedNote == null)
            {
                MessageBox.Show(Strings["MsgSelectNote"].ToString());
            }
            else
            {
                model.Id4Edit = SelectedNote[model.TblNotes.Id.Name].ToString();
                new FrmNote().Show();
            }
        }

        private void OpenFolder()
        {
            String path = config.PathCustomerFolders + Cid.ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            System.Diagnostics.Process.Start(path);
        }

        #endregion // Notes
    }
}
