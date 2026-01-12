//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Code;
//using portal.Repository.VP.WP;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.BD
//{
//    public static class BidWorkFlowAssignmentRepository
//    {
//        public static void GenerateWorkFlow(Bid bid, VPContext db)
//        {

//            if (bid == null)
//            {
//                throw new ArgumentNullException(nameof(bid));
//            }
//            //bid = db.Bids.Where(f => f.Co == bid.Co && f.BidId == bid.BidId).FirstOrDefault();
//            bid.GenerateWorkFlowAssignments();

//            //var timeStamp = DateTime.Now;

//            //foreach (var workFlow in bid.WorkFlows.Where(f => f.PackageId == null && f.Active == "Y").ToList())
//            //{
//            //    workFlow.Active = "N";
//            //    if (workFlow.AssignedTo == StaticFunctions.GetUserId())
//            //    {
//            //        workFlow.CompletedOn = timeStamp;
//            //        workFlow.AssignedStatus = bid.StatusId > workFlow.Status ? 2 : 1;
//            //    }
//            //    if (workFlow.Comments != null)
//            //    {
//            //        var line = ForumLineRepository.Init(bid.Forum);
//            //        line.CreatedBy = workFlow.AssignedTo;
//            //        line.CreatedOn = workFlow.AssignedOn;
//            //        line.Comment = workFlow.Comments;
//            //        line.HtmlComment = workFlow.Comments;

//            //        bid.Forum.Lines.Add(line);
//            //    }
//            //}

//            //foreach (var package in bid.Packages)
//            //{
//            //    var workFlows = bid.WorkFlows.Where(f => f.PackageId == package.PackageId && f.Active == "Y").ToList();
//            //    foreach (var workFlow in workFlows)
//            //    {
//            //        workFlow.Active = "N";
//            //        if (workFlow.AssignedTo == StaticFunctions.GetUserId())
//            //        {
//            //            workFlow.CompletedOn = timeStamp;
//            //            workFlow.AssignedStatus = bid.StatusId > workFlow.Status ? 2 : 1;
//            //        }
//            //        if (workFlow.Comments != null)
//            //        {
//            //            var line = ForumLineRepository.Init(bid.Forum);
//            //            line.CreatedBy = workFlow.AssignedTo;
//            //            line.CreatedOn = workFlow.AssignedOn;
//            //            line.Comment = workFlow.Comments;
//            //            line.HtmlComment = workFlow.Comments;

//            //            bid.Forum.Lines.Add(line);
//            //        }
//            //    }

//            //    var emailList = EmailList(package, db);
//            //    foreach (var email in emailList)
//            //    {
//            //        var user = db.WebUsers.Where(f => f.Email.ToLower() == email.ToLower()).FirstOrDefault();
//            //        if (user != null)
//            //        {
//            //            var workFlow = new BidWorkFlowAssignment
//            //            {
//            //                BDCo = package.BDCo,
//            //                BidId = package.BidId,
//            //                PackageId = package.PackageId,
//            //                Status = (int)bid.Status,
//            //                LineId = bid.WorkFlows.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
//            //                StatusDate = timeStamp,
//            //                AssignedTo = user.Id,
//            //                AssignedOn = timeStamp,
//            //                AssignedStatus = 0,
//            //                CreatedBy = StaticFunctions.GetUserId(),
//            //                CreatedOn = timeStamp,
//            //                Active = "Y"
//            //            };
//            //            bid.WorkFlows.Add(workFlow);
//            //        }
//            //    }
//            //}
//            //if (bid.Packages.Count == 0)
//            //{
//            //    var workFlow = new BidWorkFlowAssignment
//            //    {
//            //        BDCo = bid.BDCo,
//            //        BidId = bid.BidId,
//            //        Status = (int)bid.Status,
//            //        LineId = bid.WorkFlows.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineId) + 1,
//            //        StatusDate = timeStamp,
//            //        AssignedTo = StaticFunctions.GetUserId(),
//            //        AssignedOn = timeStamp,
//            //        AssignedStatus = 0,
//            //        CreatedBy = StaticFunctions.GetUserId(),
//            //        CreatedOn = timeStamp,
//            //        Active = "Y"
//            //    };
//            //    bid.WorkFlows.Add(workFlow);
//            //}
//            //db.SaveChanges();
//        }

