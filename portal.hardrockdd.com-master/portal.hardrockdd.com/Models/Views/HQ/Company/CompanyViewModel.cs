using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HQ.Company
{
    public class CompanyViewModel
    {

        public CompanyViewModel()
        {

        }

        public CompanyViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null)
                return;


            HQCo = company.HQCo;
            Name = company.Name;
            Address = company.Address;
            City = company.City;
            State = company.State;
            Zip = company.Zip;
            Address2 = company.Address2;
            Phone = company.Phone;
            Fax = company.Fax;

        }


        public byte HQCo { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
    }
}