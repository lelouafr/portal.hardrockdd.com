using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class WPMapCoord
    {
        public VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }

        private GeoCoordinate _GeoCoordinate;

        public GeoCoordinate GeoCoordinate { 
            get
            {
                if (_GeoCoordinate == null)
                {
                    if (this.GPSLong == null && this.GPSLat == null && this.GPSCoords != null)
                    {
                        var coords = GPSCoords.Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
                        if (coords.Length>0)
                        {
                            this.GPSLat = decimal.TryParse(coords[0], out decimal gpslat) ? gpslat : 0;
                            this.GPSLong = decimal.TryParse(coords[1], out decimal gpslong) ? gpslong : 0;
                        }
                    }
                    if (this.GPSLong != null && this.GPSLat != null)
                        _GeoCoordinate = new GeoCoordinate((double)this.GPSLat, (double)this.GPSLong);
                    else
                        _GeoCoordinate = new GeoCoordinate(0, 0);


                }
                
                return _GeoCoordinate;
            }
            set
            {
                _GeoCoordinate = value;
            }
        }

        public static string BaseTableName { get { return "budWPMC"; } }
     
    }
}
