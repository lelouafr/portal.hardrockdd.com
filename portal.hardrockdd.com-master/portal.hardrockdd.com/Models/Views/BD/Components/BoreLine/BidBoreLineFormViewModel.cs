//using Microsoft.AspNet.Identity;
//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.Bid.BoreLine;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BidBoreLineFormViewModel
//    {
//        public BidBoreLineFormViewModel()
//        {

//        }

//        public BidBoreLineFormViewModel(DB.Infrastructure.ViewPointDB.Data.BidBoreLine line)
//        {
//            if (line == null)
//            {
//                return;
//            }

//            #region mapping
//            Co = line.Co;
//            BidId = line.BidId;
//            BoreId = line.BoreId;
//            #endregion

//            Bid = new BidViewModel(line.Bid);
//            Line = new BoreLineViewModel(line);
//            CostItems = new CostItemListViewModel(line);
//            Production = new PassListViewModel(line);
//            Attachments = new Attachment.AttachmentListViewModel(line.Bid.Co, "udBDBH", line.Bid.KeyID, line.Bid.UniqueAttchID);
            
//            Forum = new Forums.ForumLineListViewModel(line.Bid.Forum);
//            var userId = StaticFunctions.GetUserId();
//            var flowAssignment = line.Bid.WorkFlows.Where(f => f.Active == "Y" && f.AssignedTo == userId).FirstOrDefault();
            
//        }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Co")]
//        public byte Co { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Bid Id")]
//        public int BidId { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Bore Id")]
//        public int BoreId { get; set; }

//        public BidViewModel Bid { get; set; }

//        public BoreLineViewModel Line { get; set; }

//        public CostItemListViewModel CostItems { get; set; }

//        public PassListViewModel Production { get; set; }

//        public BidWorkFlowSubmitViewModel FlowSubmit { get; set; }

//        public Attachment.AttachmentListViewModel Attachments { get; set; }

//        public Forums.ForumLineListViewModel Forum { get; set; }
//    }

//}