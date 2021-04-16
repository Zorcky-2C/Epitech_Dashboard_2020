using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Infrastructure
{
    [Route("[controller]/[action]", Name = "[controller]_[action]")]
    public abstract class BaseController : Controller
    {
    }
}
