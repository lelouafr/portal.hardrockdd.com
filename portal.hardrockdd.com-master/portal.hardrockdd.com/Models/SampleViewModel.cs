using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace portal.Models
{
    public class SampleListViewModel
    {
        
        public SampleListViewModel()
        {
            List = new List<SampleViewModel>();
            for (int i = 0; i < 50; i++)
            {
                var myObj = new SampleViewModel(i, string.Format(AppCultureInfo.CInfo(), "{0}, {1}", i, DateTime.Now.AddDays(i)), DateTime.Now.AddDays(i));
                List.Add(myObj);
            }
        }

        public List<SampleViewModel> List { get; }
    }
    public class SampleViewModel
    {
        public SampleViewModel()
        {

        }

        public SampleViewModel(int id, string someString, DateTime someDateTime)
        {
            Id = id;
            SomeString = someString;
            SomeDateTime = someDateTime;
        }

        [Key]
        [UIHint("LongBox")]
        public int Id { get; set; }

        [Required]
        [UIHint("TextBox")]
        public string SomeString { get; set; }

        [Key]
        [Required]
        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Some Date")]
        public DateTime SomeDateTime { get; set; }
    }
}