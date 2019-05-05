using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MQTTnet.Server;
using WebClient.Models;

namespace WebClient.Controllers
{
    [Route("~")]
    public class HomeController : Controller
    {
        public HomeController(LampContext lampContext)
        {
            LampContext = lampContext;
        }

        private LampContext LampContext { get;}
        
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            var data = await LampContext.Lamp.Take(3).ToListAsync();
            return View(data);
        }

    }
}