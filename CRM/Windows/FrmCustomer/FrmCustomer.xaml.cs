using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CRM.Windows.FrmCustomer
{
    /// <summary>
    /// Interaktionslogik für FrmCustomer.xaml
    /// </summary>
    public partial class FrmCustomer : Window
    {
        public FrmCustomer()
        {
            InitializeComponent();
            this.DataContext = new FrmCustomerVM(this);
        }
    }
}
