using AzureXY.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AzureXY.Controllers
{
    // this is the public side
    [Authorize]
    public class TablesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public TablesController()
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: Tables
        public async Task<ActionResult> Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            return View(await db.Boards.Where(b => b.ApplicationUserID == user.Id).ToListAsync());
        }

        // GET: Tables/Add
        public ActionResult Add()
        {
            return View(new Board());
        }

        // POST: Tables/Add
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add([Bind(Include = "ID,Name")] Board board)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            board.ApplicationUserID = user.Id;
            board.Owner = user;

            if (ModelState.IsValid)
            {
                db.Boards.Add(board);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(board);
        }

        // GET: Tables/Draw/5
        public async Task<ActionResult> Draw(int? id)
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
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (board.ApplicationUserID != user.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            var vm = new BoardViewModel(board);
            vm.Drawings = await db.Drawings.ToListAsync();
            vm.Queue = await db.DrawingQueues.Where(q => q.BoardID == board.ID).ToListAsync();

            return View(vm);
        }

        public async Task<ActionResult> Push(int? tableid, int? drawingid)
        {
            if (tableid == null || drawingid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Board board = await db.Boards.FindAsync(tableid);
            Drawing drawing = await db.Drawings.FindAsync(drawingid);
            if (board == null || drawing == null)
            {
                return HttpNotFound();
            }
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (board.ApplicationUserID != user.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            var queue = new DrawingQueue(board, drawing.ID);
            db.DrawingQueues.Add(queue);
            await db.SaveChangesAsync();

            return RedirectToAction("Draw", new { id = board.ID });
        }

        // GET: Tables/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (board.ApplicationUserID != user.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(board);
        }

        // POST: Tables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name")] Board board)
        {

            // FIXME: user gets board ownership every time...
            var user = UserManager.FindById(User.Identity.GetUserId());
            board.ApplicationUserID = user.Id;
            board.Owner = user;

            if (ModelState.IsValid)
            {
                db.Entry(board).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(board);
        }

        // GET: Tables/Delete/5
        public async Task<ActionResult> Delete(int? id)
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
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (board.ApplicationUserID != user.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(board);
        }

        // POST: Tables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Board board = await db.Boards.FindAsync(id);
            db.Boards.Remove(board);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}