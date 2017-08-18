 using GTransfer.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using GTransfer.Models;

namespace GTransfer.Repository
{
    class LocationRepository : ObservableCollection<Location>
    {
        public LocationRepository()
        {
            using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
            {
                foreach (Location L in conn.Query<Location>("SELECT * FROM TBL_LOCATIONS WHERE Level='"+Settings.LocationLevelLimit+"' ORDER BY LocationName"))
                {
                    this.Add(L);
                }
            }
        }
    }
}
