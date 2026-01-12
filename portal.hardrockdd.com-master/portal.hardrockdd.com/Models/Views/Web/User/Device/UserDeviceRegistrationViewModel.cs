using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DB.Infrastructure.ViewPointDB.Data;
namespace portal.Models.Views.Web.User.Device
{
    public class UserDeviceRegistrationViewModel
    {
        public UserDeviceRegistrationViewModel()
        {

        }

        public UserDeviceRegistrationViewModel(WPUserDeviceRegistry registry)
        {
            DeviceId = registry.DeviceId;
            DeviceName = registry.DeviceName;
            UserId = registry.UserId;
            IsDisabled = !(!registry.IsDisabled && registry.User.Active); //&& registry.User.
            IsRegistered = registry != null;

        }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }

        public string UserId { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsRegistered { get; set; }
    }
}