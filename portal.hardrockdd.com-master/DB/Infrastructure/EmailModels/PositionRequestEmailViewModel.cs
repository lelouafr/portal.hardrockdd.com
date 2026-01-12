using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DB.Infrastructure.EmailModels
{
    public class PositionRequestEmailViewModel
    {
        public PositionRequestEmailViewModel()
        {

        }

        public PositionRequestEmailViewModel(HRPositionRequest request)
        {
            if (request == null)
                return;

            HRCo = request.HRCo;
            RequestId = request.RequestId;
            OpenDate = request.OpenDate;
            PRCo = request.PRCo;

            Comments = request.Comments;
            RequestedOn = request.RequestedDate;
            RequestedBy = request.RequestedBy;
            RequestedByName = request.RequestedUser.FullName();
            Status = request.Status;
            PositionCodeId = request.PositionCodeId;
            ForCrewId = request.ForCrewId;
            PriorEmployeeId = request.tPriorEmployeeId;
            if (request.RequestedUser != null)
                RequestedUser = request.RequestedUser.FullName();

            if (request.HRPosition != null)
                PositionName = request.HRPosition.Description;

            if (request.ForCrew != null)
                CrewName = request.ForCrew.Description;

            if (request.PriorPREmployee != null)
                PriorEmployeeName = request.PriorPREmployee.FullName;

            ApplicantId = request.WAApplicantId;

            NewEmployeeId = request.NewEmployeeId;
        }

        public byte HRCo { get; set; }

        public int RequestId { get; set; }

        public string Comments { get; set; }

        public System.DateTime RequestedOn { get; set; }

        public string RequestedBy { get; set; }


        public string RequestedByName { get; set; }

        public DB.HRPositionRequestStatusEnum Status { get; set; }
                
        public string PositionCodeId { get; set; }

        public string PositionName { get; set; }

        public string RequestedUser { get; set; }

        public string CrewName { get; set; }

        public string PriorEmployeeName { get; set; }

        public bool CreateCrew { get; set; }

        public System.DateTime OpenDate { get; set; }

        public byte? PRCo { get; set; }

        public string ForCrewId { get; set; }

        public int? PriorEmployeeId { get; set; }

        public int? NewEmployeeId { get; set; }

        public int? ApplicantId { get; set; }

    }
}