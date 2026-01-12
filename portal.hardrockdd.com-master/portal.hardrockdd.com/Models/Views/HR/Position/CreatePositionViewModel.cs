using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HR.Position
{
    public class CreatePositionViewModel
    {
        public CreatePositionViewModel()
        {

        }
        public CreatePositionViewModel(Code.Data.VP.VPEntities db)
        {
            db = db ?? throw new ArgumentNullException(nameof(db));

            HRCo = (byte)db.GetCurrentCompany().HRCo;
            

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

        internal PositionViewModel Create(Code.Data.VP.VPEntities db, System.Web.Mvc.ModelStateDictionary modelState)
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