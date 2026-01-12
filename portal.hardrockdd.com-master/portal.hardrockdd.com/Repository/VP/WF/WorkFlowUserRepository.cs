using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Repository.VP.WF
{
    public static class WorkFlowUserRepository
    {
        public static WorkFlowUser Init(WorkFlowSequence workFlow, string userId)
        {
            if (workFlow == null)
                throw new ArgumentNullException(nameof(workFlow));


            var user = new WorkFlowUser
            {
                WFCo = workFlow.WFCo,
                WorkFlowId = workFlow.WorkFlowId,
                SeqId = workFlow.SeqId,
                AssignedTo = userId,
                AssignedOn = workFlow.StatusDate
            };
            return user;
        }

    }
}