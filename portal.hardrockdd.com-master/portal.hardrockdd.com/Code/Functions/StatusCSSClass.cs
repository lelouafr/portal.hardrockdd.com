using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal
{
     public static partial class StaticFunctions
    {
        public static string StatusClass(DB.PORequestStatusEnum status)
        {
            return status switch
            {
                DB.PORequestStatusEnum.Open => "label-warning",
                DB.PORequestStatusEnum.Submitted => "label-success",
                DB.PORequestStatusEnum.Approved => "label-primary",
                DB.PORequestStatusEnum.Processed => "label-primary",
                DB.PORequestStatusEnum.Rejected => "label-danger",
                DB.PORequestStatusEnum.Canceled => "label-danger",
                _ => "label-danger",
            };
        }

        public static string StatusClass(DB.LeaveRequestStatusEnum status)
        {
            return status switch
            {
                DB.LeaveRequestStatusEnum.Open => "label-warning",
                DB.LeaveRequestStatusEnum.Submitted => "label-success",
                DB.LeaveRequestStatusEnum.Approved => "label-primary",
                DB.LeaveRequestStatusEnum.Processed => "label-primary",
                DB.LeaveRequestStatusEnum.Rejected => "label-danger",
                DB.LeaveRequestStatusEnum.Canceled => "label-danger",
                _ => "label-danger",
            };
        }

        public static string StatusClass(DB.POStatusEnum status)
        {
            return status switch
            {
                DB.POStatusEnum.Open => "label-success",
                DB.POStatusEnum.Complete => "label-success",
                DB.POStatusEnum.Closed => "label-primary",
                DB.POStatusEnum.Pending => "label-warning",
                _ => "label-danger",
            };
        }

        public static string StatusClass(DB.DailyTicketStatusEnum status)
        {
            return status switch
            {
                DB.DailyTicketStatusEnum.Draft => "label-warning",
                DB.DailyTicketStatusEnum.Submitted => "label-success",
                DB.DailyTicketStatusEnum.Approved => "label-primary",
                DB.DailyTicketStatusEnum.Processed => "label-primary",
                DB.DailyTicketStatusEnum.Rejected => "label-danger",
                DB.DailyTicketStatusEnum.Canceled => "label-danger",
                DB.DailyTicketStatusEnum.Deleted => "label-danger",
                _ => "label-danger",
            };
        }

        public static string StatusClass(DB.BidStatusEnum status)
        {
            return status switch
            {
                DB.BidStatusEnum.Draft => "label-warning",
                DB.BidStatusEnum.Estimate => "label-success",
                DB.BidStatusEnum.SalesReview => "label-info",
                DB.BidStatusEnum.FinalReview => "label-info",
                DB.BidStatusEnum.Proposal => "label-info",
                DB.BidStatusEnum.PendingAward => "label-info",
                DB.BidStatusEnum.ContractReview => "label-info",
                DB.BidStatusEnum.ContractApproval => "label-info",
                DB.BidStatusEnum.Awarded => "label-primary",
                DB.BidStatusEnum.NotAwarded => "label-danger",
                DB.BidStatusEnum.Canceled => "label-danger",
                DB.BidStatusEnum.Deleted => "label-danger",
                DB.BidStatusEnum.Template => "label-danger",
                _ => "label-danger",
            };
        }

    }
}