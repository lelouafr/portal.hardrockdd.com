using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.CreditCard.Employee
{
    public class TransactionListViewModel
    {
        public TransactionListViewModel()
        {

        }

        public TransactionListViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, DateTime mth )
        {
            if (employee == null) throw new System.ArgumentNullException(nameof(employee));

            PRCo = employee.PRCo;
            SupervisorId = employee.ReportsToId ?? 0;
            EmployeeId = employee.EmployeeId;
            Mth = mth;

            List = employee.CreditCardTransactions.Where(w => w.Mth == mth).Select(s => new TransactionViewModel(s)).ToList();
            Summary = new TransactionSummaryViewModel(List);
        }

        [Key]

        public byte PRCo { get; set; }

        [Key]
        [UIHint("DropdownBox")]
        [Display(Name = "Supervisor")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
        public int? SupervisorId { get; set; }

        [Key]
        [UIHint("DropdownBox")]
        [Display(Name = "Employee")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        public int EmployeeId { get; set; }

        [Key]
        public DateTime Mth { get; set; }

        public List<TransactionViewModel> List { get; }

        public TransactionSummaryViewModel Summary { get; set; }
    }

    public class TransactionSummaryViewModel
    {
        public TransactionSummaryViewModel()
        {

        }

        public TransactionSummaryViewModel(List<TransactionViewModel> list)
        {
            TotalPostedAmount = list.Sum(sum => sum.TransAmt);
            PictureCount = list.Where(f => f.PictureStatusId == DB.CMPictureStatusEnum.Attached || f.PictureStatusId == DB.CMPictureStatusEnum.NotNeeded).Count();
            CodedCount = list.Where(f => f.CodeStatusId == DB.CMTransCodeStatusEnum.AutoCoded ||
                                         f.CodeStatusId == DB.CMTransCodeStatusEnum.Coded ||
                                         f.CodeStatusId == DB.CMTransCodeStatusEnum.Complete ||
                                         f.CodeStatusId == DB.CMTransCodeStatusEnum.EmployeeCoded).Count();
            ApprovalCount = list.Where(f => f.ApprovalStatusId == DB.CMApprovalStatusEnum.SupervisorApproved).Count();
        }

        //[Key]
        //public byte PRCo { get; set; }
        //[Key]
        //[UIHint("DropdownBox")]
        //[Display(Name = "Supervisor")]
        //[Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "CCCo")]
        //public int? SupervisorId { get; set; }

        //[Key]
        //[UIHint("DropdownBox")]
        //[Display(Name = "Employee")]
        //[Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        //public int EmployeeId { get; set; }

        public decimal TotalPostedAmount { get; set; }

        public int PictureCount { get; set; }

        public int CodedCount { get; set; }

        public int ApprovalCount { get; set; }
    }

    public class TransactionViewModel
    {
        public TransactionViewModel()
        {

        }

        public TransactionViewModel(CreditTransaction transaction)
        {
            if (transaction == null) throw new System.ArgumentNullException(nameof(transaction));
            var transLine = transaction.Lines.FirstOrDefault();
            CCCo = transaction.CCCo;
            TransId = transaction.TransId;
            Source = transaction.Source;
            PRCo = transaction.PRCo ?? transaction.Employee.PRCo;
            EmployeeId = (int)transaction.EmployeeId;
            EmployeeName = transaction.Employee.FullName;
            SupervisorId = transaction.Employee.ReportsToId;
            if (transaction.Employee?.Supervisor != null)
            {
                SupervisorName = transaction.Employee.Supervisor.FullName;
            }
            VendorGroupId = transaction.Merchant.VendGroupId;
            MerchantId = transaction.MerchantId;
            Merchant = transaction.Merchant.Name;
            TransDate = transaction.TransDate;
            TransAmt = transaction.TransAmt;
            Description = transLine?.NewDescription ?? transLine?.OrigDescription;

            CodeStatusId = (DB.CMTransCodeStatusEnum)(transaction.CodedStatusId ?? 0);
            PictureStatusId = (DB.CMPictureStatusEnum)(transaction.PictureStatusId ?? 0);
            ApprovalStatusId = (DB.CMApprovalStatusEnum)(transaction.ApprovalStatusId ?? 0);
            

            if ((CodeStatusId == DB.CMTransCodeStatusEnum.Coded || 
                CodeStatusId == DB.CMTransCodeStatusEnum.AutoCoded ||
                CodeStatusId == DB.CMTransCodeStatusEnum.Complete) && transaction.Status == DB.CMTranStatusEnum.RequestedInfomation)
            {
                transaction.Status = DB.CMTranStatusEnum.Open;
                transaction.db.BulkSaveChanges();
            }
            TransactionStatusId = transaction.Status;
            Approved = ApprovalStatusId switch
            {
                DB.CMApprovalStatusEnum.New => false,
                DB.CMApprovalStatusEnum.EmployeeReviewed => false,
                DB.CMApprovalStatusEnum.SupervisorApproved => true,                
                _ => false,
            };
        }

        [HiddenInput]
        public byte? GLCo { get; set; }
        [HiddenInput]
        public byte? PRCo { get; set; }
        [HiddenInput]
        public byte? EMCo { get; set; }
        [HiddenInput]
        public byte? JCCo { get; set; }
        [HiddenInput]
        public byte? EMGroupId { get; set; }
        [HiddenInput]
        public byte? PhaseGroupId { get; set; }
        [HiddenInput]
        public byte? VendorGroupId { get; set; }


        [Key]
        public byte CCCo { get; set; }

        [Key]
        public long TransId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Source")]
        public string Source { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Supervisor")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
        public int? SupervisorId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Supervisor")]
        public string SupervisorName { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Employee")]
        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        public int EmployeeId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Merchant")]
        [Field(ComboUrl = "/APCombo/MerchantCombo", ComboForeignKeys = "CCCo")]
        public string MerchantId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Merchant")]
        public string Merchant { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Trans Date")]
        public DateTime TransDate { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        public decimal TransAmt { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.CMTranStatusEnum TransactionStatusId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Coded")]
        public DB.CMTransCodeStatusEnum CodeStatusId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Picture")]
        public DB.CMPictureStatusEnum PictureStatusId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Approval")]
        public DB.CMApprovalStatusEnum ApprovalStatusId { get; set; }

        [UIHint("SwitchBox")]
        [Display(Name = "Approval")]
        public bool Approved { get; set; }

    }
}