using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Windows;

namespace CRM
{
    public class Config
    {
        public Config()
        {
            LoadConfig();
            config = this;
        }

        private static Config config;
        public static Config GetConfig()
        {
            if (config == null)
            {
                new Config();
            }
            return config;
        }

        private void LoadConfig()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("Config.xml");

            ConnectionString = "Server=" + xdoc.SelectSingleNode("//connectionString/server").InnerText + ";"
                + "User Id=" + xdoc.SelectSingleNode("//connectionString/user").InnerText + ";"
                + "Password=" + xdoc.SelectSingleNode("//connectionString/password").InnerText + ";"
                + "Database=" + xdoc.SelectSingleNode("//connectionString/databaseName").InnerText + ";";

            // get path for file folders of customers and close application if path does not exist.
            PathCustomerFolders = xdoc.SelectSingleNode("//pathCustomerFolders").InnerText;
            if ( !(PathCustomerFolders.EndsWith(@"\") || PathCustomerFolders.EndsWith(@"/")) )
            {
                PathCustomerFolders += @"\";
            }
            if(!Directory.Exists(PathCustomerFolders))
            {
                MessageBox.Show(String.Format(@Strings["MsgConfigPathDoesNotExist"].ToString(), PathCustomerFolders));
                Environment.Exit(0);            
            }

            Language = xdoc.SelectSingleNode("//language").InnerText;
            SetStringsDictionary();
        }

        public String ConnectionString { get; private set; }

        public String PathCustomerFolders { get; private set; }

        public String Language { get; private set; }

        public ResourceDictionary Strings { get; private set; }
        /// <summary>
        /// Set resource dictionary for strings.
        /// </summary>        
        private void SetStringsDictionary()
        {
            String dic = "";            
            switch (Language)
            {
                case "es":
                    dic = "StringsSpanish.xaml";                    
                    break;
                case "de":
                    dic = "StringsGerman.xaml";
                    break;
                default:
                    dic = "StringsDefault.xaml";
                    break;
            }
            Strings = (ResourceDictionary)Application.LoadComponent(new Uri("/CRM;component/Resources/" + dic, UriKind.RelativeOrAbsolute));            
        }

    }

   
    




}
