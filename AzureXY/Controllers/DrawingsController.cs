using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AzureXY.Models;

namespace AzureXY.Controllers
{
    public class DrawingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Drawings
        public async Task<ActionResult> Index()
        {
            return View(await db.Drawings.ToListAsync());
        }

        // GET: Drawings/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Drawing drawing = await db.Drawings.FindAsync(id);
            if (drawing == null)
            {
                return HttpNotFound();
            }
            return View(drawing);
        }

        // GET: Drawings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Drawings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Instructions,Name,Created")] Drawing drawing)
        {
            if (ModelState.IsValid)
            {
                db.Drawings.Add(drawing);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(drawing);
        }

        // GET: Drawings/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Drawing drawing = await db.Drawings.FindAsync(id);
            if (drawing == null)
            {
                return HttpNotFound();
            }
            return View(drawing);
        }

        // POST: Drawings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Instructions,Name,Created")] Drawing drawing)
        {
            if (ModelState.IsValid)
            {
                db.Entry(drawing).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(drawing);
        }

        // GET: Drawings/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Drawing drawing = await db.Drawings.FindAsync(id);
            if (drawing == null)
            {
                return HttpNotFound();
            }
            return View(drawing);
        }

        // POST: Drawings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Drawing drawing = await db.Drawings.FindAsync(id);
            db.Drawings.Remove(drawing);
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
