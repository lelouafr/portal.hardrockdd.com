//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Linq;

//namespace portal.Repository.VP.DT
//{
//    public static class DailyShopTicketRepository 
//    {
        

//        //public static Models.Views.DailyTicket.DailyCrewTicketViewModel ProcessUpdate(Models.Views.DailyTicket.DailyCrewTicketViewModel model, DailyShopTicket updObj)
//        //{
//        //    if (model == null)
//        //        throw new System.ArgumentNullException(nameof(model));
//        //    if (updObj == null)
//        //        throw new System.ArgumentNullException(nameof(updObj));


//        //    if (updObj != null)
//        //    {
//        //        model.WorkDate = model.WorkDate.Year < 1900 ? (DateTime)updObj.WorkDate : model.WorkDate;

//        //        /****Write the changes to object****/
//        //        updObj.JobId = model.JobId;
//        //        updObj.CrewId = model.CrewId;
//        //        updObj.Comments = model.Comments;
//        //        updObj.WorkDate = model.WorkDate;
//        //    }
//        //    return new Models.Views.DailyTicket.DailyCrewTicketViewModel(updObj);
//        //}

//        //public static Models.Views.DailyTicket.DailyShopTicketViewModel ProcessUpdate(Models.Views.DailyTicket.DailyShopTicketViewModel model, DailyShopTicket updObj)
//        //{
//        //    if (model == null)
//        //    {
//        //        throw new System.ArgumentNullException(nameof(model));
//        //    }
//        //    if (updObj != null)
//        //    {
//        //        model.WorkDate = model.WorkDate.Year < 1900 ? (DateTime)updObj.WorkDate : model.WorkDate;

//        //        /****Write the changes to object****/
//        //        updObj.JobId = model.JobId;
//        //        updObj.CrewId = model.CrewId;
//        //        updObj.Comments = model.Comments;
//        //        updObj.WorkDate = model.WorkDate;
//        //        //updObj.DailyTicket.DailyEmployeeEntries.ToList().ForEach(e => e.JobId = updObj.JobId);
                
//        //    }
//        //    return new Models.Views.DailyTicket.DailyShopTicketViewModel(updObj);
//        //}
        
//    }
//}