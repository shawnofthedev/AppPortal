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


        //**************************************************************************
        //Get Lists
        //**************************************************************************
        public async Task<IActionResult> ProgramsList()
        {
            List<OrgChart> org = await _context.OrgChart.Include(o => o.Division)
                .Include(o => o.Analyst).OrderBy(o => o.ProgramNum).ToListAsync();

            return View(org);
        }
        public async Task<IActionResult> DivisionList()
        {
            List<Division> div = await _context.Division.Include(d => d.DivLead).ToListAsync();
            return View(div);
        }
        public async Task<IActionResult> DivLeadList()
        {
            List<DivLead> divLead = await _context.DivLead.ToListAsync();
            return View(divLead);
        }
        public async Task<IActionResult> AnalystList()
        {
            List<Analyst> analysts = await _context.Analyst.ToListAsync();
            return View(analysts);
        }


        //**************************************************************************
        //Edit tables
        //**************************************************************************
        public async Task<IActionResult> EditProgram(int? id)
        {
            bool isAuthorized = Authorize(User.Identity.Name.Remove(0, 5));
            if (isAuthorized == true)
            {
                if (id == null)
                    return NotFound();

                OrgChart program = await _context.OrgChart.SingleOrDefaultAsync(m => m.Id == id);

                if (program == null)
                    return NotFound();

                OrgViewModel viewModel = new OrgViewModel
                {
                    OrgChart = program,
                    Divisions = await _context.Division.ToListAsync(),
                    Analysts = await _context.Analyst.ToListAsync(),
                };

                return View(viewModel);
            }
            else
                TempData["ErrorMessage"] = "Unauthorized Access";
                return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> EditDivision(int? id)
        {
            bool isAuthorized = Authorize(User.Identity.Name.Remove(0, 5));
            if (isAuthorized == true)
            {
                if (id == null)
                    return NotFound();

                Division division = await _context.Division.SingleOrDefaultAsync(d => d.Id == id);

                if (division == null)
                    return NotFound();

                OrgViewModel viewModel = new OrgViewModel
                {
                    Division = division,
                    Leads = await _context.DivLead.ToListAsync(),
                };

                return View(viewModel);
            }
            else
                TempData["ErrorMessage"] = "Unauthorized Access";
                return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> EditLead(int? id)
        {
            bool isAuthorized = Authorize(User.Identity.Name.Remove(0, 5));
            if (isAuthorized == true)
            {
                if (id == null)
                    return NotFound();

                DivLead lead = await _context.DivLead.SingleOrDefaultAsync(l => l.Id == id);

                if (lead == null)
                    return NotFound();

                return View(lead);
            }
            else
                TempData["ErrorMessage"] = "Unauthorized Access";
                return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> EditAnalyst(int? id)
        {
            bool isAuthorized = Authorize(User.Identity.Name.Remove(0, 5));
            if (isAuthorized == true)
            {
                if (id == null)
                    return NotFound();
                Analyst analyst = await _context.Analyst.SingleOrDefaultAsync(a => a.Id == id);

                if (analyst == null)
                    return NotFound();

                return View(analyst);
            }
            else
                TempData["ErrorMessage"] = "Unauthorized Access";
                return RedirectToAction(nameof(Index));
        }

        //**************************************************************************
        //Get Details
        //**************************************************************************
        public async Task<IActionResult> ProgramDetails(int? id)
        {
            if (id == null)
                return NotFound();

            OrgChart program = await _context.OrgChart.Include(o => o.Division)
                .Include(o => o.Analyst).SingleOrDefaultAsync(o => o.Id == id);

            if (program == null)
                return NotFound();

            return View(program);
        }

        //Get: Program Create
        public async Task<IActionResult> CreateProgram()
        {
            bool isAuthorized = Authorize(User.Identity.Name.Remove(0, 5));
            if (isAuthorized == true)
            {
                List<Division> divisions = await _context.Division.ToListAsync();
                List<Analyst> analysts = await _context.Analyst.ToListAsync();
                OrgViewModel viewModel = new OrgViewModel
                {
                    Divisions = divisions,
                    Analysts = analysts,
                };
                return View(viewModel);
            }
            else
                TempData["ErrorMessage"] = "Unauthorized Access";
                return RedirectToAction(nameof(Index));
        }


        //add names of the users to authorize here
        public static bool Authorize(string user)
        {
            if (user == "shawvota"
                || user == "junewalk"
                || user == "chermcco"
                || user == "leigledf"
                || user == "angevota")
            {
                var isAuthorized = true;
                return isAuthorized;
            }
            else
            {
                var isAuthorized = false;
                return isAuthorized;
            }
        }
    }
}