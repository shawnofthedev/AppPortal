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
        //GET Edit tables
        //**************************************************************************
        public async Task<IActionResult> ProgramEdit(int? id)
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
        public async Task<IActionResult> DivisionEdit(int? id)
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
        public async Task<IActionResult> DivLeadEdit(int? id)
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
        public async Task<IActionResult> AnalystEdit(int? id)
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
        //Post Edit
        //**************************************************************************
        [HttpPost]
        [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Edit(int id, [bind("id,name")] Samurai samurai)
        public async Task<IActionResult> EditProgram(OrgViewModel org)
        {
            OrgChart objProgram = org.OrgChart;

            if (ModelState.IsValid)
            {
                OrgChart orgInDb = await _context.OrgChart.SingleAsync(o => o.Id == objProgram.Id);

                orgInDb.FundFunctProg = objProgram.FundFunctProg;
                orgInDb.ProgramNum = objProgram.ProgramNum;
                orgInDb.ProgramName = objProgram.ProgramName;
                orgInDb.DivisionId = objProgram.DivisionId;
                orgInDb.AnalystId = objProgram.AnalystId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProgramsList));
            }
            return RedirectToAction(nameof(ProgramEdit), new { objProgram.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDivision(OrgViewModel org)
        {
            Division objDivision = org.Division;

            if (ModelState.IsValid)
            {
                Division divInDb = await _context.Division.SingleAsync(d => d.Id == objDivision.Id);

                divInDb.DivisionName = objDivision.DivisionName;
                divInDb.DivLeadId = objDivision.DivLeadId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DivisionList));
            }
            return RedirectToAction(nameof(ProgramEdit), new { objDivision.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLead(DivLead lead)
        {
            if (ModelState.IsValid)
            {
                DivLead leadInDb  = await _context.DivLead.SingleAsync(l => l.Id == lead.Id);

                leadInDb.DivLeadName = lead.DivLeadName;
                leadInDb.DivLeadEmail = lead.DivLeadEmail;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DivLeadList));
            }
            return RedirectToAction(nameof(DivLeadEdit), new { lead.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnalyst(Analyst analyst)
        {
            if (ModelState.IsValid)
            {
                Analyst analystInDb  = await _context.Analyst.SingleAsync(a => a.Id == analyst.Id);

                analystInDb.AnalystName = analyst.AnalystName;
                analystInDb.AnalystEmail = analyst.AnalystEmail;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AnalystList));
            }
            return RedirectToAction(nameof(AnalystEdit), new { analyst.Id });
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

        public async Task<IActionResult> DivisionDetails(int? id)
        {
            if (id == null)
                return NotFound();

            Division division = await _context.Division.Include(d => d.DivLead).SingleOrDefaultAsync(d => d.Id == id);

            if (division == null)
                return NotFound();

            return View(division);
        }

        public async Task<IActionResult> DivLeadDetails(int? id)
        {
            if (id == null)
                return NotFound();

            DivLead lead = await _context.DivLead.SingleOrDefaultAsync(l => l.Id == id);

            if (lead == null)
                return NotFound();

            return View(lead);
        }

        public async Task<IActionResult> AnalystDetails(int? id)
        {
            if (id == null)
                return NotFound();

            Analyst analyst = await _context.Analyst.SingleOrDefaultAsync(a => a.Id == id);

            if (analyst == null)
                return NotFound();

            return View(analyst);
        }


        //**************************************************************************
        //GET Create New
        //**************************************************************************
        public async Task<IActionResult> ProgramCreate()
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
        public async Task<IActionResult> DivisionCreate()
        {
            bool isAuthorized = Authorize(User.Identity.Name.Remove(0, 5));
            if (isAuthorized == true)
            {
                List<DivLead> leads = await _context.DivLead.ToListAsync();
                OrgViewModel viewModel = new OrgViewModel
                {
                    Leads = leads,
                };
                return View(viewModel);
            }
            else
                TempData["ErrorMessage"] = "Unauthorized Access";
                return RedirectToAction(nameof(Index));

        }
        public IActionResult DivLeadCreate()
        {
            bool isAuthorized = Authorize(User.Identity.Name.Remove(0, 5));
            if (isAuthorized == true)
            {
                DivLead lead = new DivLead();
                return View(lead);
            }
            else
                TempData["ErrorMessage"] = "Unauthorized Access";
                return RedirectToAction(nameof(Index));

        }
        public IActionResult AnalystCreate()
        {
            bool isAuthorized = Authorize(User.Identity.Name.Remove(0, 5));
            if (isAuthorized == true)
            {
                Analyst analyst = new Analyst();
                return View(analyst);
            }
            else
                TempData["ErrorMessage"] = "Unauthorized Access";
                return RedirectToAction(nameof(Index));
        }
        //**************************************************************************
        //Post New
        //**************************************************************************
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProgram(OrgViewModel org)
        {
            OrgChart objProgram = org.OrgChart;
            if (ModelState.IsValid)
            {
                await _context.OrgChart.AddAsync(objProgram);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProgramsList));
            }
            return View(org);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDivision(OrgViewModel org)
        {
            Division objDivision = org.OrgChart.Division;
            if (ModelState.IsValid)
            {
                await _context.Division.AddAsync(objDivision);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DivisionList));
            }
            return View(org);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDivLead(DivLead lead)
        {
            if (ModelState.IsValid)
            {
                await _context.DivLead.AddAsync(lead);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DivLeadList));
            }
            return View(lead);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAnalyst(Analyst analyst)
        {
            if (ModelState.IsValid)
            {
                await _context.Analyst.AddAsync(analyst);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AnalystList));
            }
            return View(analyst);
        }
        //**************************************************************************
        //GET Delete Record
        //**************************************************************************
        public async Task<IActionResult> ProgramDelete(int? id)
        {
            bool isAuthorized = Authorize(User.Identity.Name.Remove(0, 5));
            if (isAuthorized == true)
            {
                if (id == null)
                    return NotFound();

                OrgChart program = await _context.OrgChart.Include(o => o.Division)
                    .Include(o => o.Analyst).SingleOrDefaultAsync(o => o.Id == id);

                if (program == null)
                    return NotFound();

                return View(program);
            }
            else
            {
                TempData["ErrorMessage"] = "Unauthorized Access";
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> DivisionDelete(int? id)
        {
            bool isAuthorized = Authorize(User.Identity.Name.Remove(0, 5));
            if (isAuthorized == true)
            {
                if (id == null)
                    return NotFound();

                Division division = await _context.Division.Include(d => d.DivLead).SingleOrDefaultAsync(d => d.Id == id);

                if (division == null)
                    return NotFound();

                return View(division);
            }
            else
            {
                TempData["ErrorMessage"] = "Unauthorized Access";
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> DivLeadDelete(int? id)
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
            {
                TempData["ErrorMessage"] = "Unauthorized Access";
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> AnalystDelete(int? id)
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
            {
                TempData["ErrorMessage"] = "Unauthorized Access";
                return RedirectToAction(nameof(Index));
            }
        }
        //**************************************************************************
        //POST Delete Record
        //**************************************************************************
        [HttpPost, ActionName("DeleteProgram")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            OrgChart orgChart = await _context.OrgChart.SingleOrDefaultAsync(m => m.Id == id);
            _context.OrgChart.Remove(orgChart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ProgramsList));
        }
        [HttpPost, ActionName("DeleteDivision")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDivision(int id)
        {
            List<OrgChart> org = await _context.OrgChart.Where(o => o.DivisionId == id).ToListAsync();
            if (org.Count > 0)
            {
                TempData["ErrorMessage"] = "The Division you have chosen is assigned to " + org.Count + " programs. " +
                    "You must first assign new Divisions to the programs. ";
                TempData["HintMessage"] =   "Hint: You can create a new Division and assign or edit this ones details to cover all assigned programs.";
                return RedirectToAction(nameof(DivisionDelete), new { id });
            }
            else
            {
                Division division = await _context.Division.SingleOrDefaultAsync(d => d.Id == id);
                _context.Division.Remove(division);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DivisionList));
            }
        }
        [HttpPost, ActionName("DeleteDivLead")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDivLead(int id)
        {
            List<Division> div = await _context.Division.Where(d => d.DivLeadId == id).ToListAsync();
            if (div.Count > 0)
            {
                TempData["ErrorMessage"] = "The Secretary you have chosen is assigned to " + div.Count + " division(s). " +
                    "You must first assign new Secretaries to the programs. ";
                TempData["HintMessage"] =   "Hint: You can create a new Secretary and assign or edit this ones details to cover all assigned programs.";
                return RedirectToAction(nameof(DivLeadDelete), new { id });
            }
            else
            {
                DivLead lead = await _context.DivLead.SingleOrDefaultAsync(l => l.Id == id);
                _context.DivLead.Remove(lead);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DivLeadList));
            }
        }
        [HttpPost, ActionName("DeleteAnalyst")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAnalyst(int id)
        {
            List<OrgChart> orgChart = await _context.OrgChart.Where(o => o.AnalystId == id).ToListAsync();
            if (orgChart.Count > 0)
            {
                TempData["ErrorMessage"] = "The Analyst you have chosen is assigned to " + orgChart.Count + " programs. " +
                    "You must first assign new analysts to the programs. ";
                TempData["HintMessage"] =   "Hint: You can create a new analyst and assign or edit this ones details to cover all assigned programs.";
                return RedirectToAction(nameof(AnalystDelete), new { id });
            }
            else
            {
                Analyst analyst = await _context.Analyst.SingleOrDefaultAsync(a => a.Id == id);
                _context.Analyst.Remove(analyst);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AnalystList));
            }
        }

        //**************************************************************************
        //End CRUD operations
        //**************************************************************************


        //**************************************************************************
        //Start of helper methods
        //**************************************************************************
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