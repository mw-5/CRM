using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CRM.Pages.Start
{
    class StartVM : ViewModelBase
    {
        Model.Model model = Model.Model.GetModel();

        #region Commands

        public ICommand NavCockpitCommand
        {
            get { return new RelayCommand(p => Navigator.Cockpit()); }
        }
        public ICommand NavListOfCustomersCommand
        {
            get { return new RelayCommand(p => Navigator.ListOfCustomers()); }
        }
        public ICommand RefreshTablesCommand
        {
            get { return new RelayCommand(p => model.LoadAllTablesAsync()); }
        }

        #endregion // Commands
    }
}
