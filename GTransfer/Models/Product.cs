using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

using System.Collections.ObjectModel;
using Dapper;
using GTransfer.Library;
using GTransfer.Interfaces;
using ImsPosLibrary.Models;

namespace GTransfer.Models
{
    public class ProductType : BaseModel
    {
        private string _PTYPENAME;
        private short _NATURETYPE;
        private byte _PTYPEID;
        public byte PTYPEID { get { return _PTYPEID; } set { _PTYPEID = value; OnPropertyChanged("PTYPEID"); } }
        public short NATURETYPE { get { return _NATURETYPE; } set { _NATURETYPE = value; OnPropertyChanged("NATURETYPE"); } }
        public string PTYPENAME { get { return _PTYPENAME; } set { _PTYPENAME = value; OnPropertyChanged("PTYPENAME"); } }
    }
    public enum TypeEnum
    {
        G = 71, A = 65
    }
    public enum ProductStatus
    {
        Normal = 0, Discontinued = 1, Disabled = 2
    }
    public class Product : TMenuitem, ITreeNode
    {
       
        
        #region members
        private string _ParentName;
        private TypeEnum _Type;
        
        private int _NatureType;
        private bool _isTrading;
        private ProductStatus _IsDiscontinue;
        private UnitAndQty _AltUnit;
        private ObservableCollection<AlternateUnit> _AlternateUnits;

        
        

        #region Label Alias Member
        private string _MajorGroupAlias = "Major Group";
        private string _ParentAlias = "Parent";
        private string _ProductCodeAlias = "Product Code";
        private string _DescriptionAAlias = "Description A";
        private string _DescriptionBAlias = "Description B";
        private string _ProductTypeAlias = "Product Type";
        private string _UnitAlias = "Unit";
        private string _DiscountModeAlias = "Discount Mode";
        private string _MarginAlias = "Margin";
        private string _SupProductCodeAlias = "Sup Product Code";
        private string _IsTradingAlias = "Is Trading";
        private string _IsUnknownItemAlias = "Is UnknownItem";
        private string _VATItemAlias = "VAT Item";
        private string _FCodeAlias = "FCode";
        private string _DiscountRateAlias = "Discount Rate";
        private string _DiscountAmountAlias = "Discount Amount";
        private string _IsDiscontinuedAlias = "Is Discontinued";
        private string _SupplierAccountAlias = "Supplier Account";
        private string _SalesAccountAlias = "Sales Account";
        private string _SalesReturnAccountAlias = "Sales Return Account";
        private string _PurchaseAccountAlias = "Purchase Account";
        private string _PurchaseReturnAccountAlias = "Purchase Return Account";

        #endregion


        #endregion

        private Location _Location;
        public Location Location { get { return _Location; }set { _Location = value;OnPropertyChanged("Location"); } }
        private string _LocationId;
        public string LocationId { get { return _LocationId; } set { _LocationId = value; OnPropertyChanged("LocationId"); } }
        private string _PreviousLocation;
        public string PreviousLocation { get { return _PreviousLocation; } set { _PreviousLocation = value; OnPropertyChanged("PreviousLocation"); } }
        private int _LocationVsItemId;
        public int LocationVsItemId { get { return _LocationVsItemId; } set { _LocationVsItemId = value; OnPropertyChanged("LocationVsItemId"); } }
        

        #region Property
        public ObservableCollection<AlternateUnit> AlternateUnits
        {
            get { return _AlternateUnits; }
            set { _AlternateUnits = value; OnPropertyChanged("AlternateUnits"); }
        }
        //public Array ProductTypeList { get { return Enum.GetValues(typeof(ProductType)); } }
        //public Array TypeList { get { return Enum.GetValues(typeof(TypeEnum)); } }
        //public Array ProductStatusList { get { return Enum.GetValues(typeof(ProductStatus)); } }
        //public TypeEnum Type { get { return _Type; } 
        //    set { 
        //         _Type = value;
        //         if (value == TypeEnum.A) TYPE = "A";
        //         if (value == TypeEnum.G) TYPE = "G";
        //         OnPropertyChanged("TYPE"); 
        //        } 
        //}
        public int    NatureType { get { return _NatureType; } set { _NatureType = value; OnPropertyChanged("NatureType"); } }
        public ProductStatus ISDISCONTINUE { get { return _IsDiscontinue; } set { _IsDiscontinue = value; OnPropertyChanged("DISCONTINUE"); } }

