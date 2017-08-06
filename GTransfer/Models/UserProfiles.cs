
using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Dapper;
using System.Windows;
using GTransfer.Interfaces;
using GTransfer.Library;

namespace GTransfer.Models
{
    public class UserProfiles : BaseModel, IDataErrorInfo, ITreeNode
    {
        private string _UNAME;
        private string _PASSWORD;
        private string _DESCRIPTION;
        private string _ROLE;
        private Decimal _DISLIMIT;
        private string _ACCOUNT;
        private ObservableCollection<MenuRight> _MenuRights;
        private ObservableCollection<UserAttribs> _AdditionalInfoList;
        private bool _IsExpanded;
        private bool _IsSelected;
        private ITreeNode _IParent;
        private string _TYPE;
        private string _PARENT;
        private ObservableCollection<ITreeNode> _Children;
        private bool _IsMatch;
        private byte _POST;
        private byte _HOLD;

        public byte POST { get { return _POST; } set { _POST = value; OnPropertyChanged("POST"); } }
        public byte HOLD { get { return _HOLD; } set { _HOLD = value; OnPropertyChanged("HOLD"); } }
        public Decimal DISLIMIT { get { return _DISLIMIT; } set { _DISLIMIT = value; OnPropertyChanged("DISLIMIT"); } }
        public string ACCOUNT { get { return _ACCOUNT; } set { _ACCOUNT = value; OnPropertyChanged("ACCOUNT"); } }
        public string ROLE { get { return _ROLE; } set { _ROLE = value; OnPropertyChanged("ROLE"); } }
        public string TYPE { get { return _TYPE; } set { _TYPE = value; OnPropertyChanged("TYPE"); } }
        public string PARENT { get { return _PARENT; } set { _PARENT = value; OnPropertyChanged("PARENT"); } }
        public string DESCRIPTION { get { return _DESCRIPTION; } set { _DESCRIPTION = value; OnPropertyChanged("DESCRIPTION"); } }
        public string PASSWORD { get { return _PASSWORD; } set { _PASSWORD = value; OnPropertyChanged("PASSWORD"); } }
        public string UNAME { get { return _UNAME; } set { _UNAME = value; OnPropertyChanged("UNAME"); } }
        public ObservableCollection<UserAttribs> UserRights { get { return _AdditionalInfoList; } set { _AdditionalInfoList = value; OnPropertyChanged("UserRights"); } }
        public ObservableCollection<MenuRight> MenuRights { get { return _MenuRights; } set { _MenuRights = value; OnPropertyChanged("MenuRights"); } }

        public override string ToString()
        {
            return UNAME;
        }
        public UserProfiles()
        {
            this.MenuRights = new ObservableCollection<MenuRight>();
            this.UserRights = new ObservableCollection<UserAttribs>();
        }
        //string IDataErrorInfo.Error
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //string IDataErrorInfo.this[string columnName]
        //{
        //    get { throw new NotImplementedException(); }
        //}

        public string Error
        {
            get { throw new NotImplementedException(); }
        }
        public string this[string columnName]
        {
            get
            {
                string result = null;
                // if (columnName == "UserName")
                // {
                //     if (string.IsNullOrEmpty(UNAME))
                //         result = "Please enter a  Name";
                // }
                // if (columnName == "Password")
                // {
                //     if (string.IsNullOrEmpty(PASSWORD))
                //         result = "Please enter a  Password";
                // }
                //if (columnName == "Description")
                // {
                //     if (string.IsNullOrEmpty(DESCRIPTION))
                //         result = "Please enter a  Description";
                // }
                return result;
            }
        }

        #region ITreeNodeProperties
        [IgnoreProperty(true)]
        public bool IsGroup { get { return TYPE == "G"; } }

        [IgnoreProperty(true)]
        public bool IsExpanded { get { return _IsExpanded; } set { _IsExpanded = value; OnPropertyChanged("IsExpanded"); } }
        [IgnoreProperty(true)]

        public bool IsSelected { get { return _IsSelected; } set { _IsSelected = value; OnPropertyChanged("IsSelected"); } }
        [IgnoreProperty(true)]

        public ITreeNode Parent { get { return _IParent; } set { _IParent = value; OnPropertyChanged("Parent"); } }

        [IgnoreProperty(true)]
        public string ParentID { get { return PARENT; } }

        [IgnoreProperty(true)]
        public ObservableCollection<ITreeNode> Children
        {
            get
            {
                if (_Children == null)
                    _Children = new ObservableCollection<ITreeNode>();
                return _Children;
            }
            set { _Children = value; OnPropertyChanged("Children"); }
        }

        [IgnoreProperty(true)]
        public string NodeName { get { return UNAME; } }

