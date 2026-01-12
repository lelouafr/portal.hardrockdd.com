namespace portal.Areas.HumanResource.Models.PositionRequest
{
    public class RequestEmailViewModel: RequestViewModel
    {
        public RequestEmailViewModel()
        {

        }

        public RequestEmailViewModel(DB.Infrastructure.ViewPointDB.Data.HRPositionRequest request): base(request)
        {
            
        }

    }
}