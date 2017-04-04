using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace CRM.Pages
{
    class Navigator
    {
        private static Start.Start start;
        public static Start.Start StartPage
        {
            get
            {
                return start;
            }
            set
            {
                start = value;
            }
        }
        public static void Start()
        {
            if (start == null)
            {
                start = new Start.Start();
            }
            App.Current.MainWindow.Content = start;
        }

        private static ListOfCustomers.ListOfCustomers listOfCustomers;
        public static void ListOfCustomers()
        {
            if (listOfCustomers == null)
            {
                listOfCustomers = new Pages.ListOfCustomers.ListOfCustomers();
            }                           
            App.Current.MainWindow.Content = listOfCustomers;
        }

        private static Cockpit.Cockpit cockpit;
        public static void Cockpit()
        {
            if (cockpit == null)
            {
                cockpit = new Cockpit.Cockpit();
            }
            App.Current.MainWindow.Content = cockpit;
        }
        public static void CockpitWithCid(String cid)
        {
            if (cid == null || cid.Equals(""))
            {
                MessageBox.Show(Config.GetConfig().Strings["MsgSelectCid"].ToString());
            }
            else
            {
                try
                {
                    App.Current.MainWindow.Cursor = Cursors.Wait;
                    if (cockpit == null)
                    {
                        cockpit = new Cockpit.Cockpit();
                    }
                    Cockpit.CockpitVM cvm = (Cockpit.CockpitVM)cockpit.DataContext;
                    App.Current.MainWindow.Content = cockpit;
                    cvm.CidSearchBox = int.Parse(cid);
                    cvm.UpdateCockpit();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Command couldn't be executed.\n" + e.Message + "\n" + e.StackTrace);
                }
                finally
                {
                    App.Current.MainWindow.Cursor = Cursors.Arrow;
                }               
            }
        }

        private static List<Action> AsyncUpdateCalls = new List<Action>();
        /// <summary>
        /// Registers a method that should be called to asynchronously update a table of a viewmodel.
        /// </summary>
        /// <param name="method">The method which updates the property of the viewmodel.</param>
        public static void RegisterUpdateMethod(Action method)
        {
            AsyncUpdateCalls.Add(method);
        }
        public static void UpdateTablesAsync()
        {
            foreach(Action a in AsyncUpdateCalls)
            {
                a.BeginInvoke(null, null);
            }
        }

    }
}
