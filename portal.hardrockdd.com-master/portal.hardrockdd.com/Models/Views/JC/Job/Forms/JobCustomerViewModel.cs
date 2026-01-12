using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.JC.Job.Forms
{
    public class JobContractViewModel : AuditBaseViewModel
    {
        public JobContractViewModel()
        {

        }

        public JobContractViewModel(DB.Infrastructure.ViewPointDB.Data.JCContract contract) : base(contract)
        {
            if (contract == null)
                return;

            JCCo = contract.JCCo;
            ContractId = contract.ContractId;
           
            CustomerId = contract.CustomerId;
            PayTermId = contract.PayTerms;
            DepartmentId = contract.DepartmentId;
            CustomerRef = contract.CustomerReference;

        }

        [Key]
        [HiddenInput]
        public byte JCCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Job")]
        public string ContractId { get; set; }

        [Display(Name = "Customer")]
        [UIHint("DropDownBox")]
        [Field(ComboUrl = "/ARCombo/CustomerCombo", ComboForeignKeys = "CustGroupId")]
        public int? CustomerId { get; set; }

        [Display(Name = "Department")]
        [UIHint("DropDownBox")]
        [Field(ComboUrl = "/JCCombo/DepartmentCombo", ComboForeignKeys = "JCCo")]
        public string DepartmentId { get; set; }

        [Display(Name = "Pay Terms")]
        [UIHint("DropDownBox")]
        [Field(ComboUrl = "/ARCombo/PayTermsCombo", ComboForeignKeys = "")]
        public string PayTermId { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Customer Ref")]
        public string CustomerRef { get; set; }
    }
}