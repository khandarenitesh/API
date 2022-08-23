using CNF.Business.Repositories.UnitOfWork;
using System.Web.Http;

namespace CNF.API.Controllers
{
    [Authorize]
    public class BaseApiController : ApiController
    {
        protected internal IUnitOfWork _unitOfWork;
        public BaseApiController()
        {
            //JobScheduler.Start();
            _unitOfWork = new UnitOfWork();
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
