using BarcoRota.Client.Models;
using BarcoRota.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;

namespace BarcoRotaClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly BarcoContext _context;

        public HomeController(BarcoContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string accessToken = "";
            string idToken = "";
            
            if (User.Identity.IsAuthenticated)
            {
                accessToken = await HttpContext.GetTokenAsync("access_token");
                idToken = await HttpContext.GetTokenAsync("id_token");

                if (User.Identity.IsAuthenticated)
                {
                    var member = await _context.GetMemberAsync(User.Identity.Name);
                    if (member == null)
                    {
                        member = new BarcoMember
                        {
                            UserName = User.Identity.Name,
                            Email = User.Claims.Single(c=> c.Type =="email").Value,
                            Name = User.Claims.Single(c => c.Type == "fullName").Value,
                            NickName = User.Claims.Single(c => c.Type == "nickName").Value
                        };

                        if (User.IsInRole("Rota"))
                        {
                            member.RotaStatus = RotaStatus.Active;
                        }

                        _context.BarcoMembers.Add(member);
                        _context.SaveChanges();
                    }
                }
            }

            //get current month
            List<DaySummaryViewModel> dayViewModels = new List<DaySummaryViewModel>();

            DateTime date = DateTime.Today;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            for (DateTime day = firstDayOfMonth; day <= lastDayOfMonth; day = day.AddDays(1))
            {
                var job = _context.BarcoJobs
                    .Where(j => j.StartDateTime.Date == day.Date)
                    .Include(j=>j.Shifts)
                        .ThenInclude(s=>s.BarcoMember)
                    .FirstOrDefault();
                dayViewModels.Add(new DaySummaryViewModel(day, job));
            }

            return View(dayViewModels);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminArea()
        {
            return View();
        }

        [ActionName("signout-oidc")]
        public IActionResult Signout()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
