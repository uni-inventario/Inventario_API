using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected long? UserId
        {
            get
            {
                var sub = User.FindFirstValue("id");
                
                if (!long.TryParse(sub, out var id))
                    return null;

                return id;
            }
        }
    }
}
