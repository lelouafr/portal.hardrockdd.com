//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.BD
//{
//    public class BidProposalScopeRepository
//    {

//        //public static BidProposalScope Init(Bid bid, DB.ScopeTypeEnum scopeTypeId)
//        //{
//        //    if (bid == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(bid));
//        //    }
//        //    var model = new BidProposalScope
//        //    {
//        //        BDCo = bid.BDCo,
//        //        BidId = bid.BidId,
//        //        ScopeTypeId = (byte)scopeTypeId,
//        //        Notes = string.Empty,
//        //        Title = string.Empty
//        //    };

//        //    return model;
//        //}

//        public static Models.Views.Bid.Forms.Proposal.Bid.ProposalScopeViewModel ProcessUpdate(Models.Views.Bid.Forms.Proposal.Bid.ProposalScopeViewModel model, VPContext db)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            var updObj = db.BidProposalScopes.FirstOrDefault(f => f.BDCo == model.BDCo && 
//                                                                  f.BidId == model.BidId && 
//                                                                  f.ScopeTypeId == (int)model.ScopeTypeId && 
//                                                                  f.LineId == model.LineId);

//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.Title = model.Title ?? string.Empty;
//                updObj.Notes = model.Notes ?? string.Empty;
//            }
//            return new Models.Views.Bid.Forms.Proposal.Bid.ProposalScopeViewModel(updObj);
//        }

//        //public static void CopyFromTemplate(Bid bid, VPContext db)
//        //{
//        //    if (bid == null || db == null)
//        //        return;

//        //    var template = db.Bids.FirstOrDefault(f => f.BDCo == 1 && f.BidId == 0);

//        //    foreach (var scope in template.Scopes)
//        //    {
//        //        var item = bid.Scopes.FirstOrDefault(f => f.ScopeTypeId == scope.ScopeTypeId && f.LineId == scope.LineId);
//        //        if (item == null)
//        //        {
//        //            item = bid.AddScope((DB.ScopeTypeEnum)scope.ScopeTypeId);
//        //            item = Init(bid, (DB.ScopeTypeEnum)scope.ScopeTypeId);
//        //            item.Title = scope.Title;
//        //            item.Notes = scope.Notes;
//        //        }
//        //        else
//        //        {
//        //            item.Title = scope.Title;
//        //            item.Notes = scope.Notes;
//        //        }
//        //    }
//        //}

//    }
//}
