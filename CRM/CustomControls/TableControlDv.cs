using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.ComponentModel;

namespace CRM.CustomControls
{
    public class TableControlDv : DataGrid
    {
        #region Properties & fields

        private Dictionary<String, String> filters = new Dictionary<string, string>();
        private Dictionary<String, String> types = new Dictionary<string, string>();
        private ObservableCollection<DataGridColumn> ignorableColumns = new ObservableCollection<DataGridColumn>();
        public ObservableCollection<DataGridColumn> IgnorableColumns
        {
            get
            {
                return ignorableColumns;
            }
            set
            {
                ignorableColumns = value;
            }
        }

        #endregion // Properties & fields

        public TableControlDv()
        {
#if !DEBUG  // These lines cause XAML-Editor to be unable to render xaml in VS2010. Therefore they are only compild in release builds. In order to debug filter comment preprocessor directive out.
            Loaded += new RoutedEventHandler(SetUpColumns);
            LayoutUpdated += new EventHandler(TableControlDv_LayoutUpdated);      
#endif
            AutoGeneratingColumn += new EventHandler<DataGridAutoGeneratingColumnEventArgs>(FormatDateColumn);
            AutoGeneratingColumn += new EventHandler<DataGridAutoGeneratingColumnEventArgs>(HideColumn);
        }

