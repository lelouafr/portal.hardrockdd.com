using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Crew
{
	public class CrewListViewModel
	{
        public CrewListViewModel()
        {
		}


		public CrewListViewModel(List<DB.Infrastructure.ViewPointDB.Data.Crew> tickets)
		{
			List = tickets.Select(s => new CrewViewModel(s)).ToList();
		}

		public CrewListViewModel(VPContext db)
		{
			if (db == null)
			{
				List = new List<CrewViewModel>();

				return;
			}


			List = db.Crews
				.ToList()
				.Select(s => new CrewViewModel(s))
				.ToList();
		}

		public List<CrewViewModel> List { get; }
	}

	public class CrewViewModel
	{
        public CrewViewModel()
        {
            
        }

        public CrewViewModel(DB.Infrastructure.ViewPointDB.Data.Crew crew)
        {
			if (crew == null)
				return;

			PRCo = crew.PRCo;
			CrewId = crew.CrewId;
			Status = crew.Status;
			ActiveYN = crew.Status ? "Yes" : "No";
			Description = crew.Description;
			CrewType = crew.udCrewType;
			CrewLeaderId = crew.CrewLeaderId;
			JobId = crew.JobId;

			CrewLeaderName = crew.CrewLeader?.FullName(false);
			JobName = crew.DefaultJob?.DisplayName;

		}


        [Key]
		[Required]
		[HiddenInput]
		[Display(Name = "Company #")]
		public int PRCo { get; set; }

		[Key]
		[Required]
		[UIHint("TextBox")]
		[Display(Name = "Crew #")]
		public string CrewId { get; set; }

		[UIHint("SwitchBox")]
		[Display(Name = "Status")]
		public bool Status { get; set; }



		[UIHint("TextBox")]
		[Display(Name = "Status")]
		public string ActiveYN { get; set; }

		[UIHint("TextBox")]
		[Field(LabelSize = 2, TextSize = 10)]
		[Display(Name = "Description")]
		public string Description { get; set; }

		[UIHint("TextBox")]
		[Display(Name = "Crew Type")]
		public string CrewType { get; set; }

		[UIHint("DropdownBox")]
		[Display(Name = "Crew Leader")]
		[Field(LabelSize = 2, TextSize = 10, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
		public int? CrewLeaderId { get; set; }



		[UIHint("TextBox")]
		[Display(Name = "Crew Leader")]
		public string CrewLeaderName { get; set; }

		public byte? JCCo { get; set; }

		[UIHint("DropdownBox")]
		[Field(LabelSize = 2, TextSize = 10,  ComboUrl = "/JCCombo/ActiveCombo", ComboForeignKeys = "JCCo")]
		[Display(Name = "Last Job")]
		public string JobId { get; set; }

		[UIHint("TextBox")]
		[Display(Name = "Job Name")]
		public string JobName { get; set; }

		internal CrewViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
		{
			if (db == null) throw new ArgumentNullException(nameof(db));
			var updObj = db.Crews.FirstOrDefault(f => f.PRCo == PRCo && f.CrewId == CrewId);

			if (updObj != null)
			{
				/****Write the changes to object****/
				updObj.Status = Status;
				updObj.Description = Description;
				updObj.udCrewType = CrewType;
				updObj.CrewLeaderId = CrewLeaderId;
				updObj.JobId = JobId;

				try
				{
					db.SaveChanges(modelState);
					return new CrewViewModel(updObj);
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