//        //public static List<string> EmailList(BidPackage bidPackage, VPContext db)
//        //{
//        //    if (bidPackage == null) throw new System.ArgumentNullException(nameof(bidPackage));

//        //    List<string> emailList = new List<string>();
//        //    var curUser =StaticFunctions.GetCurrentUser();


//        //    switch (bidPackage.Bid.Status)
//        //    {
//        //        case DB.BidStatusEnum.Draft:
//        //            if (bidPackage.Bid.CreatedUser == null)
//        //            {
//        //                emailList.Add(curUser.Email);
//        //            }
//        //            else
//        //            {
//        //                emailList.Add(bidPackage.Bid.CreatedUser.Email);
//        //            }
//        //            break;
//        //        case DB.BidStatusEnum.Estimate:
//        //            var projectDivision = db.ProjectDivisions.FirstOrDefault(w => w.DivisionId == bidPackage.DivisionId);
//        //            var distroGroup = projectDivision != null ? projectDivision.DistributionGroupId : "Estimate";
//        //            emailList.AddRange(EmailHelper.GetEmailList(distroGroup, "bPMPM"));
//        //            break;
//        //        case DB.BidStatusEnum.SalesReview:
//        //            emailList.Add("cory.baker@hardrockdd.com");
//        //            emailList.Add("chris.jones@hardrockdd.com");
//        //            emailList.Add(bidPackage.Bid.CreatedUser.Email);
//        //            break;
//        //        case DB.BidStatusEnum.FinalReview:
//        //            emailList.AddRange(EmailHelper.GetEmailList("ReviewFinalBid", "bPMPM"));
//        //            break;
//        //        case DB.BidStatusEnum.Proposal:
//        //            emailList.Add("cory.baker@hardrockdd.com");
//        //            emailList.Add("chris.jones@hardrockdd.com");
//        //            emailList.Add(bidPackage.Bid.CreatedUser.Email);
//        //            break;
//        //        case DB.BidStatusEnum.PendingAward:
//        //            emailList.Add("cory.baker@hardrockdd.com");
//        //            emailList.Add("chris.jones@hardrockdd.com");
//        //            emailList.Add(bidPackage.Bid.CreatedUser.Email);
//        //            break;
//        //        case DB.BidStatusEnum.ContractReview:
//        //            emailList.Add("bobby.hoover@hardrockdd.com");
//        //            break;
//        //        case DB.BidStatusEnum.ContractApproval:
//        //            emailList.Add("robert.tipton@hardrockdd.com");
//        //            break;
//        //        case DB.BidStatusEnum.Awarded:
//        //            //emailList.Add("cory.baker@hardrockdd.com");
//        //            //emailList.Add("chris.jones@hardrockdd.com");
//        //            //emailList.Add(bidPackage.Bid.CreatedUser.Email);
//        //            break;
//        //        case DB.BidStatusEnum.NotAwarded:
//        //            //emailList.Add("cory.baker@hardrockdd.com");
//        //            //emailList.Add("chris.jones@hardrockdd.com");
//        //            //emailList.Add(bidPackage.Bid.CreatedUser.Email);
//        //            break;
//        //        case DB.BidStatusEnum.Canceled:
//        //            //emailList.Add("cory.baker@hardrockdd.com");
//        //            //emailList.Add("chris.jones@hardrockdd.com");
//        //            //emailList.Add(bidPackage.Bid.CreatedUser.Email);
//        //            break;
//        //        default:
//        //            break;
//        //    }
//        //    return emailList;

//        //}

//    }
//}
