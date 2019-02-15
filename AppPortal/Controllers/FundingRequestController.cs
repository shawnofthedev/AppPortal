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
        //context stuff
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
        //End context
        //**************************************************************************


        public IActionResult Index()
        {
            string userName = User.Identity.Name.Remove(0, 5) + "@nc-cherokee.com";
            //Todo **Filter list down to not show withdrawn requests
            List<CapFundingRequest> requestList = _context.CapFundingRequests
                .Where(r => r.Initiator == userName)
                .Where(r => r.RequestStatus != "Withdrawn")
                .OrderBy(r => r.TimeStamp).ToList();
            return View(requestList);
        }

        //**************************************************************************
        //New Actions
        //**************************************************************************
        public IActionResult New()
        {
            return View();
        }

        public async Task<IActionResult> NewCost(int? id)
        {
            if (id == null)
                return NotFound();
            CapFundingRequest request = await _context.CapFundingRequests.SingleOrDefaultAsync(r => r.Id == id);
            if (request == null)
                return NotFound();

            StaggeredCostViewModel viewModel = new StaggeredCostViewModel
            {
                CapFundingRequest = request,
                StaggeredCosts = _context.StaggeredCosts.Where(c => c.CapfundingRequestId == id)
                    .OrderBy(c => c.FiscalYear).ToList(),
                StaggeredCost = new StaggeredCost()
            };

            return View("StaggeredCostForm", viewModel);
        }

        public async Task<IActionResult> NewQuote(int? id)
        {
            if (id == null)
                return NotFound();

            CapFundingRequest request = await _context.CapFundingRequests.SingleOrDefaultAsync(r => r.Id == id);
            if (request == null)
                return NotFound();

            FundingRequestViewModel viewModel = new FundingRequestViewModel
            {
                CapFundingRequest = request,
                AttachedQuote = new AttachedQuote(),
                QuoteAttachments = await _context.QuoteAttachments.Where(a => a.AttachedQuoteId == id).ToListAsync()
            };

            return View("QuoteForm",viewModel);
        }

        //Load the form for a new request
        public async Task<IActionResult> FundingRequestForm(Vw_DivisionMaster vw_DivisionMaster)
        {

            string programNum = vw_DivisionMaster.ProgramNo;
            Vw_DivisionMaster programDetail = await _context.Vw_DivisionMaster.SingleOrDefaultAsync(p => p.ProgramNo == programNum);

            //check to make sure its a valid program
            if (programDetail == null)
            {
                TempData["ErrorMessage"] = "Invalid program number entered";
                return View("New");
            }

            //Instatiate an empty CapFundingRequest and populate it
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

            //Instantiate an empty viewModel
            FundingRequestViewModel viewModel = new FundingRequestViewModel
            {
                CapFundingRequest = request,
                FundingRequestAttachments = _context.FundingRequestAttachments.Where(a => a.CapFundingRequestId == request.Id),
                StaggeredCosts = _context.StaggeredCosts.Where(s => s.CapfundingRequestId == request.Id),
            };

            return View(viewModel);
        }
        //**************************************************************************
        //End New Actions
        //**************************************************************************

        //**************************************************************************
        //Start Edit Actions
        //**************************************************************************

        public async Task<IActionResult> Edit(int id)
        {
            CapFundingRequest request = await _context.CapFundingRequests.SingleOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                TempData["ErrorMessage"] = "No active request found";
                return View("Index");
            }
            else
            {
                object getNewViewModel = await CreateViewModel(request);
                return View("FundingRequestForm", getNewViewModel);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            CapFundingRequest objFundingRequest = await _context.CapFundingRequests.SingleOrDefaultAsync(r => r.Id == id);
            object viewModel = await CreateViewModel(objFundingRequest);

            return View("FundingRequestReview", viewModel);
        }

        public async Task<IActionResult> EditQuote(int id)
        {
            AttachedQuote quote = await _context.AttachedQuote.SingleOrDefaultAsync(q => q.Id == id);
            CapFundingRequest request = await _context.CapFundingRequests.SingleOrDefaultAsync(r => r.Id == quote.CapFundingRequestId);
            if (quote == null)
                return NotFound();
            else
            {
                FundingRequestViewModel viewModel = new FundingRequestViewModel
                {
                    CapFundingRequest = request,
                    AttachedQuote = quote,
                    QuoteAttachments = await _context.QuoteAttachments.Where(a => a.AttachedQuoteId == id).ToListAsync()
                };
                return View("QuoteForm", viewModel);
            }
        }
        //**************************************************************************
        //End Edit Actions
        //**************************************************************************

        //**************************************************************************
        //Start Save Actions
        //**************************************************************************
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(FundingRequestViewModel capFunding)
        {
            //instantiate the fundingrequest object
            CapFundingRequest objFundingRequest = capFunding.CapFundingRequest;

            if (objFundingRequest.ProjectName == "" || objFundingRequest.ProgramName == null)
            {
                TempData["ErrorMessage"] = "Must have a Project/Purchase Name";
                object getNewViewModel = await CreateViewModel(objFundingRequest);
                return View("FundingRequestForm", getNewViewModel);
            }

            if (objFundingRequest.OneTimePurchase == false && objFundingRequest.RecurringNeed == false && objFundingRequest.ReplaceAsset == false)
            {
                TempData["ErrorMessage"] = "You must choose at least one purchase reason";
                object getNewViewModel = await CreateViewModel(objFundingRequest);
                return View("FundingRequstForm", getNewViewModel);
            }

            if (objFundingRequest.AmtOtherSource > 0)
            {
                if (objFundingRequest.OtherSourceExplain == "" || objFundingRequest.OtherSourceExplain == null)
                {
                    TempData["ErrorMessage"] = "You must provide an explanation when using other funding sources";
                    object getNewViewModel = await CreateViewModel(objFundingRequest);
                    return View("FundingRequestForm", getNewViewModel);
                }
            }

            //look up the assets information
            string assetNum = objFundingRequest.AssetNum;
            MunisVw_fa_master assetDetail = _context.MunisVw_Fa_Master.SingleOrDefault(a => a.a_asset_number == assetNum);

            //if we have assets to replace then lets make sure the asset is real
            if (objFundingRequest.ReplaceAsset == true)
            {
                if (assetDetail == null)
                {
                    TempData["ErrorMessage"] = "Invalid asset number entered";
                    object getNewViewModel = await CreateViewModel(objFundingRequest);
                    return View("FundingRequestForm", getNewViewModel);
                }
                //now that we have a valid asset lets populate the
                //remaining items of the funding request object
                objFundingRequest.AssetYear = assetDetail.fa_model_year;
                objFundingRequest.Make = assetDetail.fm_manuf_code;
                objFundingRequest.Serial = assetDetail.fa_serial_number;
                objFundingRequest.AssetDesc = assetDetail.a_asset_desc;
            }

            //if this is a new request
            if (objFundingRequest.Id == 0)
            {

                //add together the funding amounts for a total
                objFundingRequest.TotalCost = objFundingRequest.AmtRequest + objFundingRequest.AmtOtherSource;
                objFundingRequest.RequestStatus = "In Progress";

                await _context.CapFundingRequests.AddAsync(objFundingRequest);
            }
            else
            {
                CapFundingRequest requestInDb = await _context.CapFundingRequests.SingleAsync(r => r.Id == objFundingRequest.Id);

                requestInDb.ProgramNum = objFundingRequest.ProgramNum;
                requestInDb.ProgramName = objFundingRequest.ProgramName;
                requestInDb.DivisionName = objFundingRequest.DivisionName;
                requestInDb.DivLead = objFundingRequest.DivLead;
                requestInDb.DivLeadEmail = objFundingRequest.DivLeadEmail;
                requestInDb.Initiator = objFundingRequest.Initiator;
                requestInDb.TimeStamp = objFundingRequest.TimeStamp;
                requestInDb.LastUpdate = DateTime.Now;
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
                requestInDb.RequestStatus = "In Progress";
            }

            await _context.SaveChangesAsync();

            object viewModel = await CreateViewModel(objFundingRequest);

            return View("FundingRequestReview", viewModel);
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
            string dir = Path.Combine("E:\\ApplicationDocuments\\FundingRequests\\BudgetExplanations\\",
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

            CapFundingRequest objRequest = await _context.CapFundingRequests.SingleOrDefaultAsync(r => r.Id == requestId);

            object viewModel = await CreateViewModel(objRequest);

            return View("FundingRequestReview", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCost(StaggeredCostViewModel costViewModel)
        {
            StaggeredCost objStaggeredCost = costViewModel.StaggeredCost;

            if (objStaggeredCost.Id == 0)
            {
                objStaggeredCost.CapfundingRequestId = costViewModel.CapFundingRequest.Id;

                await _context.StaggeredCosts.AddAsync(objStaggeredCost);
            }
            else
            {
                StaggeredCost costInDb = await _context.StaggeredCosts.SingleAsync(c => c.Id == costViewModel.StaggeredCost.Id);

                costInDb.FiscalYear = costViewModel.StaggeredCost.FiscalYear;
                costInDb.Amount = costViewModel.StaggeredCost.Amount;
                costInDb.AmtJustification = costViewModel.StaggeredCost.AmtJustification;
                costInDb.DescOfActivity = costViewModel.StaggeredCost.DescOfActivity;
                costInDb.CapfundingRequestId = costViewModel.StaggeredCost.CapfundingRequestId;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { costViewModel.CapFundingRequest.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveQuote(FundingRequestViewModel request, List<IFormFile> files)
        {
            int requestId = request.CapFundingRequest.Id;
            //Check if there is a file and if its in the right format
            if (files.Count > 0)
            {
                foreach (IFormFile formFile in files)
                {
                    string ext = Path.GetExtension(formFile.FileName);
                    if (formFile.Length < 0 || ext != ".pdf")
                    {
                        TempData["ErrorMessage"] = "Files not in the correct format.";
                        return RedirectToAction(nameof(Details), new { request.CapFundingRequest.Id});
                    }
                }
            }

            //write to the database now that we know
            AttachedQuote objAttachedQuote = request.AttachedQuote;
            if (objAttachedQuote.Id == 0)
            {
                objAttachedQuote.CapFundingRequestId = request.CapFundingRequest.Id;
                await _context.AttachedQuote.AddAsync(objAttachedQuote);

                if (files.Count > 0)
                {
                    await _context.SaveChangesAsync();
                    AttachedQuote LastQuote = await _context.AttachedQuote.LastAsync();
                    int quoteId = LastQuote.Id;
                    //build path to folder to store file
                    string dir = Path.Combine("E:\\ApplicationDocuments\\FundingRequests\\Quotes\\",
                        request.CapFundingRequest.ProjectName);
                    Directory.CreateDirectory(dir); //this only happens when the directory doesnt exist
                    foreach (IFormFile formFile in files)
                    {
                        QuoteAttachments quoteAttachment = new QuoteAttachments
                        {
                            FileName = formFile.FileName,
                            FileLocation = Path.Combine(dir, formFile.FileName),
                            AttachedQuoteId = quoteId,
                        };
                        QuoteAttachments showMeTheQuoteAttachment = quoteAttachment;
                        await _context.QuoteAttachments.AddAsync(quoteAttachment);
                        UploadQuote(formFile, dir);
                    }
                }
            }
            else
            {
                AttachedQuote quoteInDb = await _context.AttachedQuote.SingleAsync(c => c.Id == objAttachedQuote.Id);

                quoteInDb.VendorName = request.AttachedQuote.VendorName;
                quoteInDb.ContactNum = request.AttachedQuote.ContactNum;
                quoteInDb.ContactName = request.AttachedQuote.ContactName;
                quoteInDb.QuoteAmt = request.AttachedQuote.QuoteAmt;
                quoteInDb.QuoteDate = request.AttachedQuote.QuoteDate;

                if (files.Count > 0)
                {
                    List<QuoteAttachments> attachmentInDb = await _context.QuoteAttachments.Where(a => a.AttachedQuoteId == quoteInDb.Id).ToListAsync();
                    //first we will loop through the attachments and delete them
                    foreach (var attachment in attachmentInDb)
                    {
                        if(attachment.FileLocation != null || attachment.FileLocation != "")
                        {
                            System.IO.File.Delete(attachment.FileLocation);
                        }
                        _context.QuoteAttachments.Remove(attachment);
                    }
                    //now that we have cleared it all lets upload the new attachments
                    //build path to folder to store file
                    string dir = Path.Combine("E:\\ApplicationDocuments\\FundingRequests\\Quotes\\",
                        request.CapFundingRequest.ProjectName);
                    Directory.CreateDirectory(dir); //this only happens when the directory doesnt exist
                    foreach (IFormFile formFile in files)
                    {
                        QuoteAttachments quoteAttachment = new QuoteAttachments
                        {
                            FileName = formFile.FileName,
                            FileLocation = Path.Combine(dir, formFile.FileName),
                            AttachedQuoteId = quoteInDb.Id,
                        };
                        QuoteAttachments showMeTheQuoteAttachment = quoteAttachment;
                        await _context.QuoteAttachments.AddAsync(quoteAttachment);
                        UploadQuote(formFile, dir);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { request.CapFundingRequest.Id });
        }

        //**************************************************************************
        //End Save Actions
        //**************************************************************************
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

            object viewModel = await CreateViewModel(request);

            return View(viewModel);
        }

        public async Task<IActionResult> EditStagCost(int? id)
        {
            if (id == null)
                return NotFound();
            StaggeredCost staggeredCost = await _context.StaggeredCosts.SingleOrDefaultAsync(s => s.Id == id);
            CapFundingRequest request = await _context.CapFundingRequests.SingleOrDefaultAsync(r => r.Id == staggeredCost.CapfundingRequestId);
            //TempData["requestId"] = staggeredCost.CapfundingRequestId;

            if (staggeredCost == null)
                return NotFound();
            else
            {
                StaggeredCostViewModel viewModel = new StaggeredCostViewModel
                {
                    CapFundingRequest = request,
                    StaggeredCost = staggeredCost,
                    StaggeredCosts = await _context.StaggeredCosts.Where(c => c.CapfundingRequestId == staggeredCost.CapfundingRequestId)
                        .OrderBy(c => c.FiscalYear).ToListAsync()
                };

                return View("StaggeredCostForm", viewModel);
            }

        }

        // GET: StaggeredCost/Delete/
        public async Task<IActionResult> DeleteStagCost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StaggeredCost staggeredCost = await _context.StaggeredCosts.SingleOrDefaultAsync(s => s.Id == id);

            if (staggeredCost == null)
            {
                return NotFound();
            }

            return View(staggeredCost);
        }

        // POST: StaggeredCost/Delete/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStagCost(int id)
        {
            StaggeredCost staggeredCost = await _context.StaggeredCosts.FirstOrDefaultAsync(s => s.Id == id);
            //get the parent object so we can redirect to the
            //appropriate view since it errors out on a redirect to action
            CapFundingRequest request = await _context.CapFundingRequests.SingleOrDefaultAsync(r => r.Id == staggeredCost.CapfundingRequestId);

            _context.StaggeredCosts.Remove(staggeredCost);
            await _context.SaveChangesAsync();

            object viewModel = await CreateViewModel(request);

            return View("FundingRequestReview", viewModel);
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

            FundingRequestViewModel viewModel = new FundingRequestViewModel
            {
                CapFundingRequest = request,
                FundingRequestAttachments = _context.FundingRequestAttachments.Where(a => a.CapFundingRequestId == request.Id),
            };

            return View("SupportingDocuments", viewModel);
        }

        public async Task<IActionResult> DeleteQuote(int? id)
        {
            if (id == null)
                return NotFound();

            AttachedQuote attachment = await _context.AttachedQuote
                .SingleOrDefaultAsync(q => q.Id == id);

            if (attachment == null)
                return NotFound();

            return View(attachment);
        }

        [HttpPost, ActionName("DeleteQuoteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuoteConfirmed(int id)
        {
            //grab everthing we need from the database
            AttachedQuote quote = await _context.AttachedQuote.SingleOrDefaultAsync(q => q.Id == id);
            CapFundingRequest request = await _context.CapFundingRequests.SingleOrDefaultAsync(r => r.Id == quote.CapFundingRequestId);
            List<QuoteAttachments> attachments = await _context.QuoteAttachments.Where(a => a.AttachedQuoteId == id).ToListAsync();

            foreach(var attachment in attachments)
            {
                if (attachment.FileLocation != null || attachment.FileLocation != "")
                {
                    System.IO.File.Delete(attachment.FileLocation);
                }
                _context.QuoteAttachments.Remove(attachment);
            }

            _context.AttachedQuote.Remove(quote);
            await _context.SaveChangesAsync();

            object getNewViewModel = await CreateViewModel(request);

            return View("FundingRequestReview", getNewViewModel);
        }


        //display pdf file in browser
        public IActionResult GetFile(int id)
        {
            FundingRequestAttachments file = _context.FundingRequestAttachments.Single(f => f.Id == id);
            return new PhysicalFileResult(file.FileLocation, "Application/pdf");
        }
        //**************************************************************************
        //File Upload helper funcion
        //**************************************************************************
        public void UploadQuote(IFormFile files, string dir)
        {
            string fileName = Path.GetFileName(files.FileName);
            string filePath = Path.Combine(dir, fileName);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                files.CopyToAsync(stream);
            }
        }
        //**************************************************************************
        //End File Upload helper funcion
        //**************************************************************************

        //**************************************************************************
        //ViewModel Creation helper funcion
        //**************************************************************************
        public async Task<object> CreateViewModel(CapFundingRequest fundingRequest)
        {
            //if we have new request coming in and we need to build a
            //viewmodel with only the basic requests
            if (fundingRequest.Id == 0)
            {
                FundingRequestViewModel viewModel = new FundingRequestViewModel
                {
                    CapFundingRequest = fundingRequest,
                    FundingRequestAttachments = await _context.FundingRequestAttachments
                        .Where(a => a.CapFundingRequestId == fundingRequest.Id).ToListAsync(),
                    StaggeredCosts = await _context.StaggeredCosts
                        .Where(c => c.CapfundingRequestId == fundingRequest.Id)
                        .OrderBy(c => c.FiscalYear).ToListAsync(),
                    AttachedQuotes = await _context.AttachedQuote
                        .Where(q => q.CapFundingRequestId == fundingRequest.Id).ToListAsync(),
                };
                return viewModel;
            }
            else
            {
                CapFundingRequest objRequest = await _context.CapFundingRequests.SingleOrDefaultAsync(r => r.Id == fundingRequest.Id);
                FundingRequestViewModel viewModel = new FundingRequestViewModel
                {
                    CapFundingRequest = objRequest,
                    FundingRequestAttachments = await _context.FundingRequestAttachments
                        .Where(a => a.CapFundingRequestId == objRequest.Id).ToListAsync(),
                    StaggeredCosts = await _context.StaggeredCosts
                        .Where(c => c.CapfundingRequestId == objRequest.Id)
                        .OrderBy(c => c.FiscalYear).ToListAsync(),
                    AttachedQuotes = await _context.AttachedQuote
                        .Where(q => q.CapFundingRequestId == fundingRequest.Id).ToListAsync(),
                };
                return viewModel;
            }
        }
        //**************************************************************************
        //End ViewModel Creation helper funcion
        //**************************************************************************
    }
}