using Leads_app.Data;
using Leads_app.Entities;
using Leads_app.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace Leads_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsController : Controller
    {
        private readonly DataContext _context;
        public LeadsController(DataContext dataContext) 
        {
            _context = dataContext;
        }

        [HttpPost("addLeads")]
        [Authorize]
        public async Task<ActionResult> AddLeads([FromBody] List<Leads> addLeads)
        {
            if (addLeads == null || !addLeads.Any())
            {
                return BadRequest("No leads provided");
            }
            
            try
            {

                string authorizationHeader = Request.Headers["Authorization"].ToString();
                var token = authorizationHeader[6..];
                var credentialAsString = Encoding.UTF8.GetString(Convert.FromBase64String(token));

                var credentials = credentialAsString.Split(":");

                var username = credentials[0];

                foreach (var lead in addLeads)
                {
                    lead.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    lead.DeliveredFrom = username;
                    var existingLead = await _context.Leads.FirstOrDefaultAsync(l => l.Email == lead.Email);


                    var isValid = IsValidEmail(lead.Email);
                    if (!isValid)
                    {
                        return Conflict($"inccorect email - {lead.Email}");
                    }

                    if (existingLead != null)
                    {
                        return Conflict($"Lead with email {lead.Email} already exists");
                    }
                }

                _context.Leads.AddRange(addLeads);
                await _context.SaveChangesAsync();

                return Ok("Leads added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        static bool IsValidEmail(string? email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            return Regex.IsMatch(email, pattern);
        }

        public IActionResult GetLeads(string search="123", int page = 1, int pageSize = 20)
        {
            try
            {
                IQueryable<Leads> query = _context.Leads;
                List<Leads> result = [.. query.Skip((page - 1) * pageSize).Take(pageSize)];

                var totalLeads = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalLeads / pageSize);

                
                ViewBag.Search = "search";
                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = page;

                return View(result);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            
        }

        [HttpPost]
        public void EditLeadWithEmail(EditUser editUser)
        {
            try
            {
                var leadToUpdate = _context.Leads.FirstOrDefault(u => u.Email == editUser.Email);

                if (leadToUpdate != null)
                {
                    leadToUpdate.DateCreated = editUser.DateCreated;
                    leadToUpdate.DeliveredFrom = editUser.DeliveredFrom;
                    leadToUpdate.FirstName = editUser.FirstName;
                    leadToUpdate.LastName = editUser.LastName;
                    leadToUpdate.PhoneNumber = editUser.PhoneNumber;
                    leadToUpdate.GeoLocation = editUser.GeoLocation;
                    leadToUpdate.Session = editUser.Session;
                    leadToUpdate.Source = editUser.Source;
                    leadToUpdate.Lander = editUser.Lander;
                    leadToUpdate.AdditionalInfo = editUser.AdditionalInfo;
                    leadToUpdate.IPAddress = editUser.IPAddress;
                    leadToUpdate.StatusId = editUser.StatusId;
                    leadToUpdate.StatusDateChange = editUser.StatusDateChange;

                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
