//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;

//namespace portal.Models.Views.Service.Request
//{
//    public class SeriviceRequestListViewModel
//    {
//        public SeriviceRequestListViewModel()
//        {

//        }

//        public SeriviceRequestListViewModel(WebUser user)
//        {
//            List = user.WorkFlows.Where(f => f.WorkFlow.TableName == "budSMSH" && f.Sequence.Active)
//                                 .SelectMany(s => s.WorkFlow.ServiceRequests)
//                                 .Select(s => new ServiceRequestViewModel(s))
//                                 .ToList();


//        }

//        public List<ServiceRequestViewModel> List { get; set; }
//    }
//    public class ServiceRequestViewModel
//    {
//        public ServiceRequestViewModel()
//        {

//        }

//        public ServiceRequestViewModel(ServiceRequest request)
//        {
            
//        }

//        [Key]
//        public byte Co { get; set; }

//        [Key]
//        public int ServiceTicketId { get; set; }

//        public int? ServiceTicketTypeId { get; set; }

//        public string CreatedBy { get; set; }

//        public DateTime? CreatedOn { get; set; }

//        public Guid? UniqueAttchID { get; set; }

//        public string Description { get; set; }


//    }
//}