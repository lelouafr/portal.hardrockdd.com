using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portal.Areas.HumanResource.Models.Position
{
    public class CreateViewModel
    {
        public CreateViewModel()
        {

        }
        public CreateViewModel(VPContext db)
        {
            db = db ?? throw new ArgumentNullException(nameof(db));

            HRCo = (byte)db.GetCurrentCompany(StaticFunctions.GetCurrentCompany().HQCo).HRCo;
            

        }

        [Key]
        [Required]
        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        [Key]
        [Required]
        [UIHint("TextBox")]
        public string PositionCodeId { get; set; }

        [Required]
        [UIHint("TextBox")]
        public string Description { get; set; }

        internal PositionViewModel Create(VPContext db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                var hr = db.HRCompanyParms.FirstOrDefault(f => f.HRCo == this.HRCo);
                var pos = hr.AddPosition(this.PositionCodeId, this.Description);

                return new PositionViewModel(pos);
            }

            return new PositionViewModel();
        }
    }
}