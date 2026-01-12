using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HR.Position
{
    public class PositionHireTaskListViewModel
    {
        public PositionHireTaskListViewModel()
        {

        }

        public PositionHireTaskListViewModel(Code.Data.VP.HRPosition position)
        {
            if (position == null)
                return;


            HRCo = position.HRCo;
            PositionCodeId = position.PositionCodeId;

            List = position.DefaultTasks.Select(s => new PositionHireTaskViewModel(s)).ToList();
        }

        [Key]
        [Required]
        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        [Key]
        [Required]
        [UIHint("TextBox")]
        public string PositionCodeId { get; set; }

        public List<PositionHireTaskViewModel> List { get; }
    }

    public class PositionHireTaskViewModel
    {
        public PositionHireTaskViewModel()
        {

        }

        public PositionHireTaskViewModel(Code.Data.VP.HRPositionTask task)
        {
            if (task == null)
                return;


            HRCo = task.HRCo;
            PositionCodeId = task.PositionCodeId;
            SeqId = task.SeqId;
            TaskId = task.TaskId;
            IsRequired = task.IsRequired ?? true;

            

        }

        [Key]
        [Required]
        [UIHint("LongBox")]
        public byte HRCo { get; set; }

        [Key]
        [Required]
        [UIHint("TextBox")]
        public string PositionCodeId { get; set; }

        [Key]
        [Required]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int SeqId { get; set; }

        [Required]
        [UIHint("DropDownBox")]
        [Display(Name = "Task")]
        [Field(ComboUrl = "/HRCombo/HireTaskCombo")]
        public int? TaskId { get; set; }

        [Required]
        [UIHint("SwitchBox")]
        [Display(Name = "Required")]
        public bool IsRequired { get; set; }

        internal PositionHireTaskViewModel ProcessUpdate(Code.Data.VP.VPEntities db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            var updObj = db.HRPositionTasks.FirstOrDefault(f => f.HRCo == this.HRCo && f.PositionCodeId == this.PositionCodeId && f.SeqId == this.SeqId);

            if (updObj != null)
            {
                updObj.TaskId = this.TaskId;
                updObj.IsRequired = this.IsRequired;

                try
                {
                    db.BulkSaveChanges();
                    return new PositionHireTaskViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}