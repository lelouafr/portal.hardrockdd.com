using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HR.Position
{
    public class PositionFormViewModel
    {
        public PositionFormViewModel()
        {

        }

        public PositionFormViewModel(Code.Data.VP.HRPosition position)
        {
            if (position == null)
                return;

            HRCo = position.HRCo;
            PositionCodeId = position.PositionCodeId;

            Info = new PositionViewModel(position);
            HireTasks = new PositionHireTaskListViewModel(position);
            ActiveEmployees = new Resource.ResourceListViewModel(position);

        }

        [Key]
        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Code")]
        public string PositionCodeId { get; set; }

        public PositionViewModel Info { get; set; }

        public PositionHireTaskListViewModel HireTasks { get; set; }

        public Resource.ResourceListViewModel ActiveEmployees { get; set; }
    }
}