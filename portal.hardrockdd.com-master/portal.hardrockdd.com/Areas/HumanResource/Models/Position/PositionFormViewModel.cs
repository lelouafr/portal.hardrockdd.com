using DB.Infrastructure.ViewPointDB.Data;
using System.ComponentModel.DataAnnotations;

namespace portal.Areas.HumanResource.Models.Position
{
    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(HRPosition position)
        {
            if (position == null)
                return;

            HRCo = position.HRCo;
            PositionCodeId = position.PositionCodeId;

            Info = new PositionViewModel(position);
            HireTasks = new HireTaskListViewModel(position);
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

        public HireTaskListViewModel HireTasks { get; set; }

        public Resource.ResourceListViewModel ActiveEmployees { get; set; }
    }
}