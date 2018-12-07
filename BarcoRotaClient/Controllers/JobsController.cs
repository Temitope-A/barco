using BarcoRota.Client.Models;
using BarcoRota.Models;
using BarcoRota.Models.Factories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BarcoRota.Client.Controllers
{
    [Authorize(Roles = Constants.ROTA_ROLE)]
    public class JobsController : Controller
    {
        private readonly BarcoContext _context;

        public JobsController(BarcoContext context)
        {
            _context = context;
        }

        // GET: Jobs
        public async Task<IActionResult> Index()
        {
            var jobs = await _context.BarcoJobs
                .Include(j => j.Shifts)
                    .ThenInclude(s => s.BarcoMember)
                .OrderBy(j => j.Created)
                .Take(30)
                .ToListAsync();
            var jobViewModels = jobs.Select(j => new BarcoJobViewModel(j));
            return View(jobViewModels);
        }

        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var barcoJob = await _context.BarcoJobs
                .Include(j => j.Shifts)
                    .ThenInclude(s => s.BarcoMember)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (barcoJob == null)
            {
                return NotFound();
            }

            return View(new BarcoJobViewModel(barcoJob));
        }

        // GET: Jobs/Create
        [Authorize(Roles = Constants.BARCO_ROLE)]
        public IActionResult Create()
        {
            var workPackages = _context.WorkPackages.ToList();
            ViewData["workPackages"] = workPackages;
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.BARCO_ROLE)]
        public async Task<IActionResult> Create([Bind("Id,WorkPackageId,Created,StartDateTime,EndDateTime,JobType,JobCapacity")] BarcoJob barcoJob)
        {
            if (ModelState.IsValid)
            {
                _context.Add(barcoJob);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(barcoJob);
        }

        // GET: Jobs/Delete/5
        [Authorize(Roles = Constants.BARCO_ROLE)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var barcoJob = await _context.BarcoJobs
                .Include(j => j.Shifts)
                    .ThenInclude(s => s.BarcoMember)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (barcoJob == null)
            {
                return NotFound();
            }

            return View(new BarcoJobViewModel(barcoJob));
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.BARCO_ROLE)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var barcoJob = await _context.BarcoJobs.SingleOrDefaultAsync(m => m.Id == id);
            _context.BarcoJobs.Remove(barcoJob);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Jobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var barcoJob = await _context.BarcoJobs.SingleOrDefaultAsync(m => m.Id == id);
            if (barcoJob == null)
            {
                return NotFound();
            }
            return View(barcoJob);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Created,StartDateTime,EndDateTime,JobType,JobCapacity")] BarcoJob barcoJob)
        {
            if (id != barcoJob.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(barcoJob);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BarcoJobExists(barcoJob.Id))
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
            return View(barcoJob);
        }

        // GET: Jobs/TakeShift/5
        public async Task<IActionResult> TakeShift(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.GetMemberAsync(User.Identity.Name);
            var job = await _context.BarcoJobs.Include(j => j.Shifts).SingleOrDefaultAsync(j => j.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            var shift = ShiftFactory.AddShift(job, member);

            if (shift != null)
            {
                //_context.Add(shift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Shifts");
            }
            else
            {
                return RedirectToAction(nameof(Details), new { @id = id });
            }
        }

        private bool BarcoJobExists(int id)
        {
            return _context.BarcoJobs.Any(e => e.Id == id);
        }
    }
}
