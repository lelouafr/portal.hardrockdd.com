//using portal.Repository.VP.DT;
//using portal.Repository.VP.HR;
//using portal.Repository.VP.PR;
//using System.Collections.Generic;
//using System.Web.Mvc;
////using www.Repository.ViewPoint.HR;

//namespace portal.Models
//{
//    public static class SelectListModel
//    {
//        public enum SelectListTypeEnum
//        {
//            None = 0,
//            State = 1,
//            Race = 2,
//            LicenseClass = 3,
//            Form = 4,
//            GroundCondition = 5,
//            WeatherCondition = 6
//        }

//        public static List<SelectListItem> StateSelectList(string selected)
//        {
//            using var repo = new StateRepository();
//            return StateRepository.GetSelectList(repo.GetStates(), selected);
//        }

//        public static List<SelectListItem> FormSelectList(string selected)
//        {
//            using var repo = new DailyFormRepository();
//            return DailyFormRepository.GetSelectList(repo.GetDailyForms(), selected);
//        }

//        public static List<SelectListItem> GroundConditionSelectionList(string selected)
//        {
//            var results = new List<SelectListItem>
//            {
//                new SelectListItem(){ Value = "Dirt", Text = "Dirt", Selected = selected == "Dirt"},
//                new SelectListItem(){ Value = "Rock", Text = "Rock", Selected = selected == "Rock"}
//            };
//            return results;
//        }


//        public static List<SelectListItem> WeatherConditionSelectionList(string selected)
//        {
//            var results = new List<SelectListItem>
//            {
//                new SelectListItem(){ Value = "Clear",Text = "Clear", Selected = selected == "Clear" },
//                new SelectListItem(){ Value = "Partly Sunny",Text = "Partly Sunny", Selected = selected == "Partly Sunny" },
//                new SelectListItem(){ Value = "Cloudy/Overcast",Text = "Cloudy/Overcast", Selected = selected == "Cloudy/Overcast" },
//                new SelectListItem(){ Value = "Raining",Text = "Raining", Selected = selected == "Raining" },
//                new SelectListItem(){ Value = "Scattered Shower",Text = "Scattered Shower", Selected = selected == "Scattered Shower" },
//                new SelectListItem(){ Value = "Hot/Extreme Heat",Text = "Hot/Extreme Heatr", Selected = selected == "Hot/Extreme Heat" },
//                new SelectListItem(){ Value = "Ice/Snow",Text = "Ice/Snow", Selected = selected == "Ice/Snow" }
//            };
//            return results;
//        }

//        public static List<SelectListItem> RaceCodeSelectList(string selected)
//        {
//            using var repo = new RaceRepository();
//            return RaceRepository.GetSelectList(repo.GetRaces(1), selected);
//        }

//        public static List<SelectListItem> LicClassSelectList(string selected)
//        {
//            var results = new List<SelectListItem>()
//            {
//                new SelectListItem() { Value = "A", Text = "Class A", Selected = selected == "A" ? true : false },
//                new SelectListItem() { Value = "B", Text = "Class B", Selected = selected == "B" ? true : false },
//                new SelectListItem() { Value = "C", Text = "Class C", Selected = selected == "C" ? true : false },
//                new SelectListItem() { Value = "D", Text = "Class D", Selected = selected == "D" ? true : false }
//            };

//            return results;
//        }
//    }
//}