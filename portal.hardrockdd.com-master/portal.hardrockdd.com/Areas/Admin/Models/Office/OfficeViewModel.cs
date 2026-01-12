using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Areas.Admin.Models.Office
{
    public class OfficeListViewModel
    {
        public OfficeListViewModel()
        {

        }

        public OfficeListViewModel(List<HQOffice> list)
        {
            List = list.Select(s => new OfficeViewModel(s)).ToList();
        }

        public List<OfficeViewModel> List { get; }
    }

    public class OfficeViewModel
    {
        public OfficeViewModel()
        {

        }

        public OfficeViewModel(HQOffice office)
        {
            if (office == null)
                return;
            Description = office.Description;
            OfficeId = office.OfficeId;
            Address = office.Address;
            Address2 = office.Address2;
            City = office.City;
            State = office.State;
            Zip = office.Zip;

            Phone = office.Phone;
        }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int OfficeId { get; set; }


        [UIHint("TextBox")]
        [Display(Name = "Description")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string Description { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Address")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string Address { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Add'l Address")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string Address2 { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "City")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string City { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "State")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string State { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Zip")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string Zip { get; set; }

        [UIHint("PhoneBox")]
        [Display(Name = "Phone")]
        [Field(LabelSize = 4, TextSize = 8)]
        public string Phone { get; set; }


        internal OfficeViewModel ProcessUpdate(VPContext db, System.Web.Mvc.ModelStateDictionary modelState)
        {
            var updObj = db.HQOffices.FirstOrDefault(f => f.OfficeId == OfficeId);

            if (updObj != null)
            {
                updObj.Description = Description;
                updObj.Address = Address;
                updObj.Address2 = Address2;
                updObj.City = City;
                updObj.State = State;
                updObj.Zip = Zip;
                updObj.Phone = Phone;

                try
                {
                    db.BulkSaveChanges();
                    return new OfficeViewModel(updObj);
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