using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CRM.Windows.FrmCustomer
{
    public class FrmCustomerVM : FrmBase
    {
        public FrmCustomerVM(Window owner)
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
                    DataRow dr = (from d in model.Customers.Table.AsEnumerable()
                                  where d[tblDef.Cid.Name].ToString().Equals(model.Id4Edit)
                                  select d).First();
                    
                    Cid = int.Parse(dr[tblDef.Cid.Name].ToString());
                    Company = dr[tblDef.Company.Name].ToString();
                    Address = dr[tblDef.Address.Name].ToString();
                    Zip = dr[tblDef.Zip.Name].ToString();
                    City = dr[tblDef.City.Name].ToString();
                    Country = dr[tblDef.Country.Name].ToString();
                    ContractId = dr[tblDef.ContractId.Name].ToString();
                    ContractDate = dr[tblDef.ContractDate.Name] as DateTime?;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Could not load entry.\n" + e.Message + "\n" + e.StackTrace);
                }
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
        
        Model.DefTblCustomers tblDef = Model.Model.GetModel().TblCustomers;

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

        private String company;
        public String Company
        {
            get
            {
                return company;
            }

            set
            {
                company = value;
                UpdateMap(tblDef.Company, value);
                OnPropertyChanged("Company");
            }
        }

        private String address;
        public String Address
        {
            get
            {
                return address;
            }

            set
            {
                address = value;
                UpdateMap(tblDef.Address, value);
                OnPropertyChanged("Address");
            }
        }

        private String zip;
        public String Zip
        {
            get
            {
                return zip;
            }

            set
            {
                zip = value;
                UpdateMap(tblDef.Zip, value);
                OnPropertyChanged("Zip");
            }
        }

        private String city;
        public String City
        {
            get
            {
                return city;
            }
            set
            {
                city = value;
                UpdateMap(tblDef.City, value);
                OnPropertyChanged("City");
            }
        }

        private String country;
        public String Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
                UpdateMap(tblDef.Country, value);
                OnPropertyChanged("Country");
            }
        }

        private String contractId;
        public String ContractId
        {
            get
            {
                return contractId;
            }

            set
            {
                contractId = value;
                UpdateMap(tblDef.ContractId, value);
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
                UpdateMap(tblDef.ContractDate, value);
                OnPropertyChanged("ContractId");
            }
        }

        #endregion // Properties & fields

        public void Submit()
        {
            Submit(tblDef.TblName, new Tuple<Model.ColDef, object>(tblDef.Cid, Cid));
            this.CloseWindow();
        }
    }
}
