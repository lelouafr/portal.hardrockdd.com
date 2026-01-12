//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web.Mvc;

//namespace portal.Repository.VP.BD
//{
//    public static class BidPackageScopeRepository 
//    {
//        public static BidPackageScope Init(BidPackage package, DB.ScopeTypeEnum scopeTypeId)
//        {
//            if (package == null)
//            {
//                throw new ArgumentNullException(nameof(package));
//            }
//            var model = new BidPackageScope
//            {
//                BDCo = package.BDCo,
//                BidId = package.BidId,
//                PackageId = package.PackageId,
//                ScopeTypeId = (byte)scopeTypeId,
//                Notes = string.Empty,
//                Title = string.Empty

//                //Package = package
//            };

//            return model;
//        }
        
//        public static Models.Views.Bid.Forms.Proposal.Package.ProposalScopeViewModel ProcessUpdate(Models.Views.Bid.Forms.Proposal.Package.ProposalScopeViewModel model, VPContext db)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model));
//            }
//            var updObj = db.BidPackageScopes.FirstOrDefault(f => f.BDCo == model.BDCo && 
//                                                                 f.BidId == model.BidId && 
//                                                                 f.PackageId == model.PackageId && 
//                                                                 f.ScopeTypeId == (int)model.ScopeTypeId && 
//                                                                 f.LineId == model.LineId);
//            if (updObj != null)
//            {
//                /****Write the changes to object****/
//                updObj.Title = model.Title ?? string.Empty;
//                updObj.Notes = model.Notes ?? string.Empty;
//            }
//            return new Models.Views.Bid.Forms.Proposal.Package.ProposalScopeViewModel(updObj);
//        }
       
//        //public void CopyFromTemplate(byte Co, int BidId, int PackageId)
//        //{
//        //    var package = db.BidPackages.FirstOrDefault(f => f.BDCo == Co && f.BidId == BidId && f.PackageId == PackageId);
//        //    var tPackage = db.BidPackages.FirstOrDefault(f => f.BDCo == Co && f.BidId == 0 && f.PackageId == 0);

//        //    foreach (var scope in tPackage.Scopes)
//        //    {
//        //        var item = tPackage.Scopes.Where(f => f.BDCo == Co && f.BidId == BidId && f.PackageId == PackageId && f.ScopeTypeId == scope.ScopeTypeId && f.LineId == scope.LineId).FirstOrDefault();
//        //        if (item == null)
//        //        {
//        //            item = Init(package, (DB.ScopeTypeEnum)scope.ScopeTypeId);
//        //            item.Title = scope.Title;
//        //            item.Notes = scope.Notes;
//        //            package.Scopes.Add(item);
//        //        }
//        //    }
//        //    db.SaveChanges();
//        //}
//    }
//}
