using GTransfer.Library;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dapper;
using System.Collections.ObjectModel;
using GTransfer.Models;
using GTransfer.Interfaces;

namespace GTransfer.ViewModels
{
    class parentlabel
    {
        public string label { get; set; }
        public string value { get; set; }
    }
    class LocationEntryViewModel : BaseViewModelWithTree
    {
        private Location _LocationObj;
        private Location _selectedLocation;
        private bool _editMode;
        private string _InformationTest;
        private int _noOfChild;
        private bool _autogenerateCode;
        private string _labelNoOfChild = "No. Of Child";
        private string _labelCode = "Code";
        private bool _autoGenerateVisible;

        public bool autoGenerateVisible { get { return _autoGenerateVisible; } set { _autoGenerateVisible = value; OnPropertyChanged("autoGenerateVisible"); } }
        public string labelNoOfChild { get { return _labelNoOfChild; } set { _labelNoOfChild = value; OnPropertyChanged("labelNoOfChild"); } }
        public bool autogenerateCode { get { return _autogenerateCode; } set { _autogenerateCode = value; OnPropertyChanged("autogenerateCode"); } }
        public int noOfChild { get { return _noOfChild; } set { _noOfChild = value; OnPropertyChanged("noOfChild"); } }
        public string InformationTest { get { return _InformationTest; } set { _InformationTest = value; OnPropertyChanged("InformationTest"); } }
        public bool editMode { get { return _editMode; } set { _editMode = value; OnPropertyChanged("editMode"); } }
        public Location selectedLocation { get { return _selectedLocation; } set { _selectedLocation = value; OnPropertyChanged("selectedLocation"); } }
        public Location LocationObj { get { return _LocationObj; } set { _LocationObj = value; OnPropertyChanged("LocationObj"); } }
        private ObservableCollection<ITreeNode> _LocationTreeList;
        public ObservableCollection<ITreeNode> LocationTreeList { get { return _LocationTreeList; } set { _LocationTreeList = value; OnPropertyChanged("LocationTreeList"); } }


        public string labelCode { get { return _labelCode; } set { _labelCode = value; OnPropertyChanged("labelCode"); } }

        private ObservableCollection<parentlabel> _parentList;
        public ObservableCollection<parentlabel> parentList { get { return _parentList; } set { _parentList = value; OnPropertyChanged("parentList"); } }
        public List<dynamic> LocationLevelLabel { get; set; }

        public LocationEntryViewModel()
        {try
            {
                NewEnabled = false;
                EditVisible = true;
                DeleteVisible = true;
                autoGenerateVisible = false;
                LocationTreeMaker();
                parentList = new ObservableCollection<parentlabel>();
                LocationLevelLabel = new List<dynamic>(GetLL());
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }


        }
        private IEnumerable<dynamic> GetLL() {
            using (SqlConnection con = new SqlConnection(GlobalClass.DataConnectionString)) {
                return  con.Query<dynamic>("SELECT level,label FROM TBL_LOCATION_LABEL");
            } }
        private string LevelLabel(int level)
        {
           // string label = "";
            string label= LocationLevelLabel.FirstOrDefault(l => l.level == level).label;
            //switch (level)
            //{
            //    case 1:
            //        label = "Floor";
            //        break;
            //    case 2:
            //        label = "Rack";
            //        break;
            //    case 3:
            //        label = "Shelf";
            //        break;
            //    case 4:
            //        label = "Cell";
            //        break;

            //}
            return label;
        }

