using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Web.User.Device;
using System.Linq;
using System.Web.Http;

namespace portal.Controllers.API.Register
{
    [APIAuthorizeDevice]
    public class RegisterController : ApiController
    {
        [HttpPost]
        public UserDeviceRegistrationViewModel Post(UserDeviceRegistrationViewModel register)
        {
            using var db = new VPContext();
            var reg = db.WPUserDeviceRegistries.FirstOrDefault(f => f.DeviceId == register.DeviceId);
            UserDeviceRegistrationViewModel result;
            if (reg == null)
            {
                result = new UserDeviceRegistrationViewModel()
                {
                    DeviceId = register.DeviceId,
                    IsDisabled = true,
                };
            }
            else 
            {
                if (reg.DeviceName != register.DeviceName)
                {
                    reg.DeviceName = register.DeviceName;
                    db.SaveChanges();
                }
                result = new UserDeviceRegistrationViewModel(reg);
            }

            return result;
        }
    }
}
