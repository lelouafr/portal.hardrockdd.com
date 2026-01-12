//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;

//namespace portal.Models.Views.AP.CreditCard
//{
//    public class SummaryListViewModel
//    {

//    }
//    public class SummaryViewModel
//    {

//        [Key]
//        [UIHint("DropdownBox")]
//        [Display(Name = "Supervisor")]
//        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "Co")]
//        public int? SupervisorId { get; set; }

//        [Key]
//        [UIHint("DropdownBox")]
//        [Display(Name = "Employee")]
//        [Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "Co")]
//        public int EmployeeId { get; set; }

//        public decimal TotalPostedAmount { get; set; }

//        public int PictureCount { get; set; }

//        public int CodedCount { get; set; }

//        public int ApprovalCount { get; set; }

//        public int TransCount { get; set; }
//    }
//}