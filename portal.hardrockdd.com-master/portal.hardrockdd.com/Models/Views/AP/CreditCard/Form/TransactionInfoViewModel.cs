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
    public class InfoViewModel : AuditBaseViewModel
    {
        public InfoViewModel()
        {

        }

        public InfoViewModel(DB.Infrastructure.ViewPointDB.Data.CreditTransaction trans) : base(trans)
        {
            if (trans == null) throw new System.ArgumentNullException(nameof(trans));
            

            CCCo = trans.CCCo;
            TransId = trans.TransId;
            PRCo = trans.PRCo ?? trans.Employee.PRCo;
            EmployeeId = (int)trans.EmployeeId;
            MerchantId = trans.MerchantId;
            TransDate = trans.TransDate;
            TransAmt = trans.TransAmt;
            Description = trans.NewDescription ?? trans.OrigDescription;
            APRef = trans.APRef;// ?? trans.UniqueTransId.Remove(0, 8);

            OriginalAPRef = trans.UniqueTransId.Remove(0, 8);
            Mth = trans.Mth;

            IsAPRefRequired = trans.Merchant.IsAPRefRequired ?? false;

            CodeStatusId = (DB.CMTransCodeStatusEnum)(trans.CodedStatusId ?? 0);
            PictureStatusId = (DB.CMPictureStatusEnum)(trans.PictureStatusId ?? 0);
            ApprovalStatusId = (DB.CMApprovalStatusEnum)(trans.ApprovalStatusId ?? 0);

            TranStatusId = (DB.CMTranStatusEnum)(trans.TransStatusId ?? 0);
            //HasAttachment = StaticFunctions.HasAttachments(job.UniqueAttchID);

            //Category = job.Category?.Description;
            //Status = StaticFunctions.GetComboValues("EMEquipStatus").FirstOrDefault(f => f.DatabaseValue == job.Status)?.DisplayValue;
            //Type = StaticFunctions.GetComboValues("EMEquipType").FirstOrDefault(f => f.DatabaseValue == job.Status)?.DisplayValue;

            Actions = new ActionViewModel(trans);
        }


        [HiddenInput]
        public byte? PRCo { get; set; }


        [HiddenInput]
        [Key]
        public byte CCCo { get; set; }

        [HiddenInput]
        [Key]
        public long TransId { get; set; }

        [HiddenInput]
        public DateTime Mth { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Employee")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
        public int EmployeeId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Merchant")]
        [Field(ComboUrl = "/APCombo/MerchantCombo", ComboForeignKeys = "CCCo")]
        public string MerchantId { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Trans Date")]
        public DateTime TransDate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal TransAmt { get; set; }


        [UIHint("TextAreaBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 2, TextSize = 10)]
        public string Description { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "APRef")]
        [Field(LabelSize = 2, TextSize = 4)]
        public string APRef { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Orig APRef")]
        [Field(LabelSize = 2, TextSize = 4)]
        public string OriginalAPRef { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Coded")]
        public DB.CMTransCodeStatusEnum CodeStatusId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Picture")]
        public DB.CMPictureStatusEnum PictureStatusId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Approval")]
        public DB.CMApprovalStatusEnum ApprovalStatusId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.CMTranStatusEnum TranStatusId { get; set; }

        public bool IsAPRefRequired { get; set; }

        public ActionViewModel Actions { get; set; }
    }
}