using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GTransfer.Interfaces;
using GTransfer.Models;

namespace GTransfer.Library
{

    public enum ButtonAction
    {
        New = 1, Edit = 2, Init = 0, Selected = 3, Loaded = 5, Hidenew = 6
    }
    /// <summary>
    /// This is the base class for all viewmodels. 
    /// It implements INotifyPropertyChanged interface which is used to notify XAML of any property change.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// INotifyPropertyChanged Interface implemented Event Handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public ButtonAction _action;
        private bool _EntryPanelEnabled;
        private bool _NewEnabled;
        private bool _EditEnabled;
        private bool _SaveEnabled;
        private bool _DeleteEnabled;
        private bool _PrintEnabled;
        private bool _NewGroupEnabled;
        private bool _LoadEnabled = true;

        public bool LoadEnabled
        {
            get { return _LoadEnabled; }
            set { _LoadEnabled = value; }
        }

        public bool NewGroupEnabled
        {
            get { return _NewGroupEnabled; }
            set { _NewGroupEnabled = value; }
        }

        private bool _PreviewEnabled;
        private bool _PostEnabled;
        private bool _ExportEnabled;
        private bool _ImportEnabled;
        private short _FocusedElement;
        private ButtonAction _TransactionMode;
        private string _Tmode;

        /// <summary>
        /// Mode of transaction like 'NEW','EDIT','LOADED','NOTHING'
        /// </summary>
        public virtual string Tmode
        {
            get { return _Tmode; }
            set { _Tmode = value; OnPropertyChanged("Tmode"); }
        }

