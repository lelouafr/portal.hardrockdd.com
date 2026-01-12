using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PO
{
    public static class RequestRepository 
    {
        //private VPContext db = new VPContext();

        public static PORequest Init(HQCompanyParm company)
        {
            var user = company.db.GetCurrentUser();
            var model = new PORequest
            {
                POCo = (byte)company.POCo,
                RequestId = company.POCompanyParm.PORequests.DefaultIfEmpty().Max(f => f == null ? 0 : f.RequestId) + 1,                
                CreatedBy = user.Id,
                CreatedOn = DateTime.Now,
                CreatedUser = user,
                OrderedByCo = user.PREmployee.PRCo,
                OrderedBy = user.PREmployee.EmployeeId,
                OrderUser = user.PREmployee,
                OrderedDate = DateTime.Now.Date,
                VendorGroup = company.VendorGroupId,
                POCompanyParm = company.POCompanyParm,

            };

            model.Status = DB.PORequestStatusEnum.Open;
            return model;
        }
        public static Models.Views.Purchase.Request.PORequestViewModel ProcessUpdate(Models.Views.Purchase.Request.PORequestViewModel model, ModelStateDictionary modelState)
        {
            using var db = new VPContext();
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = db.PORequests.FirstOrDefault(f => f.POCo == model.POCo && f.POCo == model.POCo && f.RequestId == model.RequestId);
            if (updObj != null)
            {
                var vendChange = updObj.VendorId != model.VendorId;
                if (updObj.VendorId != 999999)
                {
                    model.NewVendorAddress = null;
                    model.NewVendorName = null;
                    model.NewVendorPhone = null;
                }

                /****Write the changes to object****/
                //UpdateJobInfo(updObj, model, db);
                updObj.VendorGroup = model.VendorGroupId ?? updObj.POCompanyParm.HQCompanyParm.VendorGroupId;
                updObj.VendorId = model.VendorId;
                if (model.Description != null)
                    updObj.Description = model.Description.Substring(0, model.Description.Length > 60 ? 59 : model.Description.Length);
                else
                    updObj.Description = model.Description;
                if (model.OrderedDate >= new DateTime(1900, 1, 1))
                {
                    updObj.OrderedDate = model.OrderedDate;
                }
                updObj.NewVendorAddress = model.NewVendorAddress;
                updObj.NewVendorName = model.NewVendorName;
                updObj.NewVendorPhone = model.NewVendorPhone;

                db.SaveChanges(modelState);
            }
            return new Models.Views.Purchase.Request.PORequestViewModel(updObj);
        }

    }
}