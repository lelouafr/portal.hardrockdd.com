using System.ComponentModel.DataAnnotations;
using System.Linq;
using DB.Infrastructure.ViewPointDB.Data;

namespace portal.Models.Views.Web
{
    public class WebUserOverrideViewModel
    {

        public WebUserOverrideViewModel()
        {
            UserId = StaticFunctions.GetUserId();
        }

        [Key]
        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 0, TextSize = 12, ComboUrl = "/WebUsers/Combo", ComboForeignKeys = "")]
        [Display(Name = "Override User")]
        public string UserId { get; set; }
    }

    public class WebCompanyOverrideViewModel
    {

        public WebCompanyOverrideViewModel()
        {
            UserId = StaticFunctions.GetUserId();
            DivisionId = StaticFunctions.GetCurrentDivision().DivisionId;
            CompanyId = StaticFunctions.GetCurrentCompany().HQCo;

            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();
            var usr = StaticFunctions.GetCurrentUser();
            var user = db.WebUsers.FirstOrDefault(f => f.Id == usr.Id);

            var list = user.DivisionLinks.Select(s => s.Division.HQCompany).Distinct().ToList();
            CompanyCnt = list.Count;

        }

        [Key]
        [Required]
        [UIHint("DropdownBoxNav")]
        [Field(LabelSize = 4, TextSize = 8, ComboUrl = "/WebUsers/Combo", ComboForeignKeys = "")]
        [Display(Name = "Override User")]
        public string UserId { get; set; }
    

        [Key]
        [Required]
        [UIHint("DropdownBoxNav")]
        [Field(LabelSize = 4, TextSize = 8, ComboUrl = "/WPCombo/WPUserDivisionCombo", ComboForeignKeys = "")]
        [Display(Name = "Division")]
        public int DivisionId { get; set; }


        [Key]
        [Required]
        [UIHint("DropdownBoxNav")]
        [Field(LabelSize = 4, TextSize = 8, ComboUrl = "/WPCombo/WPUserCompanyCombo", ComboForeignKeys = "")]
        [Display(Name = "Company")]
        public byte CompanyId { get; set; }

        public int CompanyCnt { get; set; }
    }
}