        /// <summary>
        /// Notifies XAML when a property within viewmodel is changed.
        /// </summary>
        /// <param name="propname">Name of the Property</param>
        public void OnPropertyChanged(string propname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propname));
            }
        }

        public RelayCommand NewCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand UndoCommand { get; set; }
        public RelayCommand LoadDataCommand { get; set; }
        public RelayCommand PrintCommand { get; set; }
        public RelayCommand PreviewCommand { get; set; }
        public RelayCommand PostCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand FocusCommand { get; set; }
        public RelayCommand HoldCommand { get; set; }
        

        public ButtonAction TransactionMode { get { return _TransactionMode; } set { _TransactionMode = value; OnPropertyChanged("TransactionMode"); } }
        public bool EntryPanelEnabled { get { return _EntryPanelEnabled; } set { _EntryPanelEnabled = value; OnPropertyChanged("EntryPanelEnabled"); } }
        public bool NewEnabled { get { return  _NewEnabled ; } set { _NewEnabled = value; OnPropertyChanged("NewEnabled"); } }
        public bool EditEnabled { get { return  _EditEnabled ; } set { _EditEnabled = value; OnPropertyChanged("EditEnabled"); } }
        public bool SaveEnabled { get { return _SaveEnabled; } set { _SaveEnabled = value; OnPropertyChanged("SaveEnabled"); } }
        public bool DeleteEnabled { get { return  _DeleteEnabled; } set { _DeleteEnabled = value; OnPropertyChanged("DeleteEnabled"); } }
        public bool PrintEnabled { get { return (CurMenuRight.PRINT == 1) ? _PrintEnabled : false; } set { _PrintEnabled = value; OnPropertyChanged("PrintEnabled"); } }
        public bool PreviewEnabled { get { return _PreviewEnabled; } set { _PreviewEnabled = value; OnPropertyChanged("PreviewEnabled"); } }
        public bool PostEnabled { get { return _PostEnabled; } set { _PostEnabled = value; OnPropertyChanged("PostEnabled"); } }
        public bool ImportEnabled { get { return _ImportEnabled; } set { _ImportEnabled = value; OnPropertyChanged("ImportEnabled"); } }
        public bool ExportEnabled { get { return _ExportEnabled; } set { _ExportEnabled = value; OnPropertyChanged("ExportEnabled"); } }
        public bool HoldEnabled { get { return _HoldEnabled; } set { _HoldEnabled = value; OnPropertyChanged("HoldEnabled"); } }

        public short FocusedElement { get { return _FocusedElement; } set { _FocusedElement = value; OnPropertyChanged("FocusedElement"); if (value > 0) FocusedElement = 0; } }
        public string TempVoucherName { get; set; }
        /// <summary>
        /// Default Construtor
        /// </summary>
        public BaseViewModel()
        {
            SetAction(ButtonAction.Init);
            NewCommand = new RelayCommand(ExecuteNew, CanExecuteNew);
            EditCommand = new RelayCommand(ExecuteEdit, CanExecuteEdit);
            SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            UndoCommand = new RelayCommand(ExecuteUndo, CanExecuteUndo);
            LoadDataCommand = new RelayCommand(ExecuteLoad, CanExecuteLoad);
            PrintCommand = new RelayCommand(ExecutePrint, CanExecutePrint);
            PreviewCommand = new RelayCommand(ExecutePreview, CanExecutePreview);
            PostCommand = new RelayCommand(ExecutePost, CanExecutePost);
            ImportCommand = new RelayCommand(ExecuteImport, CanExecuteImport);
            ExportCommand = new RelayCommand(ExecuteExport, CanExecuteExport);
            HoldCommand = new RelayCommand(ExecuteHold, CanExecuteHold);           
   
        }


        #region CanExecutes
        private bool CanExecuteHold(object obj)
        {
            return true;
        }
        protected virtual bool CanExecuteExport(object obj)
        {
            return (CurMenuRight.EXPORT == 1) ? _ExportEnabled : false; ;
        }
        private bool CanExecuteImport(object obj)
        {
            return _ImportEnabled;
        }
        private bool CanExecutePost(object obj)
        {
            return PostEnabled;
        }
        protected virtual bool CanExecutePreview(object obj)
        {
            return PreviewEnabled;
        }

        protected virtual bool CanExecutePrint(object obj)
        {
            return (CurMenuRight.PRINT == 1) ? PrintEnabled : false;
        }

        public virtual bool CanExecuteUndo(object obj)
        {
            return true;
        }

        public virtual bool CanExecuteLoad(object obj)
        {
            //return _action == ButtonAction.Init || _action == ButtonAction.Selected;
            return LoadEnabled;

        }

        public virtual bool CanExecuteDelete(object obj)
        {
            return  _action == ButtonAction.Selected ;
        }

        public virtual bool CanExecuteSave(object obj)
        {
            return _action == ButtonAction.New || _action == ButtonAction.Edit;
        }
        public virtual bool CanExecuteEdit(object obj)
        {
            return  _action == ButtonAction.Selected;
        }

        public virtual bool CanExecuteNew(object obj)
        {
            return NewEnabled;
        }
        #endregion

        #region Command Execute methods
        public virtual void ExecuteExport(object obj)
        {

        }

        protected virtual void ExecuteImport(object obj)
        {

        }

        protected virtual void ExecutePost(object obj)
        {
            PostMethod(obj);

        }

        public virtual void ExecutePreview(object obj)
        {

        }

        public virtual void ExecutePrint(object obj)
        {

        }

        protected virtual void ExecuteUndo(object obj)
        {
            if (UndoMethod(obj) == false) //false is to continue operation
            { SetAction(ButtonAction.Init, obj); }
        }

        protected virtual void ExecuteLoad(object obj)
        {
            LoadMethod(obj);
        }

        protected virtual void ExecuteDelete(object obj)
        {
            DeleteMethod(obj);
        }


        protected virtual void ExecuteSave(object obj)
        {

            SaveMethod(obj);
        }


        protected virtual void ExecuteEdit(object obj)
        {
            SetAction(ButtonAction.Edit, obj);
            UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;
            keyboardFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

        }



        public virtual void ExecuteNew(object obj)
        {
            SetAction(ButtonAction.New, obj);
        }
        private void ExecuteHold(object obj)
        {
            HoldMethod(obj);
        }
        #endregion

        #region Methods
        public virtual void NewMethod(object obj) { }
        public virtual void SaveMethod(object obj) { }
        public virtual void EditMethod(object obj) { }
        /// <summary>
        /// Methode which is called whte undo is clicked along with the setactions of the baseviwmodel
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>false to continue the seactions procedure: True to halt the setaction procedure</returns>
        public virtual bool UndoMethod(object obj)
        {
            return false;
            //CHECK WHETHER THIS IS CALL FROM SETACTION METHOD, IF CALLED THEN DONT' CALL SETACTION AGAIN, IT WILL STACK OVERFLOW
            //string  alreadycalled = obj as string ;
            //if (alreadycalled != null)
            //{
            //    if (alreadycalled == "CALLED") return;
            //}

        }
        public virtual void PrintMethod(object obj) { }
        public virtual void PreviewMethod(object obj) { }
        public virtual void DeleteMethod(object obj) { }
        public virtual void LoadMethod(object obj) { }
        public virtual void PostMethod(object obj) { }
        public virtual void HoldMethod(object obj) { }
        #endregion

        #region Button Actions
        /// <summary>
        /// Sets current operation state of entry form.
        /// </summary>
        /// <param name="action">Operation state to be set</param>
        public void SetAction(ButtonAction action, object obj = null)
        {
            PostEnabled = false;
            HoldEnabled = false;

            switch (action)
            {

                case ButtonAction.New:
                    Tmode = "NEW";
                    NewMethod(obj);
                    ClearAction(action);
                    Tmode = "NEW";
                    SaveEnabled = true;
                    EntryPanelEnabled = true;
                    LoadEnabled = false;
                    HoldEnabled = true;

                    break;
                case ButtonAction.Edit:
                    EditMethod(obj);
                    ClearAction(action);
                    Tmode = "EDIT";
                    SaveEnabled = true;
                    EntryPanelEnabled = true;

                    LoadEnabled = false;
                    break;
                case ButtonAction.Init:

                    if (UndoMethod(obj) == false)//true to halt below procedure
                    {
                        ClearAction(action);
                        Tmode = "NOTHING";
                        NewEnabled = true;
                        LoadEnabled = true;
                    }
                    break;
                case ButtonAction.Selected:
                    ClearAction(action);
                    Tmode = "LOADED";
                    NewEnabled = true;
                    EditEnabled = true;
                    DeleteEnabled = true;
                    PostEnabled = true;

                    break;

            }
            _action = action;
        }
        private void ClearAction(ButtonAction action)
        {
            TransactionMode = action;
            SaveEnabled = false;
            EditEnabled = false;
            NewEnabled = false;
            EntryPanelEnabled = false;
            DeleteEnabled = false;
        }
        #endregion
        #region AddedByAmir
        private MenuRight CurMenuRight=new MenuRight();
        bool _NewVisible;
        private bool _EditVisible;
        private bool _DeleteVisible;
        private bool _PrintVisible;
        private bool _ExportVisible;
        private bool _PostVisible;
        private bool _HoldVisible;
        private bool _HoldEnabled;
        public bool NewVisible { get { return _NewVisible; } set { _NewVisible = value; OnPropertyChanged("NewVisible"); } }
        public bool EditVisible { get { return _EditVisible; } set { _EditVisible = value; OnPropertyChanged("EditVisible"); } }
        public bool DeleteVisible { get { return _DeleteVisible; } set { _DeleteVisible = value; OnPropertyChanged("DeleteVisible"); } }
        public bool PrintVisible { get { return _PrintVisible; } set { _PrintVisible = value; OnPropertyChanged("PrintVisible"); } }
        public bool PostVisible { get { return _PostVisible; } set { _PostVisible = value; OnPropertyChanged("PostVisible"); } }
        public bool ExportVisible { get { return _ExportVisible; } set { _ExportVisible = value; OnPropertyChanged("ExportVisible"); } }
        public bool HoldVisible { get { return _HoldVisible; } set { _HoldVisible = value; OnPropertyChanged("HoldVisible"); } }
        //public string _MenuName;
        //public string MenuName
        //{
        //    get { return _MenuName; }
        //    set
        //    {
        //        _MenuName = value;
        //        OnPropertyChanged("MenuName");
        //        CurMenuRight = StaticCollection.UserRights.FirstOrDefault(x => x.MID == value);
        //        if (CurMenuRight == null)
        //            CurMenuRight = new MenuRight();
        //        var form = StaticCollection.MenuList.FirstOrDefault(x => x.MENUNAME == value);
        //        if (form != null)
        //        {
        //            NewVisible = (form.NEW == 1);
        //            EditVisible = (form.EDIT == 1);
        //            DeleteVisible = (form.DELETE == 1);
        //            PrintVisible = (form.PRINT == 1);
        //            ExportVisible = (form.EXPORT == 1);
        //            PostVisible = (form.POST == 1);
        //            HoldVisible = (form.HOLD == 1);
        //        }
        //        else
        //        {
        //            NewVisible = false;
        //            EditVisible = false;
        //            DeleteVisible = false;
        //            PrintVisible = false;
        //            ExportVisible = false;
        //        }
        //        if (PostVisible && GlobalClass.CurUser.UserRights.Any(x => x.COLUMN_NAME.ToUpper() == "VOUCHERPOSTRIGHT"))
        //        {
        //            PostVisible = (byte)GlobalClass.CurUser.UserRights.First(x => x.COLUMN_NAME.ToUpper() == "VOUCHERPOSTRIGHT").VALUE == 1;
        //        }

        //        if (HoldVisible && GlobalClass.CurUser.UserRights.Any(x => x.COLUMN_NAME.ToUpper() == "VOUCHERHOLDRIGHT"))
        //        {
        //            HoldVisible = (byte)GlobalClass.CurUser.UserRights.First(x => x.COLUMN_NAME.ToUpper() == "VOUCHERHOLDRIGHT").VALUE == 1;
        //        }

        //    }
        //}
        #endregion

    
    }

    public class BaseViewModelWithTree : BaseViewModel
    {

        private string _SearchCriteria;
        private bool _ByCode;
        private ITreeNode _Root;
        protected ObservableCollection<ITreeNode> _TotalNodes = new ObservableCollection<ITreeNode>();
        protected IEnumerable<ITreeNode> TreeSource;

        public ObservableCollection<ITreeNode> TotalNodes { get { return _TotalNodes; } set { _TotalNodes = value; } }
        public string SearchCriteria { get { return _SearchCriteria; } set { _SearchCriteria = value; OnPropertyChanged("SearchCriteria"); } }
        public ITreeNode Root { get { return _Root; } set { _Root = value; OnPropertyChanged("Root"); } }
        public bool  ByCode { get { return _ByCode; } set { _ByCode = value; OnPropertyChanged("ByCode"); } }

        public void SearchInTree(ITreeNode Item)
        {
            var itm = Item;
            if (Item == null)
                return;
            foreach (object item in Item.Children)
            {
                itm = (ITreeNode)item;

                if (ByCode == false)
                {
                    if (itm.NodeName.ToUpper() == SearchCriteria.ToUpper())
                    {
                        itm.IsSelected = true;
                        ExpandToRoot(itm);

                        OnPropertyChanged("SelectedItem");
                        OnPropertyChanged("IsSelected");
                        break;
                    }
                }
                else if (ByCode == true )
                {
                    if (itm.NodeID.ToUpper() == SearchCriteria.ToUpper())
                    {
                        itm.IsSelected = true;
                        ExpandToRoot(itm);

                        OnPropertyChanged("SelectedItem");
                        OnPropertyChanged("IsSelected");
                        break;
                    }
                }
                if (itm.Children != null && itm.Children.Count > 0)
                {
                    SearchInTree(itm);
                }
            }
        }

        public void RemoveItemFromTree(ObservableCollection<ITreeNode> Tree, ITreeNode Item)
        {
            var itm = Item;
            if (Item == null)
                return;
            if (Tree.Any(x => x.NodeID == Item.NodeID))
                Tree.Remove(Tree.First(x => x.NodeID == Item.NodeID));
            else
                foreach (ITreeNode item in Tree)
                {
                    if (item.Children != null)
                        RemoveItemFromTree(item.Children, Item);
                }
        }
        public void ExpandToRoot(ITreeNode Item)
        {
            if (Item.Parent != null)
            {
                Item.Parent.IsExpanded = true;
                //Item.Parent.IsMatch = true;
                ExpandToRoot(Item.Parent);
            }
        }
    }


}
