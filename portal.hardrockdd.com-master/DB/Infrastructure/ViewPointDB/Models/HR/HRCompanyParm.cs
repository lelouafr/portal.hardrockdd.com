using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class HRCompanyParm
    {
        private VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = this.HQCompanyParm.db;
                    _db ??= VPContext.GetDbContextFromEntity(this);
                }
                return _db;
            }
        }

        public HRPosition AddPosition(string positionCodeId, string description)
        {
            var position = this.HRPositions.FirstOrDefault(f => f.PositionCodeId == positionCodeId);
            if (position == null)
            {
                position = new HRPosition()
                {
                    HRCo = this.HRCo,
                    PositionCodeId = positionCodeId,
                    Description = description,
                    JobTitle = description,
                    BegSalary = 0,
                    EndSalary = 999999,
                    PartTimeYN = "N",
                    AdYN = "N",
                    BonusYN = "N",
                    ReportPosition = null,
                    Type = "N",
                    OpenJobs = 0,
                    ShowOnWeb = "N",
                    AutoAssignOrg = "N",
                    HRCompany = this,
                    //db = db
                };

                this.HRPositions.Add(position);
                db.SaveChanges();

            }


            return position;
        }

        public HRTermRequest AddTermRequest()
        {
            var request = new HRTermRequest
            {
                HRCo = this.HRCo,
                RequestId = db.GetNextId(HRTermRequest.BaseTableName),
                RequestedDate = DateTime.Now,
                RequestedBy = db.CurrentUserId,

                HRCompanyParm = this,
            };

            this.HRTermRequests.Add(request);

            return request;
        }

        public HRTermRequest AddTermRequest(HRTermRequest model)
        {
            if (model == null)
                return AddTermRequest();
            var hrEmp = db.HRResources.FirstOrDefault(f => f.HRCo == model.HRCo && f.HRRef == model.HRRef);
            var request = new HRTermRequest
            {
                db = db,
                HRCompanyParm = this,
                RequestedUser = db.GetCurrentUser(),
                HRResource = hrEmp,

                HRCo = model.HRCo,
                RequestId = db.GetNextId(HRTermRequest.BaseTableName),
                RequestedDate = model.RequestedDate,
                RequestedBy = model.RequestedBy,
                HRRef = model.HRRef,
                Comments = model.Comments,
                TermDate = model.TermDate,


            };
            request.GenerateWorkFlow();
            request.Status = DB.HRTermRequestStatusEnum.New;
            //request.UpdateStatusChange(request.StatusId)
            this.HRTermRequests.Add(request);

            return request;
        }

        public HRPositionRequest AddPositionRequest()
        {
            var request = new HRPositionRequest
            {
                HRCo = this.HRCo,
                RequestId = db.GetNextId(HRPositionRequest.BaseTableName),
                RequestedDate = DateTime.Now,
                RequestedBy = db.CurrentUserId,
                OpenDate = DateTime.Now.Date,
                HRCompanyParm = this,
            };

            this.HRPositionRequests.Add(request);

            return request;
        }

        public HRPositionRequest AddPositionRequest(HRPositionRequest model)
        {
            if (model == null)
                return AddPositionRequest();
            //var hrEmp = db.HRResources.FirstOrDefault(f => f.HRCo == model.HRCo && f.HRRef == model.HRRef);
            var crew = db.Crews.FirstOrDefault(f => f.PRCo == model.PRCo && f.CrewId == model.ForCrewId);
            var request = new HRPositionRequest
            {
                db = db,
                HRCompanyParm = this,
                RequestedUser = db.GetCurrentUser(),

                HRCo = model.HRCo,
                RequestId = db.GetNextId(HRPositionRequest.BaseTableName),
                RequestedDate = model.RequestedDate,
                OpenDate = model.RequestedDate.Date,
                RequestedBy = model.RequestedBy,
                PositionCodeId = model.PositionCodeId,
                Comments = model.Comments,
                ForCrewId = model.ForCrewId,
                PriorEmployeeId = model.PriorEmployeeId,

            };
            request.GenerateWorkFlow();
            request.Status = DB.HRPositionRequestStatusEnum.New;

            this.HRPositionRequests.Add(request);

            return request;
        }

        internal HRBenefitCode GetHRBenefitCode(PRImportBenefit benefit)
        {
            var codeStr = benefit.PlanName.Substring(benefit.PlanName.Length - 6);
            codeStr = codeStr.Replace("(", string.Empty);
            codeStr = codeStr.Replace(")", string.Empty);

            var code = this.HRBenefitCodes.FirstOrDefault(f => f.BenefitCodeId == codeStr);
            if (code == null)
            {
                code = new HRBenefitCode() 
                { 
                    HRCo = this.HRCo,
                    BenefitCodeId = codeStr,
                    Description = benefit.PlanName,
                    PlanName = benefit.PlanName,
                    PlanNumber = codeStr,

                    HRCompany = this,
                    db = db,
                };

                this.HRBenefitCodes.Add(code);
            }

            return code;

        }

        public HRCompanyAsset AddAsset(string category)
        {
            var list = this.HRCompanyAssets.Where(f => f.AssetCategory == category).ToList();
            
            var number = 1;
            var assetId = string.Format("{0}-{1}", category, number.ToString().PadLeft(5, '0'));
            var asset = list.FirstOrDefault(f => f.AssetId == assetId);
            
            while (asset != null)
            {
                number++;
                assetId = string.Format("{0}-{1}", category, number.ToString().PadLeft(5, '0'));
                asset = list.FirstOrDefault(f => f.AssetId == assetId);
            }


            asset = new HRCompanyAsset() { 
                HRCo = this.HRCo,
                AssetCategory = category,
                AssetId = assetId,
                tStatusId = 0,
                AssetDesc = "New Asset",
            };

            this.HRCompanyAssets.Add(asset);
            db.BulkSaveChanges();
            return asset;
        }

    }
}