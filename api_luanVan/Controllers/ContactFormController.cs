using api_LuanVan.DataTransferObject;
using api_LuanVan.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_LuanVan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactFormController : ControllerBase
    {
        private readonly DbluanvantotnghiepContext _context;
        public ContactFormController(DbluanvantotnghiepContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_ContactForm>>> GetAllContactForms()
        {
            return await _context.ContactForms
                .Select(m => new DTO_ContactForm
                {
                    ContactId = m.ContactId,
                    UserId = m.UserId,
                    Content = m.Content,
                    CreateAt = m.CreateAt
                }).ToListAsync();
        }

        [HttpGet("contactform/{userid}")]
        public async Task<ActionResult<DTO_ContactForm>> GetContactForm(string userid)
        {
            var contactform = await _context.ContactForms.Where(m => m.UserId == userid).ToListAsync();
            if (contactform == null || contactform.Count == 0)
                return NotFound();
            return Ok(contactform);
        }

        [HttpPost]
        public async Task<ActionResult<DTO_ContactForm>> CreateContactForm([FromBody] DTO_ContactForm dto)
        {
            var contactForm = new ContactForm
            {
                UserId = dto.UserId,
                Content = dto.Content,
                CreateAt = DateTime.Now
            };
            _context.ContactForms.Add(contactForm);
            await _context.SaveChangesAsync();
            dto.ContactId = contactForm.ContactId;
            return CreatedAtAction(nameof(GetContactForm), new { userid = dto.UserId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DTO_ContactForm>> UpdateContactForm(int id, [FromBody] DTO_ContactForm dto)
        {
            if (id != dto.ContactId)
                return BadRequest();

            var contactForm = await _context.ContactForms.FindAsync(id);
            if (contactForm == null)
                return NotFound();

            //contactForm.UserId = dto.UserId;
            contactForm.Content = dto.Content;
            contactForm.CreateAt = DateTime.Now;
            _context.Entry(contactForm).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(dto);
        }
    }
}