        [IgnoreProperty(true)]
        public UnitAndQty AltUnit { get { return _AltUnit; } set { _AltUnit = value; OnPropertyChanged("AltUnit"); } }
        //MCODE, PARENT, TYPE, PTYPE, MGROUP, FCODE, MENUCODE, DESCA, DESCB, UNIT, VAT, DISMODE, DISRATE, DISAMOUNT, MARGIN, DISCONTINUE, SUPCODE, SAC, PAC, SRAC, PRAC, ISUNKNOWN, CRDATE, EDATE
        #endregion

        #region Constructors
        public Product()
        {
            
            AltUnit = new UnitAndQty();
            AlternateUnits = new ObservableCollection<AlternateUnit>();
        }
        public Product(string MCode, bool DefaultAltUnitByPrate = false)
            : this()
        {
            GetProduct(MCode, DefaultAltUnitByPrate);
            
        }

        
        public void GetProduct(string Mcode,bool DefaultAltUnitByPrate=false)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    conn.Open();
                    //NEED TO CHANGE THE SQL QUERY AS PER LOADING REQUIREMENT WITH ITEMRATE , RATEGROUP ETC.
                    //this.MCODE = MCode;
                    string OrderCondition = DefaultAltUnitByPrate ? "ISDEFAULTPRATE" : "ISDEFAULT";
                    string qstring;
                    qstring = @"SELECT MCODE,BARCODE, PARENT, TYPE, PTYPE, MGROUP, FCODE, MENUCODE, DESCA, DESCB, BASEUNIT, VAT, DISMODE, DISRATE, DISAMOUNT, MARGIN, 
DISCONTINUE, SUPCODE, SAC, PAC, SRAC, PRAC, ISUNKNOWN, CRDATE, EDATE,B.NATURETYPE,A.PRATE_A,A.PRATE_A PRATE,A.VAT,A.HASSERVICECHARGE  FROM MENUITEM A INNER JOIN PTYPE B ON A.PTYPE=B.PTYPEID WHERE MCODE=@MCODE ;" +
@"SELECT * FROM (SELECT MCODE,BASEUNIT ALTUNIT,1 CONFACTOR,RATE_A RATE,1 AS ISDEFAULT , BARCODE BRCODE,PRATE_A PRATE,1 AS ISDEFAULTPRATE FROM MENUITEM WHERE MCODE=@MCODE 
UNION ALL select MCODE,ALTUNIT,CONFACTOR,RATE,CASE WHEN ISDEFAULT=1 THEN 2 ELSE 0 END ISDEFAULT,BRCODE ,PRATE,CASE WHEN ISDEFAULTPRATE=1 THEN 2 ELSE 0 END ISDEFAULTPRATE  FROM MULTIALTUNIT WHERE ISDISCONTINUE=0 AND MCODE=@MCODE
) AS A ORDER BY ISDEFAULTPRATE DESC ;";

