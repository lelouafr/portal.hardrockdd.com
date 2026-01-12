using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.AP.CreditCard.Form
{
    public class TransactionApprovalViewModel : AuditBaseViewModel
    {
        public TransactionApprovalViewModel()
        {

        }

        public TransactionApprovalViewModel(DB.Infrastructure.ViewPointDB.Data.CreditTransaction trans) : base(trans)
        {
            if (trans == null) throw new System.ArgumentNullException(nameof(trans));
            

            CCCo = trans.CCCo;
            TransId = trans.TransId;
            ApprovalStatusId = (DB.CMApprovalStatusEnum)(trans.ApprovalStatusId ?? 0);

            Approved = ApprovalStatusId switch
            {
                DB.CMApprovalStatusEnum.New => false,
                DB.CMApprovalStatusEnum.EmployeeReviewed => false,
                DB.CMApprovalStatusEnum.SupervisorApproved => true,
                _ => false,
            };
        }

        [HiddenInput]
        [Key]
        public byte CCCo { get; set; }

        [HiddenInput]
        [Key]
        public long TransId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Approval")]
        public DB.CMApprovalStatusEnum ApprovalStatusId { get; set; }


        [UIHint("EnumBox")]
        [Display(Name = "Approval")]
        public bool Approved { get; set; }


    }
}