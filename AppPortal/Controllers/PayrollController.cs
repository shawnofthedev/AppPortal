using System;
using System.Collections.Generic;
using System.Linq;
using AppPortal.Models;
using Microsoft.AspNetCore.Mvc;
using AppPortal.ViewModels;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.IO;

namespace AppPortal.Controllers
{
    public class PayrollController : Controller 
    {
        //**************************************************************************
        //DB context stuff
        //**************************************************************************
        private readonly FinanceContext _context;
        public PayrollController(FinanceContext context)
        {
            _context = context; 
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose(); 
        }
        //**************************************************************************
        //End db context
        //**************************************************************************
 

        public IActionResult Index()
        {
            return View();
        }

        // GET: initial query form for leavebuyback
        public IActionResult LeaveBuyBack()
        {
            return View();
        }

        //get applicable employee information based on emp ID
        public IActionResult Query(MunisVw_EmployeeMaster munisVw_EmployeeMaster)
        { 
            //lets populate some variable to be used in error checking 
            int empNum = munisVw_EmployeeMaster.EmployeeNumber;
            int currentFy = GetFyYear(); //lets use the function that returns the current fiscal year 
            var currentPayDate = GetPayPeriodEndDate().Date;


            //query the employee master table to return an employee object based on the employee number
            MunisVw_EmployeeMaster employee = _context.MunisVw_EmployeeMaster.SingleOrDefault(m => m.EmployeeNumber == empNum);

            //check if employee exist and is active
            if (employee == null || employee.ActiveStatusCode == "I")
            {
                TempData["ErrorMessage"] = "Invalid employee number entered"; 
                return RedirectToAction("LeaveBuyBack", "Payroll");
            }

            //lets get all the request of that employee so we can check if they have requested two this fiscal year
            List<LeaveBuyBackRequest> getAllRequests = _context.LeaveBuyBackRequest.Where(m => m.EmpNum == empNum).ToList(); 
            
            //now we count up how many request that employee has made this pay period and fiscal year
            int requestCount = getAllRequests.Where(m => m.FiscalYear == currentFy).Count();
            int payPeriodRequestCount = getAllRequests.Where(m => m.PayPeriodEnd == currentPayDate).Count();

            //first lets see if they have requested one for the current pay period
            if (payPeriodRequestCount > 0)
            {
                TempData["ErrorMessage"] = "Employee " + empNum + " request already submitted for this pay period";
                //return RedirectToAction("LeaveBuyBack", "Payroll");
                return Redirect("LeaveBuyBack");
            }
               
            //lets not allow a request to be made if they have 2 this year
            if (requestCount >= 2)
            {
                TempData["ErrorMessage"] = "Employee " + empNum + " has requested buy back 2 times this year";
                return RedirectToAction("LeaveBuyBack", "Payroll");
            }


            //get employees leave balance
            MunisVw_EmployeeAnnual annual = _context.MunisVw_EmployeeAnnual.SingleOrDefault(h => h.EmployeeNumber == empNum); 
            decimal hoursAvailable = Convert.ToDecimal(annual.AnnualAvailable);
            if (hoursAvailable < 120)
            {
                TempData["ErrorMessage"] = "Employee " + empNum + " does not have the minimum 120 hours available";
                return RedirectToAction("LeaveBuyBack", "Payroll");
            }

            //build employees full name
            string fullName = employee.FirstName.Trim(); 
            if (employee.MI.Trim() != "")
            {
                fullName += " " + employee.MI.Trim() + " " + employee.LastName.Trim();
            }
            else
            {
                fullName += " " + employee.LastName.Trim();
            }

            //populate leavebuyback object
            //to send to the leave buy back form view
            LeaveBuyBackRequest request = new LeaveBuyBackRequest
            {
                EmpNum = employee.EmployeeNumber,
                EmpName = fullName,
                PayPeriodEnd = currentPayDate,
                RequestBy = User.Identity.Name.Remove(0, 5),
                RequestDate = DateTime.Today,
                ProgramNum = employee.WorkLocCd,
                AvailableHours = hoursAvailable,
                FiscalYear = currentFy,
            };

            return View("LeaveBuyBackForm", request);
        }