                    var MultiResult = conn.QueryMultiple(qstring, new { MCODE = Mcode });
                    var Prd = MultiResult.Read<Product>().SingleOrDefault();
                    var altunits = MultiResult.Read<AlternateUnit>().ToList();
                    GlobalClass.CopyPropertyValuesOnlyPresent(Prd as Product, this);
                    this.AlternateUnits = new ObservableCollection<AlternateUnit>(altunits);
                          
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        
//        public Product(string MCode, bool DefaultAltUnitByPrate = false)
//            : this()
//        {

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
//                using (SqlCommand cmd = conn.CreateCommand())
//                {
//                    //NEED TO CHANGE THE SQL QUERY AS PER LOADING REQUIREMENT WITH ITEMRATE , RATEGROUP ETC.
//                    this.MCODE = MCode;
//                    conn.Open();
//                    cmd.CommandText = @"SELECT MCODE, PARENT, TYPE, PTYPE, MGROUP, FCODE, MENUCODE, DESCA, DESCB, BASEUNIT UNIT, VAT, DISMODE, DISRATE, DISAMOUNT, MARGIN, 
//                                        DISCONTINUE, SUPCODE, SAC, PAC, SRAC, PRAC, ISUNKNOWN, CRDATE, EDATE,B.NATURETYPE,A.PRATE_A PRATE FROM MENUITEM A INNER JOIN PTYPE B ON A.PTYPE=B.PTYPEID WHERE MCODE='" + MCODE + "'";
//                    using (SqlDataReader dr = cmd.ExecuteReader())
//                    {
//                        while (dr.Read())
//                        {

//                            PARENT = dr["PARENT"].ToString();
//                            TYPE = (dr["TYPE"]).ToString();
//                            PTYPE = Convert.ToByte (dr["PTYPE"]);
//                            MGROUP = dr["MGROUP"].ToString();
//                            FCODE = GParse.ToDecimal (dr["FCODE"]);
//                            MENUCODE = dr["MENUCODE"].ToString();
//                            DESCA = dr["DESCA"].ToString();
//                            DESCB = dr["DESCB"].ToString();
//                            BASEUNIT  = dr["UNIT"].ToString();
//                            VAT = Convert.ToByte (dr["VAT"]);
//                            DISMODE = dr["DISMODE"].ToString();
//                            DISRATE = GParse.ToDecimal(dr["DISRATE"]);
//                            MARGIN = GParse.ToDecimal(dr["MARGIN"]);
//                            ISUNKNOWN = Convert.ToByte (dr["ISUNKNOWN"]);
//                            DISCONTINUE = Convert.ToByte (dr["DISCONTINUE"]);
//                            SUPCODE = dr["SUPCODE"].ToString();
//                            SAC = dr["SAC"].ToString();
//                            SRAC = dr["SRAC"].ToString();
//                            PAC = dr["PAC"].ToString();
//                            PRAC = dr["PRAC"].ToString();
//                            NatureType = Convert.ToByte(dr["NATURETYPE"]);
//                            PRATE_A  = GParse.ToDecimal(dr["Prate"]);
//                        }
//                        dr.Close();

//                    }
//                    string OrderCondition = DefaultAltUnitByPrate ? "ISDEFAULTPRATE" : "ISDEFAULT";
//                    cmd.CommandText = @"SELECT * FROM (
//                        SELECT BASEUNIT UNIT,1 CONFACTOR,RATE_A RATE,1 AS ISDEFAULT , BARCODE BRCODE,PRATE_A PRATE,1 AS ISDEFAULTPRATE FROM MENUITEM WHERE MCODE='" + MCode + @"'
//                        UNION ALL
//                        select ALTUNIT,CONFACTOR,RATE,CASE WHEN ISDEFAULT=1 THEN 2 ELSE 0 END ISDEFAULT,BRCODE ,PRATE,CASE WHEN ISDEFAULTPRATE=1 THEN 2 ELSE 0 END ISDEFAULTPRATE  FROM MULTIALTUNIT WHERE ISDISCONTINUE=0 AND MCODE='" + MCode + @"'
//                        ) AS A ORDER BY " + OrderCondition + " DESC";

//                    using (SqlDataReader dr = cmd.ExecuteReader())
//                    {
//                        while (dr.Read())
//                        {
//                            AlternateUnit Aunit = new AlternateUnit();
//                            Aunit.ALTUNIT = dr["UNIT"].ToString();
//                            Aunit.CONFACTOR = GParse.ToDecimal(dr["CONFACTOR"]);
//                            Aunit.BRCODE = dr["BRCODE"].ToString();
//                            Aunit.PRATE = GParse.ToDecimal(dr["PRATE"]);
//                            Aunit.RATE = GParse.ToDecimal(dr["RATE"]);
//                            Aunit.ISDEFAULT = GParse.ToBool(dr["ISDEFAULT"]);
//                            Aunit.ISDEFAULTPRATE = GParse.ToBool(dr["ISDEFAULTPRATE"]);
//                            AlternateUnits.Add(Aunit);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
        public Product(string Parnt, TypeEnum typ)
        {
            //DataTable dt;
            //if (typ == TypeEnum.A)
            //    dt = DataConnection.getData("Select 'M' + Convert(varchar(20), max(convert(numeric,right(mcode,len(mcode)-1)))+1) maxcode from Product_master where type='A' ");
            //else
            //    dt = DataConnection.getData("Select 'PRG' + Convert(varchar(20), max(convert(numeric,right(mcode,len(mcode)-3)))+1) maxcode from Product_master where type='G' ");
            //string Maxcode = dt.Rows[0]["maxcode"].ToString();
            //this._Parent = Parnt;
            //this._Type = typ;
            //MCode = Maxcode;
        }

