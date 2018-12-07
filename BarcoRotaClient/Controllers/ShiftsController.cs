using BarcoRota.Client.Models;
using BarcoRota.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BarcoRota.Client.Controllers
{
    [Authorize(Roles = Constants.ROTA_ROLE)]
    public class ShiftsController : Controller
    {
        private readonly BarcoContext _context;

        public ShiftsController(BarcoContext context)
        {
            _context = context;
        }

        // GET: Shifts
        public async Task<IActionResult> Index()
        {
            var shitfs = await _context.BarcoShifts
                                .Where(s=>s.BarcoMember.UserName == User.Identity.Name)
                                .Include(s=>s.BarcoMember)
                                .OrderBy(s=>s.BarcoJob.StartDateTime)
                                .Include(s=>s.BarcoJob)
                                .ToListAsync();
            var shiftViewModels = shitfs.Select(s => new BarcoShiftViewModel(s, s.BarcoJob));

            return View(shiftViewModels);
        }

        // GET: Shifts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var barcoShift = await _context.BarcoShifts
                .SingleOrDefaultAsync(m => m.Id == id);
            if (barcoShift == null)
            {
                return NotFound();
            }

            return View(barcoShift);
        }
        
        // GET: Shifts/Start/5
        public async Task<IActionResult> Start(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var barcoShift = await _context.BarcoShifts
                .SingleOrDefaultAsync(m => m.Id == id);
            if (barcoShift == null)
            {
                return NotFound();
            }
            barcoShift.ShiftStatus = ShiftStatus.Started;
            return RedirectToAction(nameof(Index));
        }

        // GET: Shifts/Remind/5
        public async Task<IActionResult> Remind(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var barcoShift = await _context.BarcoShifts
                .SingleOrDefaultAsync(m => m.Id == id);
            if (barcoShift == null)
            {
                return NotFound();
            }

            //TODO: send reminder

            return RedirectToAction(nameof(Index));
        }

        // POST: Shifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var barcoShift = await _context.BarcoShifts.SingleOrDefaultAsync(m => m.Id == id);
            _context.BarcoShifts.Remove(barcoShift);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BarcoShiftExists(int id)
        {
            return _context.BarcoShifts.Any(e => e.Id == id);
        }
    }
}
