using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Data;
using System.Windows;

namespace CRM.Pages.Cockpit
{
    class AttachmentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate AttachmentTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataRowView dr = item as DataRowView;
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is DataRowView)
            {
                if (!dr[Model.Model.GetModel().TblNotes.Attachment.Name].ToString().Equals(""))
                {
                    return AttachmentTemplate;
                }                
            }
            return DefaultTemplate;
        }
    }
}
