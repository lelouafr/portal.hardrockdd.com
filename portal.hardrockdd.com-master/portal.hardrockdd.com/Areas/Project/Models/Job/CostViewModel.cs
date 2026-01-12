using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DB.Infrastructure.ViewPointDB.Data;


namespace portal.Areas.Project.Models.Job
{
    public class CostListViewModel
    {

        public CostListViewModel()
        {

        }

        public CostListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, bool inlcudeUnPosted = true)
        {
            if (job == null)
                return;
            
            List = job.ActualJobCosts().Select(s => new CostViewModel(s)).ToList();

            JCCo = job.JCCo;
            JobId = job.JobId;

            var jobCosts = job.ActualJobCosts();
            List = jobCosts.Select(s => new CostViewModel(s)).ToList();
            if (inlcudeUnPosted)
            {
                var jobUnPostCosts = job.UnPostedJobCosts();
                List.AddRange(jobUnPostCosts.Select(s => new CostViewModel(s, true)).ToList());
            }

        }

        public CostListViewModel(DB.Infrastructure.ViewPointDB.Data.Job job, DateTime mth, bool inlcudeUnPosted = true)
        {

            if (job == null || mth == null)
                return;
            JCCo = job.JCCo;
            JobId = job.JobId;

            var jobCosts = job.ActualJobCosts(mth);
            List = jobCosts.Select(s => new CostViewModel(s)).ToList();

            if (inlcudeUnPosted)
            {
                var jobUnPostCosts = job.UnPostedJobCosts(mth);
                List.AddRange(jobUnPostCosts.Select(s => new CostViewModel(s, true)).ToList());
            }

        }

        [Key]
        public byte JCCo { get; set; }

        [Key]
        public string JobId { get; set; }

        public List<CostViewModel> List { get; }
    }

    public class CostViewModel
    {
        public CostViewModel()
        {

        }

        public CostViewModel(DB.Infrastructure.ViewPointDB.Data.JobCost cost, bool UnPosted = false)
        {
            if (cost == null)
            {
                return;
            }

            JCCo = cost.JCCo;
            Mth = cost.Mth;
            CostTrans = cost.CostTrans;
            JobId = cost.JobId;
            PhaseGroupId = cost.PhaseGroupId;
            PhaseId = cost.PhaseId;
            PhaseDescription = cost?.JobPhase?.Description;
            CostTypeID = cost.CostTypeID;
            CostTypeDesc = cost.CostType?.Description ?? "Unknown";
            Posted = !UnPosted;
            PostedDate = cost.PostedDate;
            ActualDate = cost.ActualDate;
            JCTransType = cost.JCTransType;
            Source = cost.Source;
            Description = cost.Description ?? "";
            ActualHours = cost.ActualHours;
            ActualUnits = cost.ActualUnits;
            ActualCost = cost.ActualCost;
            PRCo = cost.PRCo;
            Employee = cost.Employee;
            VendorGroup = cost.VendorGroup;
            Vendor = cost.Vendor;
            APCo = cost.APCo;
            APTrans = cost.APTrans;
            APLine = cost.APLine;
            APRef = cost.APRef;
            PO = cost.PO;
            POItem = cost.POItem;
            EMCo = cost.EMCo;
            EMEquip = cost.EMEquip;

            DisplayDescription = JCTransType switch
            {
                "PR" => string.Format(AppCultureInfo.CInfo(), "{0}", cost.JobPhase?.Description),
                "PR2" => string.Format(AppCultureInfo.CInfo(), "{0}: {1}", cost.PREmployee?.EmployeeId, cost.PREmployee.FullName(false)),
                "EM" => string.Format(AppCultureInfo.CInfo(), "{0}: {1}", cost.Equipment?.EquipmentId, cost.Equipment?.Description),
                "AP" => cost.Source.Trim() switch { 
                            "AP Entry"=> string.Format(AppCultureInfo.CInfo(), "{0}: {1}", cost.APTransLine?.APTran?.Vendor?.VendorId, cost.APTransLine?.APTran?.Vendor?.Name),
                            _ => string.Format(AppCultureInfo.CInfo(), "{0}", cost.Description),
                        },                
                _ => Description,
            };
            if (JCTransType == "PR" && string.IsNullOrEmpty(Description))
            {
                Description = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", cost.PREmployee?.EmployeeId, cost.PREmployee.FullName(false));// (cost.EarnFactor ?? 0) == 1.5m ? "Overtime" : "Regular";
            }
            switch (JCTransType)
            {
                case "AP":
                    if (cost.APTransLine?.CCTransId != null)
                    {
                        Description = string.Format(AppCultureInfo.CInfo(), "{0}: {1}", cost.APTransLine.APTran.CCTransaction.Employee.FullName(), Description);
                    }
                    break;
                case "EM":
                    if (cost.Source.Trim() == "EMRev" && cost.ActualCost != 0)
                    {
                        Description = string.Format(AppCultureInfo.CInfo(), "{0}: {1} {2} x ${3}", Description, cost.PostedUnits, cost.UM, cost.ActualCost/cost.PostedUnits);
                    }
                    break;
                default:
                    break;
            }
        }

        [Key]
        public byte JCCo { get; set; }
        [Key]
        public System.DateTime Mth { get; set; }
        [Key]
        public int CostTrans { get; set; }

        public bool Posted { get; set; }

        public string JobId { get; set; }

        public byte PhaseGroupId { get; set; }
        public string PhaseId { get; set; }
        public string PhaseDescription { get; set; }
        public byte CostTypeID { get; set; }
        public string CostTypeDesc { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime ActualDate { get; set; }
        public string JCTransType { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public decimal ActualHours { get; set; }
        public decimal ActualUnits { get; set; }
        public decimal ActualCost { get; set; }
        public byte? PRCo { get; set; }
        public int? Employee { get; set; }
        public byte? VendorGroup { get; set; }
        public int? Vendor { get; set; }
        public byte? APCo { get; set; }
        public int? APTrans { get; set; }
        public short? APLine { get; set; }
        public string APRef { get; set; }
        public string PO { get; set; }
        public short? POItem { get; set; }
        public byte? EMCo { get; set; }
        public string EMEquip { get; set; }


        public string DisplayDescription { get; set; }
    }
}