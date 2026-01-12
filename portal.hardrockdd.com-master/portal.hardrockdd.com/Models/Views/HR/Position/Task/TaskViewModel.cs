using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HR.Task
{
    public class TaskListViewModel
    {
        public TaskListViewModel()
        {

        }

        public TaskListViewModel(Code.Data.VP.VPEntities db)
        {
            if (db == null)
                return;


            List = db.HRTasks.Select(s => new TaskViewModel(s)).ToList();
        }



        public List<TaskViewModel> List { get; }
    }

    public class TaskViewModel
    {
        public TaskViewModel()
        {

        }

        public TaskViewModel(Code.Data.VP.HRTask task)
        {
            if (task == null)
                return;


            TaskId = task.TaskId;
            TaskTypeId = task.TaskTypeId;
            Description = task.Description;
            AssignedTo = task.DefaultAssignedTo;
        }

        [Key]
        [Required]
        [UIHint("LongBox")]
        [Display(Name = "Task")]
        public int TaskId { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        public byte? TaskTypeId { get; set; }

        [Required]
        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [UIHint("DropDownBox")]
        [Display(Name = "Assigned To")]
        [Field(ComboUrl = "/WPCombo/WPUserCombo")]
        public string AssignedTo { get; set; }


        internal TaskViewModel ProcessUpdate(Code.Data.VP.VPEntities db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            var updObj = db.HRTasks.FirstOrDefault(f => f.TaskId == this.TaskId);

            if (updObj != null)
            {
                updObj.Description = this.Description;
                updObj.DefaultAssignedTo = this.AssignedTo;
                updObj.TaskTypeId = this.TaskTypeId;

                try
                {
                    db.BulkSaveChanges();
                    return new TaskViewModel(updObj);
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