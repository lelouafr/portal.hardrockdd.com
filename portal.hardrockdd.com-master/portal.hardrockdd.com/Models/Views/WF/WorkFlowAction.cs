using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Models.Views.WF
{
    public class WorkFlowAction
    {
        public string TableName { get; set; }

        public int ActionId { get; set; }

        public string Title { get; set; }

        public string ButtonClass { get; set; }

        public string ActionUrl { get; set; }

        public int GotoStatusId { get; set; }

        public string ActionRedirect { get; set; }

        public bool IsAjax { get; set; }
    }
}