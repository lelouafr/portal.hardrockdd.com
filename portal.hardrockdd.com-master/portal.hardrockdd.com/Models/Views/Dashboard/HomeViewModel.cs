using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web;

namespace portal.Models.Views.Dashboard
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            using var db =  new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var userId = StaticFunctions.GetUserId();
            var results = db.udWPHM_Index(userId).FirstOrDefault();
            ApprovalCount = results.DailyApprovalCnt ?? 0;
            RejectedCount = results.DailyRejectCnt ?? 0;
            BidSalesCount = results.BidSalesCount ?? 0;
            UnAssignedEquipmentCnt = results.UnAssignedEquipmentCnt ?? 0;
            BidReviewCount = results.BidReviewCount ?? 0;
            POApprovalCount = results.POApprovalCount ?? 0;
            PORejectCount = results.PORejectCount ?? 0;
            LeaveReviewCount = results.LeaveReviewCount ?? 0;
            LeaveOpenCount = results.LeaveOpenCount ?? 0;
            DirectReportCnt = results.DirectReportCnt ?? 0;
            InvoiceCount = results.InvoiceCount ?? 0;
            InvoiceTotal = results.InvoiceAMT ?? 0;
            InvoicePendingTotal = results.InvoicePendingAMT ?? 0;
            InvoicePendingCount = results.InvoicePendingCount ?? 0;
            EMAuditApprovalCount = results.EMAuditApprovalCount ?? 0;
            EMAuditProcessCount = results.EMAuditProcessCount ?? 0;
            EMAuditCount = results.EMAuditCount ?? 0;
            YourInvoicePendingCount = results.YourInvoicePendingCount ?? 0;
            YourInvoicePendingTotal = results.YourInvoicePendingTotal ?? 0;

            CMMth = (DateTime)results.CMMth;
            CMEmployeeMissingPictures = results.CMEmployeeMissingPictures ?? 0;
            CMEmployeeMissingPictureAmount = results.CMEmployeeMissingPictureAmount ?? 0;

            CMPriorMth = (DateTime)results.CMPriorMth;
            CMEmployeePriorAmount = results.CMEmployeePriorAmount ?? 0;
            CMEmployeePriorMissingPictures = results.CMEmployeePriorMissingPictures ?? 0;
            CMEmployeePriorMissingPictureAmount = results.CMEmployeePriorMissingPictureAmount ?? 0;

            CMEmployeeAmount = results.CMEmployeeAmount ?? 0;
            CMEmployeeNeedingCoding = results.CMEmployeeNeedingCoding ?? 0;
            CMSupervisorNeedingApproval = results.CMSupervisorNeedingApproval ?? 0;
            CMSupervisorPriorNeedingApproval = results.CMSupervisorPriorNeedingApproval ?? 0;

            CMAdminNeedingCoding = results.CMAdminNeedingCoding ?? 0;
            CMAdminNeedingAmount = results.CMAdminNeedingAmount ?? 0;

            EMServiceRequestLineCount = results.EMServiceRequestLineCount ?? 0;

            HRTermRequestCount = results.HRTermRequestCount ?? 0;
            HRPositionRequestCount = results.HRPositionRequestCount ?? 0;
            HRPositionRequestAvailableApplicansCount = results.HRPositionRequestAvailableApplicansCount ?? 0;
            HRSubmitedApplicationCount = results.HRSubmitedApplicationCount ?? 0;

            PMLocateRequestCount = results.PMLocateRequestCount ?? 0;
            PMLocateExpiringCount = results.PMLocateExpiringCount ?? 0;

        }
        public long HRTermRequestCount { get; set; }
        public long HRPositionRequestCount { get; set; }
        public long HRPositionRequestAvailableApplicansCount { get; set; }
        public long HRSubmitedApplicationCount { get; set; }

        public long ApprovalCount { get; set; }

        public long InvoiceCount { get; set; }

        public long InvoicePendingCount { get; set; }

        public double InvoiceTotal { get; set; }

        public double InvoicePendingTotal { get; set; }

        public double YourInvoicePendingTotal { get; set; }

        public long YourInvoicePendingCount { get; set; }

        public long POApprovalCount { get; set; }
        public long PORejectCount { get; set; }

        

        public long RejectedCount { get; set; }

        public long BidReviewCount { get; set; }

        public long LeaveReviewCount { get; set; }

        public long LeaveOpenCount { get; set; }

        public long BidSalesCount { get; set; }

        public long DirectReportCnt { get; set; }

        public long UnAssignedEquipmentCnt { get; set; }

        public long EMAuditCount { get; set; }

        public long EMAuditApprovalCount { get; set; }

        public long EMAuditProcessCount { get; set; }

        public DateTime CMMth { get; set; }

        public long CMEmployeeMissingPictures { get; set; }

        public double CMEmployeeMissingPictureAmount { get; set; }

        public DateTime CMPriorMth { get; set; }
        public long CMEmployeePriorMissingPictures { get; set; }

        public double CMEmployeePriorMissingPictureAmount { get; set; }

        public long CMEmployeeNeedingCoding { get; set; }

        public double CMEmployeeAmount { get; set; }
        
        public double CMEmployeePriorAmount { get; set; }

        public long CMSupervisorNeedingApproval { get; set; }

        public long CMSupervisorPriorNeedingApproval { get; set; }

        public long CMAdminNeedingCoding { get; set; }

        public long CMAdminNeedingAmount { get; set; }

        public long EMServiceRequestLineCount { get; set; }

        public long PMLocateRequestCount { get; set; }

        public long PMLocateExpiringCount { get; set; }

    }
}