        [IgnoreProperty(true)]
        public string NodeID { get { return UNAME; } }

        [IgnoreProperty(true)]
        public bool IsMatch { get { return _IsMatch; } set { _IsMatch = value; OnPropertyChanged("IsMatch"); } }
        #endregion

        #region ITreeNodeMethods
        public bool IsCriteriaMatched(string criteria)
        {
            return String.IsNullOrEmpty(criteria) || (UNAME.ToUpper()).Contains(criteria.ToUpper());
        }
        public void ApplyCriteria(string criteria, Stack<ITreeNode> ancestors)
        {
            if (IsCriteriaMatched(criteria))
            {
                IsMatch = true;
                foreach (var ancestor in ancestors)
                {
                    ancestor.IsMatch = true;
                    ancestor.IsExpanded = !String.IsNullOrEmpty(criteria);

                }
            }
            else
                IsMatch = false;

            ancestors.Push(this);
            foreach (ITreeNode child in Children)
                child.ApplyCriteria(criteria, ancestors);

            ancestors.Pop();
        }

        public void Search(string criteria)
        {
            if (IsCriteriaMatched(criteria))
            {
                IsMatch = true;
                ExpandParent(this);

            }
            else
            {
                IsMatch = false;
                foreach (ITreeNode child in Children)
                {
                    child.Search(criteria);
                }
            }
        }

        public void ExpandParent(ITreeNode CurItem)
        {
            if (CurItem.Parent != null)
            {
                if (CurItem.IsMatch == false) CurItem.IsMatch = true;
                if (CurItem.IsExpanded == false) CurItem.IsExpanded = true;

            }
        }
        #endregion

    }

    public class UserAttribs : BaseViewModel
    {
        #region members
        private string _COLUMN_NAME;
        private Type _DATATYPE;
        private object _VALUE;
        private string _DESCRIPTION;
        #endregion
        #region properties
        public object VALUE
        {
            get { return _VALUE; }
            set { _VALUE = value; OnPropertyChanged("VALUE"); }
        }
        public string COLUMN_NAME
        {
            get { return _COLUMN_NAME; }
            set { _COLUMN_NAME = value; OnPropertyChanged("COLUMN_NAME"); }
        }
        public string DESCRIPTION
        {
            get { return _DESCRIPTION; }
            set { _DESCRIPTION = value; OnPropertyChanged("DESCRIPTION"); }
        }
        public Type DATATYPE
        {
            get { return _DATATYPE; }
            set { _DATATYPE = value; OnPropertyChanged("DATATYPE"); }
        }

        public string DATA_TYPE
        {
            set
            {
                switch (value.ToLower())
                {
                    case "tinyint":
                        DATATYPE = typeof(bool);
                        break;
                    case "bit":
                        DATATYPE = typeof(bool);
                        break;
                    case "varchar":
                        DATATYPE = typeof(string);
                        break;
                    default:
                        DATATYPE = typeof(string);
                        break;
                }
            }
        }

        #endregion
        public UserAttribs()
        {

        }
    }

    public class FormMenu : BaseModel
    {
        private byte _DELETE;
        private byte _EDIT;
        private byte _EXPORT;
        private byte _PRINT;
        private byte _NEW;
        private byte _POST;
        private byte _HOLD;
        private string _DATACONTEXT;
        private string _SHORTCUTKEY;
        private string _FORMPATH;
        private string _IMAGEPATH;
        private string _PARENT;
        private string _MENUNAME;
        private string _MID;
        private byte _DATACONTEXT_VTYPE;
        public static List<FormMenu> MenuList;
        private int _MENUGROUP;
        private string _REPORTNAME;

