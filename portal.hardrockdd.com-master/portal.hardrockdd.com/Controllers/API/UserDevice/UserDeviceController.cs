//using NetSuiteDB.Infrastructure.Data;
//using portal.Models.Admin.Security;
//using portal.Models.GL.TrialBalance;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Runtime.Caching;
//using System.Web.Http;

//namespace portal.Controllers.API.Register
//{
//    [APIAuthorizeDevice]
//    public class UserDeviceController : ApiController
//    {
//        [HttpPost]
//        public ActRegistrationViewModel Post(ActRegistrationViewModel register)
//        {
//            using var db = new PortalDBEntities();
//            var reg = db.SysExcelDevices.FirstOrDefault(f => f.DeviceId == register.DeviceId);
//            ActRegistrationViewModel result;
//            if (reg == null)
//            {
//                result = new ActRegistrationViewModel()
//                {
//                    DeviceId = register.DeviceId,
//                    IsDisabled = true,
//                };
//            }
//            else 
//            {
//                if (reg.DeviceName != register.DeviceName)
//                {
//                    reg.DeviceName = register.DeviceName;
//                    db.SaveChanges();
//                }
//                result = new ActRegistrationViewModel(reg);
//            }

//            return result;
//        }
//    }
//}
