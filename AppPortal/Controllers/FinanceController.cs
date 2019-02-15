using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppPortal.Models;
using AppPortal.Models.Finance;
using AppPortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppPortal.Controllers
{
    public class FinanceController : Controller
    {
        private readonly FinanceContext _context;

        public FinanceController(FinanceContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ProgramsList()
        {
            if (User.Identity.Name.Remove(0, 5) == "shawvota"
                || User.Identity.Name.Remove(0, 5) == "junewalk"
                || User.Identity.Name.Remove(0, 5) == "angevota")
            {
                var org = await _context.OrgChart.Include(o => o.Division)
                    .Include(o => o.Analyst).OrderBy(o => o.ProgramNum).ToListAsync();

                return View(org);
            }
            else
            {
                return NotFound();
            }

        }

        public async Task<IActionResult> ProgramDetails(int? id)
        {
            if (id == null)
                return NotFound();

            var program = await _context.OrgChart.Include(o => o.Division)
                .Include(o => o.Analyst).SingleOrDefaultAsync(o => o.Id == id);

            if (program == null)
                return NotFound();

            return View(program);
        }

        //Get: Program Create
        public async Task<IActionResult> CreateProgram()
        {
            if (User.Identity.Name.Remove(0, 5) == "shawvota"
                || User.Identity.Name.Remove(0, 5) == "junewalk"
                || User.Identity.Name.Remove(0, 5) == "angevota")
            {
                var divisions = await _context.Division.ToListAsync();
                var analysts = await _context.Analyst.ToListAsync();
                var viewModel = new OrgViewModel
                {
                    Divisions = divisions,
                    Analysts = analysts,
                };
                return View(viewModel);
            }
            else
                return NotFound();
        }
    }
        // POST: Samurais/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]

        //public async Task<IActionResult> Create([Bind("Id,ProgramName")] OrgChart orgChart)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(samurai);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(samurai);
        //}

}