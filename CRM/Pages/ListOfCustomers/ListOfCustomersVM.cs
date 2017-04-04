using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System.Data;
using System.Windows.Data;
using CRM.Windows.FrmCustomer;

namespace CRM.Pages.ListOfCustomers
{
    class ListOfCustomersVM : ViewModelBase
    {
        public ListOfCustomersVM()
        {
            Navigator.RegisterUpdateMethod(() => { model.LoadCustomers(); Customers = model.Customers; });
        }

        Model.Model model = Model.Model.GetModel();

        #region Commands

        public ICommand NavStartCommand
        {
            get { return new RelayCommand(p => Navigator.Start()); }
        }
        public ICommand NavCockpitCommand
        {
            get { return new RelayCommand(p => this.NavCockpitWithCid()); }
        }
        public ICommand NewCustomerCommand
        {
            get { return new RelayCommand(p => NewCustomer()); }
        }
        public ICommand EditCustomerCommand
        {
            get { return new RelayCommand(p => EditCustomer()); }
        }

        #endregion // Commands

        #region Navigation

        private void NavCockpitWithCid()
        {
            if (SelectedRow == null)
            {
                MessageBox.Show(Strings["MsgSelectRow"].ToString());
            }
            else
            {
                Navigator.CockpitWithCid((SelectedRow)[model.TblCustomers.Cid.Name].ToString());
            }
        }

        #endregion // Navigation

        public DataView Customers
        {
            get { return model.Customers; }
            set { OnPropertyChanged("Customers"); }
        }
        public void LoadCustomersAsync()
        {
            new Task(() => { Customers = model.Customers; }).Start();

        }

        private DataRowView selectedRow;
        public DataRowView SelectedRow
        {
            get
            {
                return selectedRow;
            }
            set
            {
                selectedRow = value;
                OnPropertyChanged("SelectedRow");
            }
        }

        private void NewCustomer()
        {
            model.Id4Edit = null;
            new FrmCustomer().Show();
        }

        private void EditCustomer()
        {
            if (SelectedRow == null)
            {
                MessageBox.Show(Strings["MsgSelectCustomer"].ToString());
            }
            else
            {
                model.Id4Edit = SelectedRow[model.TblCustomers.Cid.Name].ToString();
                new FrmCustomer().Show();
            }
        }
    }
}
