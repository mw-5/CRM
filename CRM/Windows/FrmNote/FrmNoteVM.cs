using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.IO;

namespace CRM.Windows.FrmNote
{
    public class FrmNoteVM : FrmBase
    {
        public FrmNoteVM(Window owner)
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
                    DataRow dr = (from d in model.Notes.Table.AsEnumerable()
                                  where d[tblDef.Id.Name].ToString().Equals(model.Id4Edit)
                                  select d).First();

                    Id = int.Parse(dr[tblDef.Id.Name].ToString());
                    Cid = int.Parse(dr[tblDef.Cid.Name].ToString());
                    CreatedBy = dr[tblDef.CreatedBy.Name].ToString();
                    EntryDate = dr[tblDef.EntryDate.Name] as DateTime?;
                    Memo = dr[tblDef.Memo.Name].ToString();
                    Category = dr[tblDef.Category.Name].ToString();
                    Attachment = dr[tblDef.Attachment.Name].ToString();                    
                }
                catch (Exception e)
                {
                    MessageBox.Show("Could not load entry.\n" + e.Message + "\n" + e.StackTrace);
                }
            }
            else
            {
                Cid = model.Cid;
            }
            model.Id4Edit = null;
        }

        #region Commands        

        public ICommand SubmitCommand
        {
            get { return new RelayCommand(p => Submit()); }
        }
        public ICommand AttachFileCommand
        {
            get { return new RelayCommand(p => AttachFile()); }
        }
        public ICommand RemoveAttachmentCommand
        {
            get { return new RelayCommand(p => RemoveAttachment()); }
        }

        #endregion // Commands

        #region Properties & fields

        Model.DefTblNotes tblDef = Model.Model.GetModel().TblNotes;

        private int id;
        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
                UpdateMap(tblDef.Id, value);
                OnPropertyChanged("Id");
            }
        }

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

        private String createdBy;
        public string CreatedBy
        {
            get
            {
                return createdBy;
            }

            set
            {
                if (value == null)
                {
                    value = Environment.UserName;
                }
                createdBy = value;
                UpdateMap(tblDef.CreatedBy, value);
                OnPropertyChanged("CreatedBy");
            }
        }

        private DateTime? entryDate;
        public DateTime? EntryDate
        {
            get
            {
                return entryDate;
            }

            set
            {
                if (value == null)
                {
                    value = DateTime.Now;
                }
                entryDate = value;
                UpdateMap(tblDef.EntryDate, value);
                OnPropertyChanged("EntryDate");
            }
        }

        private String memo;
        public string Memo
        {
            get
            {
                return memo;
            }

            set
            {
                memo = value;
                UpdateMap(tblDef.Memo, value);
                OnPropertyChanged("Memo");
            }
        }

        private String category;
        public string Category
        {
            get
            {
                return category;
            }

            set
            {
                category = value;
                UpdateMap(tblDef.Category, value);
                OnPropertyChanged("Category");
            }
        }

        private String attachment;
        public string Attachment
        {
            get
            {
                return attachment;
            }

            set
            {
                attachment = value;
                UpdateMap(tblDef.Attachment, value);
                OnPropertyChanged("Attachment");
            }
        }

        private String pathSrcAttachment;
        public String PathSrcAttachment
        {
            get
            {
                return pathSrcAttachment;
            }

            set
            {
                pathSrcAttachment = value;
            }
        }
       
        #endregion // Properties & fields

        public void Submit()
        {
            CopyAttachment();

            if (CreatedBy == null) { CreatedBy = System.Environment.UserName; }
            if (EntryDate == null) { EntryDate = DateTime.Now; }

            Submit(tblDef.TblName, new Tuple<Model.ColDef, object>(tblDef.Id, Id));

            this.CloseWindow();
        }

        public void CopyAttachment()
        {
            if (PathSrcAttachment != null) // only copies new files as path is not set when existing note is loaded
            {
                bool isCanceled = false;
                String pathDst = Config.GetConfig().PathCustomerFolders + Cid;
                if (!Directory.Exists(pathDst))
                {
                    Directory.CreateDirectory(pathDst);
                }
                pathDst += @"\" + Attachment;
                if (File.Exists(pathDst))
                {
                    if (MessageBox.Show(Strings["MsgFileAlreadyExists"].ToString(), Strings["CaptionFileAlreadyExists"].ToString(), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        File.Delete(pathDst);
                    }
                    else
                    {
                        isCanceled = true;
                    }
                }
                if (!isCanceled)
                {
                    model.Tasks.Add(new Action(() => File.Copy(pathSrcAttachment + Attachment, pathDst)).BeginInvoke(null, null));
                }
            }
        }

        public void RemoveAttachment()
        {
            Attachment = null;
            PathSrcAttachment = null;
        }

        public void AttachFile()
        {
            Forms.OpenFileDialog ofd = new Forms.OpenFileDialog();

            if (ofd.ShowDialog() == Forms.DialogResult.OK)
            {
                if (File.Exists(ofd.FileName))
                {
                    Attachment = ofd.SafeFileName;
                    pathSrcAttachment = ofd.FileName.Substring(0, ofd.FileName.Length - ofd.SafeFileName.Length);
                }
            }            
        }
    }
}