        private void LocationTreeMaker()
        {
            try
            {
                var WList = new ObservableCollection<dynamic>();
                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    WList = new ObservableCollection<dynamic>(conn.Query<dynamic>("SELECT * FROM RMD_WAREHOUSE"));
                    TreeSource = conn.Query<Location>("SELECT * FROM TBL_LOCATIONS");
                }

                LocationTreeList = new ObservableCollection<ITreeNode>();
                foreach (var W in WList)
                {
                    Root = new Location() { LocationId = W.NAME, LocationCode = W.NAME, LocationName = W.NAME, Path = W.NAME, IsExpanded = true, IsMatch = true, Level = 0, Warehouse = W.NAME, Children = new ObservableCollection<ITreeNode>(), Parent = new Location() };
                    (Root as Location).PropertyChanged += ATree_PropertyChanged;
                    LoadChild(Root as Location);
                    LocationTreeList.Add(Root as Location);
                }


                //  LoadChild(Root as Location);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadChild(Location Parent, Location _LocationType = null)
        {
            try
            {
                foreach (Location l in TreeSource.Where(x => x.ParentID == Parent.LocationId).OrderBy(x => x.NodeName))
                {
                    Location NewNode = new Location() { LocationId = l.LocationId, LocationCode = l.LocationCode, LocationName = l.LocationName, Parent = Parent, Path = l.Path, IsMatch = true, Level = l.Level, ParentLocation = Parent.LocationId, Warehouse = Parent.Warehouse, Children = new ObservableCollection<ITreeNode>() };

                    Parent.Children.Add(NewNode);
                    NewNode.PropertyChanged += ATree_PropertyChanged;
                    _TotalNodes.Add(NewNode);
                    // if (NewNode) {
                    LoadChild(NewNode);
                    // LocationTreeList.Add(NewNode);
                    //  }

                }

            }
            catch (Exception ex)
            {
                GlobalClass.ProcessError(ex, "Ledger Account");
            }
        }

        public void ATree_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected" && (sender as ITreeNode).IsSelected)
            {
                try
                {
                    // parentList.Add(new parentlabel() {label=LevelLabel() });
                    InformationTest = "";

                    selectedLocation = (Location)sender;
                    labelCode = LevelLabel(selectedLocation.Level) + " Code";
                    labelNoOfChild = "No.Of " + LevelLabel(selectedLocation.Level);
                    LocationObj = selectedLocation;
                    LocationObj.PropertyChanged += LocationObj_PropertyChanged;
                    editMode = true;
                    SetAction(ButtonAction.Selected);
                    if (selectedLocation.Level == 0)
                    {
                        EditEnabled = false;
                        DeleteEnabled = false;
                    }
                    if (selectedLocation.Level == Settings.LocationLevelLimit)
                    {
                        InformationTest = "Level Limit Reached.Can't add further sub Level.";
                        NewEnabled = false;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public override bool UndoMethod(object obj)
        {
            autoGenerateVisible = false;
            autogenerateCode = false;
            noOfChild = 0;
            editMode = false;
            selectedLocation = null;
            LocationObj = null;
            InformationTest = "";
            parentList = new ObservableCollection<parentlabel>();
            return false;
        }
        public override void DeleteMethod(object obj)
        {
            if (MessageBox.Show("You are about to delete current Location.Once you Delete you cannot recover it back. Do you want to proceed?", "Delete Data", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
            {
                Conn.Open();
                var checkChild = Conn.ExecuteScalar("SELECT LocationId FROM TBL_LOCATIONS WHERE ParentLocation='" + selectedLocation.LocationId + "'");
                if (checkChild != null) { MessageBox.Show("Location contains another sub location so it can't be deleted"); return; }

                //validation of stock needed
                Conn.Query("DELETE FROM TBL_LOCATIONS WHERE LocationId='" + selectedLocation.LocationId + "'");
            }
            MessageBox.Show("Selected Location is Deleted Successfully...");
            LocationTreeMaker();
            ExecuteUndo(null);
        }
        public override void ExecuteNew(object obj)
        {
            if (selectedLocation == null) { MessageBox.Show("Please select the location in Tree to insert sub Location");  return; }
            LocationObj = new Location();
            LocationObj.PropertyChanged += LocationObj_PropertyChanged;
            LocationObj.Parent = selectedLocation;
            LocationObj.ParentLocation = selectedLocation.LocationId;
            LocationObj.Warehouse = selectedLocation.Warehouse;
            LocationObj.Level = selectedLocation.Level + 1;
            labelCode = LevelLabel(LocationObj.Level) + " Code";
            labelNoOfChild = "No.Of " + LevelLabel(LocationObj.Level);
            //if (LocationObj.Level == Settings.LocationLevelLimit) {
            //LocationObj.LocationCode=}     
            editMode = false;
            if (selectedLocation.Level == 0) { autoGenerateVisible = false; }
            else { autoGenerateVisible = true; }
            SetAction(ButtonAction.New);

        }
     
        private void LocationObj_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LocationCode")
            {
                LocationObj.LocationName = LocationObj.LocationCode + "(" + LevelLabel(LocationObj.Level) + ")";
            }
        }

        //private IEnumerable<Location> GetParentList(int level) {
        //    parentList.Add(new parentlabel() {label=LevelLabel(level),value= });
        //     }
        private string GetNewLId()
        {
            using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
            {

                return Conn.ExecuteScalar("SELECT ISNULL(MAX(CAST(LocationId AS Int)),0)+1 FROM TBL_LOCATIONS WHERE LocationId NOT LIKE '%[a-z]%' AND ISNUMERIC(LocationId) = 1").ToString();
            }
        }
        private string GetNewLCode(int level, string warehouse, string parent)
        {
            using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
            {

                var code = Conn.ExecuteScalar("SELECT ISNULL(MAX(CAST(LocationCode AS Int)),0)+1 FROM TBL_LOCATIONS WHERE LocationCode NOT LIKE '%[a-z]%' AND ISNUMERIC(LocationCode) = 1 AND Level=" + level + " AND Warehouse='" + warehouse + "' AND ParentLocation='" + parent + "'").ToString();
                if (code.Length == 1) { code = "0" + code; }
                return code;
            }
        }
        private string GetNewCellRowCode(int Level, string warehouse,string parent)
        {
            using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
            {
                Conn.Open();
               
                var C = Conn.ExecuteScalar("SELECT ISNULL(MAX(CAST(cellRowCode AS Int)),0)+1 FROM TBL_LOCATIONS WHERE cellRowCode NOT LIKE '%[a-z]%' AND ISNUMERIC(cellRowCode) = 1 AND Level="+Level+ "AND Warehouse='" + warehouse + "' AND ParentLocation='" + parent + "'").ToString();
                if (C.Length == 1) { C = "0" + C; }
               
                return C;
            }
        }
        string CellRCode = "";
        public override void SaveMethod(object obj)
        {
            if (selectedLocation == null) { MessageBox.Show("Invalid Data."); return; }
            if (LocationObj.Level > Settings.LocationLevelLimit) { MessageBox.Show("Level exceed the Level Limits"); return; }
            if (autogenerateCode == false && _action == ButtonAction.New)
            {
                if (string.IsNullOrEmpty(LocationObj.LocationCode) || string.IsNullOrEmpty(LocationObj.LocationName)) { return; }
                if (LocationObj.Level == Settings.LocationLevelLimit) {
                    if (validateCellLCode(LocationObj.LocationCode, LocationObj.Level, LocationObj.Warehouse) == false) { return; }
                }
                else
                {
                    if (validateLCode(LocationObj.LocationCode, LocationObj.Level, LocationObj.Warehouse, LocationObj.ParentLocation) == false) { return; }
                }
            }

            try
            {
                //if (MessageBox.Show("You are about to save current Location. Do you want to proceed?", "Save Data", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                //    return;
                if (_action == ButtonAction.New)
                {
                    if (autogenerateCode == true)
                    {
                        if (noOfChild == 0)
                        {
                            noOfChild = 1;
                        }
                        for (int i = 0; i < noOfChild; i++)
                        {
                            var LCode = "";
                            var cellRow = "";
                            if (LocationObj.Level == Settings.LocationLevelLimit)
                            {
                                
                                var F = (LocationObj.Parent.Parent.Parent as Location).LocationCode;
                                var R = (LocationObj.Parent.Parent as Location).LocationCode;
                                var S = (LocationObj.Parent as Location).LocationCode;
                                cellRow = GetNewCellRowCode(LocationObj.Level,LocationObj.Warehouse,LocationObj.ParentLocation);
                                getUniqueCellRowCode(F+R+S,cellRow);
                                if (string.IsNullOrEmpty(CellRCode)) { MessageBox.Show("Error On generating Code"); break;}
                                LCode = F + R + S + CellRCode;

                               
                            }
                            else
                            {
                                LCode = GetNewLCode(LocationObj.Level, LocationObj.Warehouse, LocationObj.ParentLocation);
                            }
                            var L = new Location() { LocationId = GetNewLId(), LocationCode = LCode, LocationName = LCode + "(" + LevelLabel(LocationObj.Level) + ")", ParentLocation = LocationObj.ParentLocation, Level = LocationObj.Level, Warehouse = LocationObj.Warehouse, Path = selectedLocation.Path + "\\" + LCode ,cellRowCode= CellRCode };
                            using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
                            {
                                Conn.Open();
                                Conn.Query("INSERT INTO TBL_LOCATIONS(LocationId,LocationCode, LocationName, ParentLocation, Level, Warehouse, Path,cellRowCode) VALUES(@LocationId,@LocationCode,@LocationName,@ParentLocation,@Level,@Warehouse, @Path,@cellRowCode)", L);
                            }
                        }

                    }
                    else
                    {
                        LocationObj.Path = selectedLocation.Path + "\\" + LocationObj.LocationCode;
                        LocationObj.LocationId = GetNewLId();
                        using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
                        {
                            Conn.Open();
                            Conn.Query("INSERT INTO TBL_LOCATIONS(LocationId,LocationCode, LocationName, ParentLocation, Level, Warehouse, Path) VALUES(@LocationId,@LocationCode,@LocationName,@ParentLocation,@Level,@Warehouse, @Path)", LocationObj);
                        }
                    }
                    MessageBox.Show(LevelLabel(LocationObj.Level)+" Added Sucessfully...");
                }
                else if (_action == ButtonAction.Edit)
                {
                    LocationObj.Path=(LocationObj.Parent as Location).Path + "\\" + LocationObj.LocationCode;
                    using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
                    {
                        Conn.Open();
                        Conn.Query("UPDATE TBL_LOCATIONS SET LocationCode=@LocationCode,LocationName=@LocationName,ParentLocation=@ParentLocation,Level=@Level,Warehouse=@Warehouse,Path=@Path WHERE LocationId=@LocationId", LocationObj);
                        MessageBox.Show("Selected Location is Updated Sucessfully...");
                    }
                }
                LocationTreeMaker();
                ExecuteUndo(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error On Saving..." + ex.Message);
            }
        }
        private void getUniqueCellRowCode(string LCodeWithoutRowCode,string cellRowCode) {
            var C = cellRowCode;
            if (validateCellLCode(LCodeWithoutRowCode+cellRowCode, LocationObj.Level, LocationObj.Warehouse) == false)
            {
                 C = (Convert.ToInt32(cellRowCode) + 1).ToString();
                if (C.Length == 1) { C = "0" + C; }
                getUniqueCellRowCode(LCodeWithoutRowCode,C);
            }
            else {
                CellRCode=C;
            }
        }
        private bool validateLCode(string code, int level, string warehouse, string parent)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    var result = Conn.ExecuteScalar("SELECT LocationId from TBL_LOCATIONS WHERE Level=" + level + " AND LocationCode='" + code + "' AND Warehouse='" + warehouse + "' AND ParentLocation='" + parent + "'");
                    if (result != null) { MessageBox.Show("Code Duplication Error...Please Choose another Code"); return false; }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error ..." + ex.Message);
                return false;
            }
        }
        private bool validateCellLCode(string code, int level, string warehouse) {
            try
            {
                using (SqlConnection Conn = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    var result = Conn.ExecuteScalar("SELECT LocationId from TBL_LOCATIONS WHERE LocationCode='" + code + "' AND Warehouse='" + warehouse + "'");
                    if (result != null) { if (autogenerateCode == false) { MessageBox.Show("Code Duplication Error...Please Choose another Code"); } return false; }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error..." + ex.Message);
                return false;
            }
        }
    }
}