        //action to save the request object to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(LeaveBuyBackRequest leaveBuyBackRequest)
        {

            //here we check if all fields are valid
            if (!ModelState.IsValid)
            {
                if (leaveBuyBackRequest.HoursRequested > 40)
                {
                    TempData["ErrorMessage"] = "Cannot request more than 40 hours";
                }
                return View("LeaveBuyBackForm", leaveBuyBackRequest);
            }

            //add the leavebuyback object to the dbset
            if (leaveBuyBackRequest.Id == 0)
            {
                _context.LeaveBuyBackRequest.Add(leaveBuyBackRequest);

                //save to database and return to submit another if needed.
                _context.SaveChanges();
                TempData["Success"] = "Request Successfully Submitted!";
                return RedirectToAction("LeaveBuyBack", "Payroll");
            }
            else
            {
                LeaveBuyBackRequest buyBackInDb = _context.LeaveBuyBackRequest.Single(r => r.Id == leaveBuyBackRequest.Id);

                buyBackInDb.EmpNum = leaveBuyBackRequest.EmpNum;
                buyBackInDb.EmpName = leaveBuyBackRequest.EmpName;
                buyBackInDb.PayPeriodEnd = leaveBuyBackRequest.PayPeriodEnd;
                buyBackInDb.HoursRequested = leaveBuyBackRequest.HoursRequested;
                buyBackInDb.RequestDate = leaveBuyBackRequest.RequestDate;
                buyBackInDb.RequestBy = leaveBuyBackRequest.RequestBy;
                buyBackInDb.ProgramNum = leaveBuyBackRequest.ProgramNum;
                buyBackInDb.AvailableHours = leaveBuyBackRequest.AvailableHours;
                buyBackInDb.FiscalYear = leaveBuyBackRequest.FiscalYear;

                _context.SaveChanges();

                return RedirectToAction("BuyBackReport", "Payroll");
            }

        }

        public IActionResult BuyBackReport()
        {
            if (User.Identity.Name.Remove(0, 5) == "shawvota" 
                || User.Identity.Name.Remove(0, 5) == "junewalk" 
                || User.Identity.Name.Remove(0, 5) == "DEVOPHEA" 
                || User.Identity.Name.Remove(0, 5) == "alexarma" 
                || User.Identity.Name.Remove(0, 5) == "SUSIWOLF" 
                || User.Identity.Name.Remove(0, 5) == "angevota")
            {
                //grab a list of all requests
                var list = _context.LeaveBuyBackRequest.ToList();
                //now lets narrow down the list to just the unique dates
                var result = list.GroupBy(m => m.PayPeriodEnd).Select(grp => grp.First().PayPeriodEnd).ToList();

                //send the result over to the viewmodel to populate the dropdown on screen
                var viewModel = new BuyBackRequestViewModel
                {
                    LeaveBuyBackRequests = result
                };
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

        } 
        public IActionResult ReportViewer(BuyBackRequestViewModel PayDate)
        {
            var queryDate = PayDate.LeaveBuyBackRequests.Single();
            TempData["queryDate"] = queryDate;
            List<LeaveBuyBackRequest> request = _context.LeaveBuyBackRequest.Where(r => r.PayPeriodEnd == queryDate).ToList();

            return View(request);
        }

        public IActionResult ReportDetail(int id)
        {
            var request = _context.LeaveBuyBackRequest.SingleOrDefault(r => r.Id == id);

            if (request == null)
                return NotFound();

            return View(request);
        }


        //**************************************************************************
        //Begin Section if calculation used in this controller only
        //**************************************************************************
        //gets the current pay period end date
        public DateTime GetPayPeriodEndDate()
        {
            DateTime startTime = new DateTime(2018, 09, 19); //seed the calculation with a valid date
            DateTime now = DateTime.Today; //populate variable with the current date
            TimeSpan diff = now.Subtract(startTime); //now get the difference in days of the two
            int daysToEndPeriod = diff.Days % 14; //now get the modulus to know how many days are left until the end of the pay period
            if (daysToEndPeriod == 0)
            {
                return now.AddDays(-4).Date;
            }
            else
            {
                return DateTime.Now.AddDays(10 - daysToEndPeriod).Date;
            }
        }
 
        //gets the current fiscal year
        public static int GetFyYear()
        {
            int year = Convert.ToInt32(DateTime.Today.Year);

            if (DateTime.Today.Month >= 10)
            {
                return year + 1;
            }
            else
            {
                return year;
            }
        } 
        //**************************************************************************
        //End Section
        //************************************************************************** 
    }
}