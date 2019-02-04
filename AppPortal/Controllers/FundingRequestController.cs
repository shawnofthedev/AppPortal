using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppPortal.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using AppPortal.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AppPortal.Controllers
{
    public class FundingRequestController : Controller
    {
        //**************************************************************************
        //DB context stuff
        //**************************************************************************
        private readonly FinanceContext _context;

        public FundingRequestController(FinanceContext context)
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
            string userName = User.Identity.Name.Remove(0, 5) + "@nc-cherokee.com";
            //Todo **Filter list down to not show withdrawn requests
            List<CapFundingRequest> requestList = _context.CapFundingRequests
                .Where(r => r.Initiator == userName)
                .Where(r => r.RequestStatus != "Withdrawn").ToList();
            return View(requestList);
        }

        public IActionResult New()
        {
            return View();
        }

        public IActionResult FundingRequestForm(Vw_DivisionMaster vw_DivisionMaster)
        {
            string programNum = vw_DivisionMaster.ProgramNo;
            Vw_DivisionMaster programDetail = _context.Vw_DivisionMaster.SingleOrDefault(p => p.ProgramNo == programNum); 

            if (programDetail == null)
            {
                TempData["ErrorMessage"] = "Invalid program number entered";
                return View("New");
            }

            CapFundingRequest request = new CapFundingRequest
            {
                ProgramNum = programDetail.ProgramNo,
                DivisionName = programDetail.DivisionName,
                ProgramName = programDetail.ProgramName,
                DivLead = programDetail.DivLead,
                DivLeadEmail = programDetail.DivLeadEmail,
                Initiator = User.Identity.Name.Remove(0, 5) + "@nc-cherokee.com",
                TimeStamp = System.DateTime.Now,
            };

            FundingRequestViewModel viewModel = new FundingRequestViewModel
            {
                CapFundingRequest = request,
                FundingRequestAttachments = _context.FundingRequestAttachments.Where(a => a.Id == request.Id),
            };
 
            return View(viewModel); 
        }

        public IActionResult Edit(int id)
        {
            CapFundingRequest request = _context.CapFundingRequests.SingleOrDefault(r => r.Id == id);

            if (request == null)
            {
                TempData["ErrorMessage"] = "No active request found";
                return View("Index");
            }
            else
            {
                FundingRequestViewModel viewModel = new FundingRequestViewModel
                {
                    CapFundingRequest = request,
                    FundingRequestAttachments = _context.FundingRequestAttachments.Where(a => a.CapFundingRequestId == id).ToList(),
                    StaggeredCosts = _context.StaggeredCosts.Where(s => s.CapfundingRequestId == id).ToList(),
                };

                return View("FundingRequestForm", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(FundingRequestViewModel capFunding) 
        {
            //instantiate the fundingrequest object
            CapFundingRequest objFundingRequest = capFunding.CapFundingRequest;

            //if this is a new request
            if (objFundingRequest.Id == 0)
            {
                //if we have assets to replace then lets make sure the asset is real
                if (objFundingRequest.ReplaceAsset == true)
                {
                    //look up the assets information
                    string assetNum = objFundingRequest.AssetNum;
                    MunisVw_fa_master assetDetail = _context.MunisVw_Fa_Master.SingleOrDefault(a => a.a_asset_number == assetNum);

                    if (assetDetail == null)
                    { 
                        TempData["ErrorMessage"] = "Invalid asset number entered"; 
                        return View("FundingRequestForm", capFunding);
                    }
                    //now that we have a valid asset lets populate the
                    //remaining items of the funding request object
                    objFundingRequest.AssetYear = assetDetail.fa_model_year;
                    objFundingRequest.Make = assetDetail.fm_manuf_code;
                    objFundingRequest.Serial = assetDetail.fa_serial_number;
                    objFundingRequest.AssetDesc = assetDetail.a_asset_desc; 
                }


                //add together the funding amounts for a total
                objFundingRequest.TotalCost = objFundingRequest.AmtRequest + objFundingRequest.AmtOtherSource;
                objFundingRequest.RequestStatus = "Created";

                await _context.CapFundingRequests.AddAsync(objFundingRequest);
            }
            else
            {
                CapFundingRequest requestInDb = _context.CapFundingRequests.Single(r => r.Id == objFundingRequest.Id);

                requestInDb.ProgramNum = objFundingRequest.ProgramNum;
                requestInDb.ProgramName = objFundingRequest.ProgramName;
                requestInDb.DivisionName = objFundingRequest.DivisionName;
                requestInDb.DivLead = objFundingRequest.DivLead;
                requestInDb.DivLeadEmail = objFundingRequest.DivLeadEmail;
                requestInDb.Initiator = objFundingRequest.Initiator;
                requestInDb.TimeStamp = objFundingRequest.TimeStamp;
                requestInDb.LastUpdate = System.DateTime.Now;
                requestInDb.ProjectName = objFundingRequest.ProjectName;
                requestInDb.ProjectOverview = objFundingRequest.ProjectOverview;
                requestInDb.AmtRequest = objFundingRequest.AmtRequest;
                requestInDb.AmtOtherSource = objFundingRequest.AmtOtherSource;
                requestInDb.TotalCost = objFundingRequest.AmtRequest + objFundingRequest.AmtOtherSource;
                requestInDb.OtherSourceExplain = objFundingRequest.OtherSourceExplain;
                requestInDb.OneTimePurchase = objFundingRequest.OneTimePurchase;
                requestInDb.RecurringNeed = objFundingRequest.RecurringNeed;
                requestInDb.ReplaceAsset = objFundingRequest.ReplaceAsset;
                requestInDb.AssetYear = objFundingRequest.AssetYear;
                requestInDb.Make = objFundingRequest.Make;
                requestInDb.AssetNum = objFundingRequest.AssetNum;
                requestInDb.Serial = objFundingRequest.Serial;
                requestInDb.AssetDesc = objFundingRequest.AssetDesc;
                requestInDb.RequestStatus = "Edited";
            } 

            await _context.SaveChangesAsync();

            FundingRequestViewModel viewModel = new FundingRequestViewModel
            {
                CapFundingRequest = objFundingRequest,
                FundingRequestAttachments = _context.FundingRequestAttachments
                    .Where(a => a.CapFundingRequestId == objFundingRequest.Id).ToList(),
                StaggeredCosts = _context.StaggeredCosts
                    .Where(c => c.CapfundingRequestId == objFundingRequest.Id).ToList()
            };

            return View("Costs", viewModel); 

        }

        public async Task<IActionResult> SupportingDocuments(int? id)
        {
            if (id == null)
            {
                return NotFound(); 
            }

            CapFundingRequest request = await _context.CapFundingRequests.SingleOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound(); 
            }

            FundingRequestViewModel viewModel = new FundingRequestViewModel
            {
                CapFundingRequest = request,
                FundingRequestAttachments = _context.FundingRequestAttachments.Where(a => a.CapFundingRequestId == id).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> FileUpload(FundingRequestViewModel request, List<IFormFile> files)
        {
            //first lets verify we have a file and that it is the correct type
            foreach (IFormFile formFile in files)
            {
                string ext = Path.GetExtension(formFile.FileName);
                if (formFile.Length < 0 || ext != ".pdf")
                {
                    TempData["ErrorMessage"] = "Files must be in pdf format.";
                    return RedirectToAction(nameof(FundingRequestForm));
                }

                //todo Verify the file doesnt already exist and handle it accordingly
            }

            //build path to pdf folder
            string dir = Path.Combine("E:\\ApplicationDocuments\\FundingRequests\\pdf\\",
                request.CapFundingRequest.ProjectName + "_" + request.CapFundingRequest.Id);

            //create directory by default if one already exists it will ignore the line
            Directory.CreateDirectory(dir); 
            int requestId = request.CapFundingRequest.Id;

            //Now its time to copy the files to the directory 
            foreach (IFormFile formFile in files)
            {
                //get file size to make sure one is there
                long size = files.Sum(f => f.Length);
                string fileName = Path.GetFileName(formFile.FileName);
                string filePath = Path.Combine(dir, fileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                    FundingRequestAttachments attachments = new FundingRequestAttachments
                    {
                        FileName = fileName,
                        CapFundingRequestId = requestId,
                        FileLocation = filePath
                    };
                    await _context.FundingRequestAttachments.AddAsync(attachments);
                }
            } 
            await _context.SaveChangesAsync();

            var objRequest = await _context.CapFundingRequests.SingleOrDefaultAsync(r => r.Id == requestId);
            FundingRequestViewModel viewModel = new FundingRequestViewModel
            {
                CapFundingRequest = objRequest, 
                FundingRequestAttachments = _context.FundingRequestAttachments
                    .Where(a => a.CapFundingRequestId == objRequest.Id).ToList(),
                StaggeredCosts = _context.StaggeredCosts
                    .Where(c => c.CapfundingRequestId == objRequest.Id).ToList()
            };

            return View("FundingRequestForm", viewModel); 
        }

        // GET: CapFundingRequest/Delete/5
        public async Task<IActionResult> Withdraw(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CapFundingRequest request = await _context.CapFundingRequests
                .SingleOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }


        // POST: CapFundingRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            CapFundingRequest request = await _context.CapFundingRequests.SingleAsync(r => r.Id == id);

            request.RequestStatus = "Withdrawn";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteAttachment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            FundingRequestAttachments attachment = await _context.FundingRequestAttachments
                .SingleOrDefaultAsync(r => r.Id == id);

            if (attachment == null)
            {
                return NotFound();
            }

            return View(attachment);
        }

        // POST: FundingRequestAttachments/Delete/
        [HttpPost, ActionName("DelAttachmentConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DelAttachmentConfirmed(int id)
        {
            FundingRequestAttachments attachment = await _context.FundingRequestAttachments.SingleOrDefaultAsync(a => a.Id == id);
            //hold onto the parent request id so we can redirect to the correct form
            CapFundingRequest request = await _context.CapFundingRequests.SingleOrDefaultAsync(r => r.Id == attachment.CapFundingRequestId);
            //get the location of the file to delete
            string fileLocation = attachment.FileLocation;

            System.IO.File.Delete(fileLocation);
            _context.FundingRequestAttachments.Remove(attachment);
            await _context.SaveChangesAsync();

            var viewModel = new FundingRequestViewModel
            {
                CapFundingRequest = request,
                FundingRequestAttachments = _context.FundingRequestAttachments.Where(a => a.CapFundingRequestId == request.Id),
            };

            return View("SupportingDocuments", viewModel);
        }
    }
}