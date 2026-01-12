//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.AR.Customer;
//using portal.Models.Views.JC.Industry;
//using portal.Models.Views.JC.Industry.Form;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.JC
//{
//    public static class IndustryRepository 
//    {       
//        public static IndustryViewModel ProcessUpdate(IndustryViewModel model, VPContext db)
//        {
//            if (model == null) throw new ArgumentNullException(nameof(model));
//            if (db == null) throw new ArgumentNullException(nameof(db));

//            var updObj = db.JCIndustries.FirstOrDefault(f => f.JCCo == model.JCCo && f.IndustryId == model.IndustryId);

//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.Description = model.Description;
//            }
//            return new IndustryViewModel(updObj);
//        }
       
//        public static IndustryInfoViewModel ProcessUpdate(IndustryInfoViewModel model, VPContext db)
//        {
//            if (model == null) throw new ArgumentNullException(nameof(model));
//            if (db == null) throw new ArgumentNullException(nameof(db));

//            var updObj = db.JCIndustries.FirstOrDefault(f => f.JCCo == model.JCCo && f.IndustryId == model.IndustryId);

//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.Description = model.Description;
//            }
//            return new IndustryInfoViewModel(updObj);
//        }

//    }
//}