        void FormatDateColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                DataGridTextColumn col = e.Column as DataGridTextColumn;
                if (col != null)
                {
                    col.Binding.StringFormat = "{0:dd.MM.yyyy HH:mm:ss}";
                }
            }
        }

        void HideColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            foreach (DataGridColumn ic in IgnorableColumns)
            {
                if (e.Column.Header != null && e.Column.Header.ToString().Equals(ic.Header.ToString()))
                {
                    e.Column.Visibility = System.Windows.Visibility.Hidden;
                    break;
                }
            }
        }

        void SetUpColumns(object sender, RoutedEventArgs e)
        {
            AddColumnsToDictionary();
            AddTypesToDictionary();
            Loaded -= SetUpColumns;
        }

        private void AddColumnsToDictionary()
        {
            filters.Clear();
            foreach (DataGridColumn c in this.Columns)
            {
                if (!filters.ContainsKey(c.Header.ToString()))
                {
                    filters.Add(c.Header.ToString(), null);
                }
            }
        }

        private void AddTypesToDictionary()
        {
            types.Clear();
            DataView dv = (DataView)ItemsSource;
            if (dv.Table != null)
            {
                foreach (DataColumn col in dv.Table.Columns)
                {
                    if (col.DataType.ToString().IndexOf("String") > 0)
                    {
                        types.Add(col.ColumnName, "String");
                    }
                    else if (col.DataType.ToString().IndexOf("Date") > 0 || col.DataType.ToString().IndexOf("Time") > 0)
                    {
                        types.Add(col.ColumnName, "date");
                    }
                    else if (col.DataType.ToString().IndexOf("bool", StringComparison.InvariantCultureIgnoreCase) > 0)
                    {
                        types.Add(col.ColumnName, "Boolean");
                    }
                    else
                    {
                        types.Add(col.ColumnName, "Numeric");
                    }
                }
            }
        }

        #region Filtering

        private void ApplyFilter(String columnName, DataGridColumnHeader colHeader, String filterTerm)
        {
            filters[columnName] = filterTerm;
            try
            {
                ((DataView)this.ItemsSource).RowFilter = BuildFilterTerm();
                colHeader.Background = filteredBrush;
            }
            catch
            {
                MessageBox.Show("The entered filter is invalid.\nPlease check the operator and quotation marks.", "Filter is invalid", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ReapplyFilter()
        {
            try
            {
                ((DataView)this.ItemsSource).RowFilter = BuildFilterTerm();
                foreach (DataGridColumn c in this.Columns)
                {
                    if (filters[c.Header.ToString()] != null)
                    {
                        GetHeader(c, this).Background = filteredBrush;
                    }
                }
            }
            catch { }
        }

        private String BuildFilterTerm()
        {
            String filterTerm = "";
            foreach (String key in filters.Keys)
            {
                if (filters[key] != null)
                {
                    if (filterTerm.Length >0)
                    {
                        filterTerm += " AND ";
                    }
                    filterTerm += "[" + key + "]" + filters[key];
                }
            }
            return filterTerm;
        }

        private void ClearAllFilters()
        {
            // set filters in dictionary to null
            for (int i = 0; i < filters.Count; i++)
            {
                filters[filters.Keys.ToArray<String>()[i]] = null;
            }

            foreach (DataGridColumn c in this.Columns)
            {
                DataGridColumnHeader ch = GetHeader(c, this);
                if (ch != null)
                {
                    ch.Background = unfilteredBrush;
                }
            }

            ((DataView)ItemsSource).RowFilter = "";
        }

        private void ClearSelectedFilter(String columnName, DataGridColumnHeader colHeader)
        {
            filters[columnName] = null;
            ((DataView)ItemsSource).RowFilter = BuildFilterTerm();
            colHeader.Background = unfilteredBrush;
        }

        #endregion // Filtering

        #region Creation of Column Headers

        Brush unfilteredBrush = null;
        Brush filteredBrush = new LinearGradientBrush(Color.FromArgb(0, 251, 189, 33), Color.FromArgb(255, 251, 189, 33), 90.0);

        private void SetUpColumnHeaders() // if DataGridColumnHeader only has to be created one time
        {
            foreach (DataGridColumn c in this.Columns)
            {
                DataGridColumnHeader headerObj = GetHeader(c, this);
                if (headerObj != null)
                {
                    headerObj.ContextMenu = new TblContextMenu(c.Header.ToString(), this, headerObj);
                }
            }
        }

        void TableControlDv_LayoutUpdated(object sender, EventArgs e) // if DataGridColumnHeader doesn't need to be updated every time
        {
            SetUpColumnHeaders();
            ReapplyFilter();
        }

        private DataGridColumnHeader GetHeader(DataGridColumn column, DependencyObject reference)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(reference); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(reference, i);

                DataGridColumnHeader colHeader = child as DataGridColumnHeader;
                if ((colHeader != null) && (colHeader.Column == column))
                {
                    return colHeader;
                }

                colHeader = GetHeader(column, child);
                if (colHeader != null)
                {
                    return colHeader;
                }
            }

            return null;
        }

        #endregion // Creation of Column Headers

        #region ContextMenu

        class TblContextMenu : ContextMenu
        {
            #region General Fields

            DataGridColumnHeader colHeader;
            String colName;
            TableControlDv tblControl;

            #endregion // general fields

            public TblContextMenu(String colName, TableControlDv tblControl, DataGridColumnHeader colHeader)
            {
                this.colHeader = colHeader;
                this.colName = colName;
                this.tblControl = tblControl;

                CreateMiClearAllFilters();
                CreateMiClearThisFilter();

                if (tblControl.types.ContainsKey(colName) && tblControl.types[colName] == "Date")
                {
                    CreateDateFilter();
                }
                else
                {
                    CreateTextFilterBox();
                }

                CreateTreeViewButton();

                //CreateMiTestDataType(); // Testing of DataType of a column
            }

            #region Menu Items

            #region Clear all filters

            MenuItem miClearAllFilters = new MenuItem();
            private void CreateMiClearAllFilters()
            {
                miClearAllFilters.Header = "Clear all filters";
                miClearAllFilters.Click += new RoutedEventHandler(miClearAllFilters_Click);
                this.Items.Add(miClearAllFilters);
            }
            void miClearAllFilters_Click(object sender, RoutedEventArgs e)
            {
                tblControl.ClearAllFilters();
            }

            #endregion // Clear all filters

            #region Clear this filter

            MenuItem miClearThisFilter = new MenuItem();
            private void CreateMiClearThisFilter()
            {
                miClearThisFilter.Header = "Delete this filter";
                miClearThisFilter.Click += new RoutedEventHandler(miClearThisFilter_Click);
                this.Items.Add(miClearThisFilter);
            }
            void miClearThisFilter_Click(object sender, RoutedEventArgs e)
            {
                tblControl.ClearSelectedFilter(colName, colHeader);
            }

            #endregion // Clear this filter

            #region FilterBox

            TextBox tbFilter = new TextBox();
            private void CreateTextFilterBox()
            {
                MenuItem m = new MenuItem();
                WrapPanel wp = new WrapPanel();
                Button b = new Button();
                b.Content = "apply filter";
                b.Click += new RoutedEventHandler(MiFilter_Click);
                wp.Children.Add(tbFilter);
                wp.Children.Add(b);
                tbFilter.Width = 140;
                m = new MenuItem();
                m.Header = wp;
                m.StaysOpenOnClick = true;
                this.Items.Add(m);
            }
            private String CreateFilterExpressionFromTextbox()
            {
                bool isString = tblControl.types[this.colName] == "String";
                bool isdate = tblControl.types[this.colName] == "Date";
                String quotes = isString || isdate ? "'" : "";
                String expr = isString ? "LIKE " : "= ";
                expr += quotes + tbFilter.Text + quotes;
                return expr;
            }
            void MiFilter_Click(object sender, RoutedEventArgs e)
            {
                IsOpen = false;
                tblControl.ApplyFilter(colName, colHeader, CreateFilterExpressionFromTextbox());
            }

            #endregion FilterBox

            #region Date Filter

            MenuItem miDateFilter;
            DatePicker startDatePicker;
            DatePicker endDatePicker;
            private void CreateDateFilter()
            {
                // create date pickers
                WrapPanel wpStart = CreateDateFilterWrapPanel("Start:", out startDatePicker);
                WrapPanel wpEnd = CreateDateFilterWrapPanel("End:", out endDatePicker);
                WrapPanel wp = new WrapPanel();
                wp.Children.Add(wpStart);
                wp.Children.Add(wpEnd);
                wp.Orientation = Orientation.Vertical;
                startDatePicker.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(startDatePicker_SelectedDateChanged);
                endDatePicker.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(endDatePicker_SelectedDateChanged);

                // create filter button
                Button b = new Button();
                b.Content = "Apply filter";
                b.Click += new RoutedEventHandler(ApplyDateFilter_Click);
                b.Width = 102;
                WrapPanel wpButton = new WrapPanel();
                wpButton.Orientation = Orientation.Horizontal;
                Label lbl = new Label(); // empty label in order to position button
                lbl.Width = 40;
                wpButton.Children.Add(lbl);
                wpButton.Children.Add(b);
                wp.Children.Add(wpButton);

                // create menu item
                miDateFilter = new MenuItem();
                miDateFilter.Header = wp;miDateFilter.StaysOpenOnClick = true;
                this.Items.Add(miDateFilter);
            }
            private WrapPanel CreateDateFilterWrapPanel(String text, out DatePicker datePicker)
            {
                WrapPanel wp = new WrapPanel();
                datePicker = new DatePicker();
                Label lbl = new Label();
                lbl.Content = text;
                lbl.Width = 40;
                lbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                wp.Children.Add(lbl);
                wp.Children.Add(datePicker);
                wp.Orientation = Orientation.Horizontal;
                return wp;
            }
            private String CreateFilterExpressionFromDatePickers()
            {
                String expr = " >='" + startDatePicker.SelectedDate + "' AND [" + colName + "] <'" + ((DateTime)endDatePicker.SelectedDate).AddDays(1).Date + "'";
                return expr;
            }
            void startDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
            {
                if (endDatePicker.SelectedDate == null || startDatePicker.SelectedDate > endDatePicker.SelectedDate)
                {
                    endDatePicker.SelectedDate = startDatePicker.SelectedDate;
                }
            }
            void endDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
            {
                if (startDatePicker.SelectedDate == null || endDatePicker.SelectedDate < startDatePicker.SelectedDate)
                {
                    startDatePicker.SelectedDate = endDatePicker.SelectedDate;
                }
            }
            void ApplyDateFilter_Click(object sender, RoutedEventArgs e)
            {
                if (startDatePicker.SelectedDate == null || endDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Please define start and end date.");
                }
                else
                {
                    IsOpen = false;
                    tblControl.ApplyFilter(colName, colHeader, CreateFilterExpressionFromDatePickers());
                }
            }

            #endregion // Date Filter

            #region TreeView

            public TreeView tv;
            private void CreateTreeViewButton()
            {
                // check if column is part of data view
                if (!tblControl.types.ContainsKey(colName))
                {
                    return;
                }
                MenuItem mi = new MenuItem();
                mi.Header = "Select items";
                mi.Click += new RoutedEventHandler(MiCreateTreeView_Click);
                mi.StaysOpenOnClick = true;
                this.Items.Add(mi);
            }
            void MiCreateTreeView_Click(object sender, RoutedEventArgs e)
            {
                this.Cursor = System.Windows.Input.Cursors.Wait;
                MenuItem mi = (MenuItem)sender;
                this.Items.Remove(mi);
                CreateTreeView();
                this.UpdateLayout();
                this.Cursor = System.Windows.Input.Cursors.Arrow;
            }
            private void CreateTreeView()
            {
                // check if column is part of data view
                if (!tblControl.types.ContainsKey(colName))
                {
                    return;
                }

                tv = new TreeView();

                IEnumerable<String> list = (from r in ((DataView)tblControl.ItemsSource).Table.AsEnumerable()
                                            select r[colName].ToString()).Distinct();

                CheckBox cbAll = new CheckBox();
                cbAll.Content = "All";
                cbAll.Click += new RoutedEventHandler(cbAll_Click);
                cbAll.IsChecked = true;
                tv.Items.Add(cbAll);

                if (tblControl.types[colName] == "Numeric")
                {
                    list = list.OrderBy(s => s.ToString().PadLeft(10, "0".ToCharArray()[0]));
                }
                else if (tblControl.types[colName] == "Date")
                {
                    list = list.OrderBy(s => s.ToString().PadLeft(10, "0".ToCharArray()[0]).Substring(6, 4) + s.ToString().PadLeft(10, "0".ToCharArray()[0]).Substring(3, 2) + s.ToString().PadLeft(10, "0".ToCharArray()[0]).Substring(0, 2));
                }
                else
                {
                    list = list.OrderBy(s => s);
                }

                foreach (String s in list)
                {
                    CheckBox cb = new CheckBox();
                    cb.Content = s;
                    cb.Click += new RoutedEventHandler(cb_Click);
                    cb.IsChecked = true;
                    tv.Items.Add(cb);
                }

                MenuItem m = new MenuItem();
                WrapPanel wp = new WrapPanel();
                Button b = new Button();
                b.Content = "Apply filter";
                b.Click += new RoutedEventHandler(ApplyCheckBoxFilter_Click);
                wp.Orientation = Orientation.Vertical;
                tv.MaxHeight = 200;
                tv.MaxWidth = 100;
                wp.Children.Add(tv);
                wp.Children.Add(b);
                m.Header = wp;
                m.MaxHeight = 240;
                m.StaysOpenOnClick = true;

                this.Items.Add(m);
            }
            /// <summary>
            /// Set 'all' checkbox to false
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void cb_Click(object sender, RoutedEventArgs e)
            {
                if (((CheckBox)sender).IsChecked != true)
                {
                    ((CheckBox)tv.Items[0]).IsChecked = false;
                }
            }
            /// <summary>
            /// Select or unselect all checkboxes
            /// </summary>            
            void cbAll_Click(object sender, RoutedEventArgs e)
            {
                Cursor = System.Windows.Input.Cursors.Wait;
                bool cbState = ((CheckBox)sender).IsChecked == true ? true : false;
                for (int i = 1; i < tv.Items.Count; i++)
                {
                    ((CheckBox)tv.Items[i]).IsChecked = cbState;
                }
                Cursor = System.Windows.Input.Cursors.Arrow;
            }
            private String CreateFilterExpressionFromCheckboxes()
            {
                String quotes = "";
                quotes = tblControl.types[this.colName] == "String" ? "'" : quotes;
                quotes = tblControl.types[this.colName] == "Date" ? "'" : quotes;
                var qry = from CheckBox cb in tv.Items
                          where cb.IsChecked == true && cb.Content.ToString() != "All"
                          select quotes + cb.Content.ToString() + quotes;
                String expr = "IN (" + String.Join(",", qry) + ")";
                return expr;
            }
            void ApplyCheckBoxFilter_Click(object sender, RoutedEventArgs e)
            {
                IsOpen = false;
                tblControl.ApplyFilter(colName, colHeader, CreateFilterExpressionFromCheckboxes());
            }

            #endregion // Tree View

            #region Test Data Type

            private void CreateTestDataType()
            {
                MenuItem m = new MenuItem();
                m.Header = "Test DataType";
                m.Click += new RoutedEventHandler(TblContextMenu_Click);
                this.Items.Add(m);
            }
            void TblContextMenu_Click(object sender, RoutedEventArgs e)
            {
                TestDataType();
            }
            private void TestDataType()
            {
                MessageBox.Show(tblControl.types[colName]);
            }

            #endregion // Test Data Type

            #endregion // Menu Items
        }

        #endregion // ContextMenu
    }
}
