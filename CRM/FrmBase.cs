using CRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CRM
{
    public class FrmBase : ViewModelBaseCloseableWindow
    {
        protected Model.Model model = Model.Model.GetModel();

        public EntryMode Mode { get; set; }

        protected Dictionary<ColDef, object> Map = new Dictionary<ColDef, object>();

        protected void UpdateMap(ColDef key, object value)
        {
            if (Map.ContainsKey(key))
            {
                Map[key] = value;
            }
            else
            {
                Map.Add(key, value);
            }
        }

        protected void Submit(String tblName, Tuple<ColDef, Object> id)
        {
            String sql = "";
           
            if (Mode == EntryMode.New)
            {
                foreach (var kv in Map) // remove id if exists
                {
                    if (kv.Key.Name.Equals(id.Item1.Name))
                    {                        
                        Map.Remove(kv.Key);                        
                        break;                                                
                    }
                }
                object _id = model.ExecuteScalar("SELECT (Max(" + id.Item1.Name + ") + 1) FROM " + tblName); // generate new id
                if (_id.ToString().Equals(""))
                {
                    _id = 1;
                }
                Map.Add(id.Item1, _id);
                sql = SqlStatements.BuildInsert(tblName, Map);
            }
            else if (Mode == EntryMode.Edit)
            {
                sql = SqlStatements.BuildUpdate(tblName, Map, id);
            }

            try
            {
                model.Tasks.Add(new Action<String>(s => model.ExecuteActionQuery(s)).BeginInvoke(sql, p => { CRM.Pages.Navigator.UpdateTablesAsync(); }, null));                
            }
            catch(Exception e)
            {
                MessageBox.Show("Submit failed.\n" + e.Message + "\n" + e.StackTrace);
            }
        }
    }

    public enum EntryMode
    {
        New,
        Edit
    }
}
