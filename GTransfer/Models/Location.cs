using GTransfer.Interfaces;
using GTransfer.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTransfer.Models
{
    public class Location:BaseModel,ITreeNode
    {
        private string _LocationId;
        private string _LocationCode;
        private string _LocationName;
        private string _ParentLocation;
        private int _Level;
        private string _Warehouse;
        private string _cellRowCode;

        private ObservableCollection<ITreeNode> _Children;
        private ITreeNode _Parent;
        private bool _IsExpanded;
        private bool _IsSelected;
        private bool _IsMatch;
        private bool _IsGroup;
        private string _Path;

        public string cellRowCode { get { return _cellRowCode; }set { _cellRowCode = value;OnPropertyChanged("cellRowCode"); } }
        public string LocationId { get { return _LocationId; }set { _LocationId = value;OnPropertyChanged("LocationId"); } }
        public string LocationCode { get { return _LocationCode; }set { _LocationCode = value;OnPropertyChanged("LocationCode"); } }
        public string LocationName { get { return _LocationName; } set { _LocationName = value; OnPropertyChanged("LocationName"); } }
        public string ParentLocation { get { return _ParentLocation; } set { _ParentLocation = value; OnPropertyChanged("ParentLocation"); } }
        public int Level { get { return _Level; } set { _Level = value; OnPropertyChanged("Level"); } }
        public string Warehouse { get { return _Warehouse; } set { _Warehouse = value; OnPropertyChanged("Warehouse"); } }
        public string Path { get { return _Path; } set { _Path = value; OnPropertyChanged("Path"); } }

        #region ITreeNodeProperties
        [IgnoreProperty(true)]
        public bool IsGroup { get { return _IsGroup; } }

        [IgnoreProperty(true)]
        public bool IsExpanded { get { return _IsExpanded; } set { _IsExpanded = value; OnPropertyChanged("IsExpanded"); } }
        [IgnoreProperty(true)]

        public bool IsSelected { get { return _IsSelected; } set { _IsSelected = value; OnPropertyChanged("IsSelected"); } }
        [IgnoreProperty(true)]

        public ITreeNode Parent { get { return _Parent; } set { _Parent = value; OnPropertyChanged("Parent"); } }

        [IgnoreProperty(true)]
        public string ParentID { get { return ParentLocation.ToString(); } }

        [IgnoreProperty(true)]
        public ObservableCollection<ITreeNode> Children { get { return _Children; } set { _Children = value; OnPropertyChanged("Children"); } }

        [IgnoreProperty(true)]
        public string NodeName { get { return LocationName; } }

        [IgnoreProperty(true)]
        public string NodeID { get { return LocationId.ToString(); } }

        [IgnoreProperty(true)]
        public bool IsMatch { get { return _IsMatch; } set { _IsMatch = value; OnPropertyChanged("IsMatch"); } }
        #endregion

        #region ITreeNodeMethods
        public bool IsCriteriaMatched(string criteria)
        {
            return String.IsNullOrEmpty(criteria) || (LocationName.ToUpper()).Contains(criteria.ToUpper());
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
                //amir ExpandParent(CurItem.Parent);
            }
        }
        #endregion

    }
    public class ItemVsLocation {
    public int Id { get; set; }
    public string LID { get; set; }
    public string MCODE { get; set; } }
}
