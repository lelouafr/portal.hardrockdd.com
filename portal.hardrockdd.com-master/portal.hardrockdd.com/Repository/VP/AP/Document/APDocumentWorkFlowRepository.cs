//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.AP
//{
//    public class APDocumentWorkFlowRepository : IDisposable
//    {
//        public APDocumentWorkFlowRepository()
//        {

//        }
       

        
//        public static void GenerateWorkFlow(APDocument request, string userIds = "")
//        {
//            //if (request == null) throw new System.ArgumentNullException(nameof(request));
//            //if (userIds == null) throw new System.ArgumentNullException(nameof(userIds));


//            //using var db = new VPContext();

//            //using var empRepo = new WP.WebUserRepository();
//            //var curUser = empRepo.GetUser(StaticFunctions.GetUserId());
//            //var emp = curUser.Employee.FirstOrDefault();
//            //var timeStamp = DateTime.Now;

//            //foreach (var workFlow in request.WorkFlows.Where(f => f.Active == "Y").ToList())
//            //{
//            //    workFlow.Active = "N";
//            //    if (workFlow.AssignedTo == curUser.Id)
//            //    {
//            //        workFlow.CompletedOn = timeStamp;
//            //        workFlow.AssignedStatus = request.Status > workFlow.Status ? 2 : 1;
//            //    }
//            //}

//            //List<WebUser> EmailList = new List<WebUser>();
//            //WebUser user;
//            //switch (request.Status)
//            //{
//            //    case DB.APDocumentStatusEnum.New:
//            //        EmailList.Add(curUser);
//            //        var webUsers = db.WebUserRoles.Where(f => f.RoleId == "APDocument").ToList();
//            //        foreach (var webuser in webUsers)
//            //        {
//            //            EmailList.Add(webuser.User);
//            //        }

//            //        break;
//            //    case DB.APDocumentStatusEnum.Reviewed:
//            //    case DB.APDocumentStatusEnum.LinesAdded:
//            //    case DB.APDocumentStatusEnum.Filed:
//            //    case DB.APDocumentStatusEnum.Error:
//            //        //EmailList.Add(emp.Supervisor.WebUser);

//            //        //user = empRepo.GetUserbyEmail("glen.lewis@hardrockdd.com");
//            //        //EmailList.Add(user);

//            //        if (request.APCo == 10)
//            //        {
//            //            user = empRepo.GetUserbyEmail("brandi@raymondconstruction.net");
//            //            if (user != null)
//            //                EmailList.Add(user);
//            //        }
//            //        else
//            //        {

//            //        }
//            //        user = empRepo.GetUserbyEmail("trudy.moore@hardrockdd.com");
//            //        EmailList.Add(user);

//            //        //user = empRepo.GetUserbyEmail("janet.schrandt@hardrockdd.com");
//            //        //EmailList.Add(user);

//            //        user = empRepo.GetUserbyEmail("bobby.hoover@hardrockdd.com");
//            //        EmailList.Add(user);

//            //        user = empRepo.GetUserbyEmail("tj.wheeler@hardrockdd.com");
//            //        EmailList.Add(user);
//            //        break;
//            //    case DB.APDocumentStatusEnum.Duplicate:
//            //    case DB.APDocumentStatusEnum.Processed:
//            //    case DB.APDocumentStatusEnum.Canceled:
//            //        break;
//            //    case DB.APDocumentStatusEnum.RequestedInfo:
//            //        var userList = userIds.Split('|');
//            //        foreach (var userid in userList)
//            //        {
//            //            user = empRepo.GetUser(userid);
//            //            EmailList.Add(user);
//            //        }
//            //        if (request.APCo == 10)
//            //        {
//            //            user = empRepo.GetUserbyEmail("brandi@raymondconstruction.net");
//            //            if (user != null)
//            //                EmailList.Add(user);
//            //        }
//            //        user = empRepo.GetUserbyEmail("trudy.moore@hardrockdd.com");
//            //        EmailList.Add(user);

//            //        //user = empRepo.GetUserbyEmail("bobby.hoover@hardrockdd.com");
//            //        //EmailList.Add(user);

//            //        //user = empRepo.GetUserbyEmail("tj.wheeler@hardrockdd.com");
//            //        //EmailList.Add(user);

//            //        break;
//            //    default:
//            //        break;
//            //}

//            //foreach (var email in EmailList)
//            //{
//            //    var workFlow = new APDocumentWorkFlow
//            //    {
//            //        APCo = request.APCo,
//            //        DocId = request.DocId,
//            //        LineId = request.WorkFlows
//            //                .DefaultIfEmpty()
//            //                .Max(f => f == null ? 0 : f.LineId) + 1,
//            //        Status = (int)request.Status,
//            //        StatusDate = timeStamp,
//            //        AssignedTo = email.Id,
//            //        AssignedOn = timeStamp,
//            //        AssignedStatus = 0,
//            //        CreatedBy = curUser.Id,
//            //        CreatedOn = timeStamp,
//            //        Active = "Y"
//            //    };
//            //    request.WorkFlows.Add(workFlow);
//            //}
//        }
        
//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~APDocumentWorkFlowRepository()
//        {
//            // Finalizer calls Dispose(false)
//            Dispose(false);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                //if (db != null)
//                //{
//                //    db.Dispose();
//                //    db = null;
//                //}
//            }
//        }
//    }
//}
