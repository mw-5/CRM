using CRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CRM
{
    public class ViewModelBaseCloseableWindow : ViewModelBase
    {
        protected Window Owner { get; set; }

        protected void CloseWindow()
        {
            if (Owner != null)
            {
                Owner.Close();
            }
        }      
    }   
}
