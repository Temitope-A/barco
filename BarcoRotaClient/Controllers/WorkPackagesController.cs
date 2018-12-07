using BarcoRota.Client.Models;
using BarcoRota.Models;
using BarcoRota.Models.Factories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BarcoRota.Client.Controllers
{
    [Authorize(Roles = Constants.BARCO_ROLE)]
    public class WorkPackagesController : Controller
    {
        private readonly BarcoContext _context;

        public WorkPackagesController(BarcoContext context)
        {
            _context = context;
        }

        // GET: WorkPackages
        public async Task<IActionResult> Index()
        {
            var data = await _context.WorkPackages
                .Include(w=>w.Jobs)
                    .ThenInclude(j=>j.Shifts)
                .Include(w=>w.Manager)
                .ToListAsync();
            var workPackages = data.Select(w => new WorkPackageViewModel(w));
            return View(workPackages);
        }

        // GET: WorkPackages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workPackage = await _context.WorkPackages
                .Include(w => w.Jobs)
                    .ThenInclude(j => j.Shifts)
                .Include(w => w.Manager)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (workPackage == null)
            {
                return NotFound();
            }

            return View(new WorkPackageViewModel(workPackage));
        }

        // GET: WorkPackages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkPackages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,FirstJob,LastJob,JobType,JobCapacity")] WorkPackageCreationViewModel wp)
        {
            var manager = await _context.GetMemberAsync(User.Identity.Name);
            var workPackage = WorkPackageFactory.CreateWorkPackage(wp.Name, wp.FirstJob, wp.LastJob, manager, wp.JobType, wp.JobCapacity);

            if (ModelState.IsValid)
            {
                _context.Add(workPackage);

                foreach (var job in workPackage.Jobs)
                {
                    _context.Add(job);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workPackage);
        }

        // GET: WorkPackages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workPackage = await _context.WorkPackages.SingleOrDefaultAsync(m => m.Id == id);
            if (workPackage == null)
            {
                return NotFound();
            }
            return View(workPackage);
        }

        // GET: WorkPackages/Fill/5
        public async Task<IActionResult> Fill(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workPackage = await _context.WorkPackages
                .Include(w=>w.Jobs)
                    .ThenInclude(j => j.Shifts)
                .SingleOrDefaultAsync(w=> w.Id == id);

            if (workPackage == null)
            {
                return NotFound();
            }

            var today = DateTime.Now.Date;
            var members = _context.BarcoMembers.Where(m=>m.RotaStatus == RotaStatus.Active).ToList();

            var result = WorkPackageFactory.FillWorkPackage(workPackage, members, false);
            while (result.UnfilledJobs > 0)
            {
                result = WorkPackageFactory.FillWorkPackage(workPackage, members, true);
            }
            workPackage.Status = WorkPackageStatus.Filled;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: WorkPackages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] WorkPackage workPackage)
        {
            if (id != workPackage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workPackage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkPackageExists(workPackage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workPackage);
        }

        // GET: WorkPackages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workPackage = await _context.WorkPackages
                .Include(w => w.Jobs)
                    .ThenInclude(j => j.Shifts)
                .Include(w => w.Manager)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (workPackage == null)
            {
                return NotFound();
            }

            return View(new WorkPackageViewModel(workPackage));
        }

        // POST: WorkPackages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workPackage = await _context.WorkPackages.SingleOrDefaultAsync(m => m.Id == id);
           // _context.BarcoJobs.RemoveRange(workPackage.Jobs);
            _context.WorkPackages.Remove(workPackage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkPackageExists(int id)
        {
            return _context.WorkPackages.Any(e => e.Id == id);
        }
    }
}
