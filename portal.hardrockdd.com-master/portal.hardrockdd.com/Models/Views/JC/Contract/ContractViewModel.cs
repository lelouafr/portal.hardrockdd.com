using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.JC.Contract
{
    public class ContractViewModel
    {
        public ContractViewModel()
        {

        }

        public ContractViewModel(DB.Infrastructure.ViewPointDB.Data.JCContract contract)
        {
            if (contract == null)
                return;

            JCCo = contract.JCCo;
            ContractId = contract.ContractId;
            Description = contract.Description;
            CustGroupId = contract.CustGroupId;
            CustomerId = contract.CustomerId;
            ARNotes = contract.ExtContract?.ARNotes;
        }

        [Key]
        public byte JCCo { get; set; }

        [Key]
        [Field(LabelSize = 2, TextSize = 3)]
        [Display(Name = "Contract")]
        public string ContractId { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 0, TextSize = 7)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [HiddenInput]
        [Display(Name = "Cust Group")]
        public byte? CustGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/ARCombo/CustomerCombo", ComboForeignKeys = "CustGroupId")]
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "AR Notes")]
        public string ARNotes { get; set; }

        internal ContractViewModel ProcessUpdate(ModelStateDictionary modelState, VPContext db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            var updObj = db.JCContracts.FirstOrDefault(f => f.JCCo == this.JCCo && f.ContractId == this.ContractId);

            if (updObj != null)
            {
                /****Write the changes to object****/
                if (updObj.ExtContract == null)
                    updObj.ExtContract = updObj.AddExtContract();

                updObj.ExtContract.ARNotes = this.ARNotes;
                try
                {
                    db.SaveChanges(modelState);
                    return new ContractViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            modelState.AddModelError("", "Object Doesn't Exist For Update!");
            return this;
        }
    }
}