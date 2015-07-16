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

        // GET: Queue/Fetch/5
        public async Task<ActionResult> Fetch(int? id, string token)
        {
            if (id == null || token == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Board board = await db.Boards.FindAsync(id);
            if (board == null)
            {
                return HttpNotFound();
            }
            else if (board.AccessToken != token)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            else
            {
                board.LastConnected = DateTime.Now;
                await db.SaveChangesAsync();
            }
            if (board.Queue == null || board.Queue.Count < 1)
            {
                return View("Nope");
            }
            else
            {
                var queue = board.Queue.OrderBy(q => q.QueueTime).First();
                Drawing drawing = await db.Drawings.FindAsync(queue.DrawingID);
                if (drawing == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    db.DrawingQueues.Remove(queue);
                    await db.SaveChangesAsync();
                }
                return View(drawing);
            }
        }
    }
}