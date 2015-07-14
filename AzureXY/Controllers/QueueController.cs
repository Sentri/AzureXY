using AzureXY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AzureXY.Controllers
{
    public class QueueController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Queue
        public ActionResult Index()
        {
            return View();
        }

        // GET: Queue/Fetch/5
        public async Task<ActionResult> Fetch(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Board board = await db.Boards.FindAsync(id);
            if (board == null)
            {
                return HttpNotFound();
            }
            return View();
        }
    }
}