        #endregion

        public UnitAndQty GetAltUnit()
        {
            return _AltUnit;
        }

        #region Overidable base methods and properties
         
        #endregion

        #region AddedByAmir

        private string _pdismode;
        private decimal _pdisrate;
        private Product _MGroup;
        private byte _rowChanged;
        private bool _IsMatch;
        private ObservableCollection<ITreeNode> _Children;
        private ITreeNode _IParent;
        private bool _IsSelected;
        private bool _IsExpanded;
        private string _PMCAT;
        private string _MCAT1;
        private string _PMCAT1;
        public string GetPath
        {
            get
            {
                if (Parent != null)
                    return (Parent as Product).PATH + "\\" + DESCA;
                else
                    return this.DESCA;
            }
        }

        [IgnoreProperty(true)]
        public string PDisMode { get { return _pdismode; } set { _pdismode = value; OnPropertyChanged("PDisMode"); } }
        [IgnoreProperty(true)]
        public decimal PDisrate { get { return _pdisrate; } set { _pdisrate = value; OnPropertyChanged("PDisrate"); } }

        public string MCAT1 { get { return _MCAT1; } set { _MCAT1 = value; OnPropertyChanged("MCAT1"); } }

        [IgnoreProperty(true)]
        public string PMCAT { get { return _PMCAT; } set { _PMCAT = value; OnPropertyChanged("PMCAT"); } }
        [IgnoreProperty(true)]
        public string PMCAT1 { get { return _PMCAT1; } set { _PMCAT1 = value; OnPropertyChanged("PMCAT1"); } }
        [IgnoreProperty(true)]
        public Array TypeList { get { return Enum.GetValues(typeof(TypeEnum)); } }
        [IgnoreProperty(true)]
        public Product MajorGroup { get { return _MGroup; } set { _MGroup = value; OnPropertyChanged("MajorGroup"); } }
        [IgnoreProperty(true)]
        public byte RowChanged { get { return _rowChanged; } set { _rowChanged = value; OnPropertyChanged("RowChanged"); } }

      
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
        public string NodeName { get { return DESCA; } }

        [IgnoreProperty(true)]
        public string NodeID { get { return MCODE; } }

        [IgnoreProperty(true)]
        public bool IsMatch { get { return _IsMatch; } set { _IsMatch = value; OnPropertyChanged("IsMatch"); } }
        #endregion

        #region ITreeNodeMethods
        public bool IsCriteriaMatched(string criteria)
        {
            return String.IsNullOrEmpty(criteria) || (DESCA.ToUpper()).Contains(criteria.ToUpper());
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

        #endregion

    }

    
}
