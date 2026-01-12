using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Google.Models.Map
{
    public class MapViewModel
    {
        public MapViewModel()
        {

        }

        public MapViewModel(WPMapSet mapSet)
        {
            MapSetId = mapSet.MapSetId;
            Coords = mapSet.Coords.Select(s => s.GPSCoords).ToList();
            Titles = mapSet.Coords.Select(s => s.Title).ToList();

            Locations = mapSet.Coords.Select(s => new MapViewLocationsModel(s)).ToList();
            Locations.FirstOrDefault().SetCenter = true;
        }

        public MapViewModel(List<string> coords, List<string> titles)
        {
            Coords = coords;
            Titles = titles;
        }
        [Key]
        public int MapSetId { get; set; }


        public List<string> Coords { get; }


        public List<string> Titles { get; }

        public List<MapViewLocationsModel> Locations { get; set; }
    }

    public class MapViewLocationsModel
    {
        public MapViewLocationsModel()
        {

        }

        public MapViewLocationsModel(WPMapCoord coords)
        {
            Title = coords.Title;
            Lat = coords.GeoCoordinate.Latitude;
            Long = coords.GeoCoordinate.Longitude;
            SetCenter = false;
        }
        public string Title { get; set; }

        public double? Lat { get; set; }

        public double? Long { get; set; }

        public bool SetCenter { get; set; }
    }
}