        public string REPORTNAME { get { return _REPORTNAME; } set { _REPORTNAME = value; OnPropertyChanged("REPORTNAME"); }   }      
        public string MENUNAME { get { return _MENUNAME; } set { _MENUNAME = value; OnPropertyChanged("MENUNAME"); } }
        public string PARENT { get { return _PARENT; } set { _PARENT = value; OnPropertyChanged("PARENT"); } }
        public string IMAGEPATH { get { return _IMAGEPATH; } set { _IMAGEPATH = value; OnPropertyChanged("IMAGEPATH"); } }
        public string FORMPATH { get { return _FORMPATH; } set { _FORMPATH = value; OnPropertyChanged("FORMPATH"); } }
        public string SHORTCUTKEY { get { return _SHORTCUTKEY; } set { _SHORTCUTKEY = value; OnPropertyChanged("SHORTCUTKEY"); } }
        public string DATACONTEXT { get { return _DATACONTEXT; } set { _DATACONTEXT = value; OnPropertyChanged("DATACONTEXT"); } }
        public byte DATACONTEXT_VTYPE { get { return _DATACONTEXT_VTYPE; } set { _DATACONTEXT_VTYPE = value; OnPropertyChanged("DATACONTEXT_VTYPE"); } }
        public byte NEW { get { return _NEW; } set { _NEW = value; OnPropertyChanged("NEW"); } }
        public byte EDIT { get { return _EDIT; } set { _EDIT = value; OnPropertyChanged("EDIT"); } }
        public byte DELETE { get { return _DELETE; } set { _DELETE = value; OnPropertyChanged("DELETE"); } }
        public byte PRINT { get { return _PRINT; } set { _PRINT = value; OnPropertyChanged("PRINT"); } }
        public byte EXPORT { get { return _EXPORT; } set { _EXPORT = value; OnPropertyChanged("EXPORT"); } }
        public byte POST { get { return _POST; } set { _POST = value; OnPropertyChanged("POST"); } }
        public byte HOLD { get { return _HOLD; } set { _HOLD = value; OnPropertyChanged("HOLD"); } }
        public int MENUGROUP { get { return _MENUGROUP; } set { _MENUGROUP = value; OnPropertyChanged("MENUGROUP"); } }
        public string MENUINDEX { get { return _MID; } set { _MID = value; OnPropertyChanged("MENUINDEX"); } }
        static FormMenu()
        {
          
        }


       

        public static void RefreshMenuTable()
        {
            IEnumerable<FormMenu> _Menus;
            try
            {
                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        _Menus = conn.Query<FormMenu>("SELECT * FROM MENU", transaction: tran);
                        foreach (FormMenu fm in MenuList)
                        {
                            if (_Menus.Any(x => x.MENUNAME == fm.MENUNAME))
                            {
                                if (!fm.Update(tran))
                                {
                                    MessageBox.Show("Failed to refresh menu", "Refreshing Menu", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                    return;
                                }
                            }
                            else
                            {
                                if (!fm.Save(tran))
                                {
                                    MessageBox.Show("Failed to refresh menu", "Refreshing Menu", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                    return;
                                }
                            }
                        }
                        tran.Commit();
                    }

                }
            }
            catch (Exception ex)
            {
                GlobalClass.ProcessError(ex, "Refreshing Menu");
            }
        }

