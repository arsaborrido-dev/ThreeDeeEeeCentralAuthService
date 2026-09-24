using AuthApi.Application.Portfolio.Contacts;
using AuthApi.Application.Portfolio.Contacts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthApi.Api.Controllers
{
    [ApiController]
    [Route("api/contacts")]
    public sealed class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AuthController> _logger;
        public ContactController(
            IContactService contactService,
            IWebHostEnvironment environment,
            ILogger<AuthController> logger)
        {
            _contactService = contactService;
            _environment = environment;
            _logger = logger;
        }

        // POST /api/contacts/message
        [HttpPost("message")]
        [EnableRateLimiting("api-strict")]
        [AllowAnonymous]
        public async Task<ActionResult<ContactMessageResponse>> SaveContactMessage(
            [FromBody] ContactMessageRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _contactService.SaveContactMessageAsync(
                request,
                cancellationToken);

            if (!result.Succeeded)
            {
                _logger.LogWarning("[ContactController:SaveContactMessage] - Message sending failed.");

                //return BadRequest(new ProblemDetails
                //{
                //    Status = StatusCodes.Status400BadRequest,
                //    Title = "CAPTCHA verification failed",
                //    Detail = "Please complete the CAPTCHA verification and try again."
                //});
                                
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Message sending failed.",
                    detail: _environment.IsDevelopment()
                        ? result.ErrorMessage
                        : "Message sending failed. Please try again later.");
            }

            //var response = result.Data;

            //return Ok(response);
            //return NoContent();
            return StatusCode(StatusCodes.Status200OK);

        }        
    }
}