        public bool Save(SqlTransaction Tran)
        {
            try
            {
                int  ret;
                string strSave = @"INSERT INTO MENU (MENUNAME, PARENT, IMAGEPATH, FORMPATH, SHORTCUTKEY, DATACONTEXT, NEW, EDIT, [DELETE], [PRINT], EXPORT, DATACONTEXT_VTYPE, MENUGROUP,MENUINDEX,REPORTNAME) 
                                VALUES (@MENUNAME, @PARENT, @IMAGEPATH, @FORMPATH, @SHORTCUTKEY, @DATACONTEXT, @NEW, @EDIT, @DELETE, @PRINT, @EXPORT, @DATACONTEXT_VTYPE, @MENUGROUP,@MENUINDEX,@REPORTNAME)";
                ret = Tran.Connection.Execute(strSave, this, transaction: Tran);
                if (ret == 1)
                {
                    string strUserSave = "merge dbo.tbluserrights as users	using (select 'Admin' as Uname, '" + this.MENUNAME + "' as MID ) as New_Mnu" +
	                            " on users.Uname=New_Mnu.Uname and Users.MID=New_Mnu.MID	when matched then update  set [OPEN]=1,[NEW]=1,[EDIT]=1,[DELETE]=1,[PRINT]=1,[EXPORT]=1 " +
	                            " when not matched then	insert 	(UNAME,MID,[OPEN],NEW,EDIT,[DELETE],[PRINT],EXPORT)	 values	 ( 'Admin','" + this.MENUNAME + "',1,1,1,1,1,1);";
                    Tran.Connection.Execute(strUserSave, transaction: Tran);
                }
                return ret==1;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool Update(SqlTransaction Tran)
        {
            try
            {
                string strUpdate = @"UPDATE MENU SET PARENT = @PARENT, IMAGEPATH = @IMAGEPATH, FORMPATH = @FORMPATH, SHORTCUTKEY = @SHORTCUTKEY, 
                                DATACONTEXT = @DATACONTEXT, NEW = @NEW, EDIT = @EDIT, [DELETE] = @DELETE, [PRINT] = @PRINT, EXPORT = @EXPORT, DATACONTEXT_VTYPE = @DATACONTEXT_VTYPE, REPORTNAME=@REPORTNAME,
                                MENUGROUP = @MENUGROUP,MENUINDEX=@MENUINDEX WHERE MENUNAME = @MENUNAME";
                return Tran.Connection.Execute(strUpdate, this, transaction: Tran) == 1;
            }
            catch (Exception EX) { MessageBox.Show(EX.InnerException.Message + this.MENUNAME); return false; }
        }
    }

    public class MenuRight : BaseModel, ITreeNode
    {
        private string _UNAME;
        private string _MID;
        private byte _DELETE;
        private byte _EDIT;
        private byte _EXPORT;
        private byte _PRINT;
        private byte _NEW;
        private byte _OPEN;
        private bool _IsExpanded;
        private bool _IsSelected;
        private ITreeNode _IParent;
        private FormMenu _AppMenu;
        private bool _IsMatch;
        private ObservableCollection<ITreeNode> _Children;

        public string UNAME { get { return _UNAME; } set { _UNAME = value; OnPropertyChanged("UNAME"); } }
        public string MID { get { return _MID; } set { _MID = value; OnPropertyChanged("MID"); } }
        public byte OPEN { get { return _OPEN; } set { _OPEN = value; OnPropertyChanged("OPEN"); } }
        public byte NEW { get { return _NEW; } set { _NEW = value; OnPropertyChanged("NEW"); } }
        public byte EDIT { get { return _EDIT; } set { _EDIT = value; OnPropertyChanged("EDIT"); } }
        public byte DELETE { get { return _DELETE; } set { _DELETE = value; OnPropertyChanged("DELETE"); } }
        public byte PRINT { get { return _PRINT; } set { _PRINT = value; OnPropertyChanged("PRINT"); } }
        public byte EXPORT { get { return _EXPORT; } set { _EXPORT = value; OnPropertyChanged("EXPORT"); } }


        [IgnoreProperty(true)]
        public FormMenu AppMenu { get { return _AppMenu; } set { _AppMenu = value; OnPropertyChanged("AppMenu"); } }


        #region ITreeNodeProperties
        [IgnoreProperty(true)]
        public bool IsGroup { get { return Children.Count > 0; } }

        [IgnoreProperty(true)]
        public bool IsExpanded { get { return _IsExpanded; } set { _IsExpanded = value; OnPropertyChanged("IsExpanded"); } }
        [IgnoreProperty(true)]

        public bool IsSelected { get { return _IsSelected; } set { _IsSelected = value; OnPropertyChanged("IsSelected"); } }
        [IgnoreProperty(true)]

        public ITreeNode Parent { get { return _IParent; } set { _IParent = value; OnPropertyChanged("Parent"); } }

        [IgnoreProperty(true)]
        public string ParentID { get { return AppMenu.PARENT.ToString(); } }

        [IgnoreProperty(true)]
        public ObservableCollection<ITreeNode> Children
        {
            get
            {
                if (_Children == null)
                    _Children = new ObservableCollection<ITreeNode>();
                return _Children;
            }
            set { _Children = value; OnPropertyChanged("Children"); }
        }

        [IgnoreProperty(true)]
        public string NodeName { get { return UNAME; } }

        [IgnoreProperty(true)]
        public string NodeID { get { return UNAME; } }

        [IgnoreProperty(true)]
        public bool IsMatch { get { return _IsMatch; } set { _IsMatch = value; OnPropertyChanged("IsMatch"); } }
        #endregion

        #region ITreeNodeMethods
        public bool IsCriteriaMatched(string criteria)
        {
            return String.IsNullOrEmpty(criteria) || (UNAME.ToUpper()).Contains(criteria.ToUpper());
        }
        public void ApplyCriteria(string criteria, Stack<ITreeNode> ancestors)
        {
            if (IsCriteriaMatched(criteria))
            {
                IsMatch = true;
                foreach (var ancestor in ancestors)
                {
                    ancestor.IsMatch = true;
                    ancestor.IsExpanded = !String.IsNullOrEmpty(criteria);

                }
            }
            else
                IsMatch = false;

            ancestors.Push(this);
            foreach (ITreeNode child in Children)
                child.ApplyCriteria(criteria, ancestors);

            ancestors.Pop();
        }

        public void Search(string criteria)
        {
            if (IsCriteriaMatched(criteria))
            {
                IsMatch = true;
                ExpandParent(this);

            }
            else
            {
                IsMatch = false;
                foreach (ITreeNode child in Children)
                {
                    child.Search(criteria);
                }
            }
        }

        public void ExpandParent(ITreeNode CurItem)
        {
            if (CurItem.Parent != null)
            {
                if (CurItem.IsMatch == false) CurItem.IsMatch = true;
                if (CurItem.IsExpanded == false) CurItem.IsExpanded = true;

            }
        }
        #endregion

    }
}