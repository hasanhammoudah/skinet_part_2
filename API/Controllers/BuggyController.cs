using System.Security.Claims;
using API.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

 [Authorize]
    public class BuggyController : BaseApiController
    
    {
        [AllowAnonymous]
        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorized()
        {
           return Unauthorized();
        }

         [HttpGet("badrequest")]
        public IActionResult GetBadRequest()
        {
           return BadRequest("Not a good request");
        }

         [HttpGet("notfound")]
        public IActionResult GetNotFound()
        {
           return NotFound();
        }

         [HttpGet("internalerror")]
        public IActionResult GetInternalError()
        {
           throw new Exception("This is a test exception");
        }
        [HttpPost("validationerror")]
        public IActionResult GetValidationError(CreateProductDto product)
        {
           return Ok();
        }

        [Authorize]
        [HttpGet("secret")]
        public IActionResult GetSecret()
        {
         var name = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
         var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
         return Ok("Hello " + name + " With the id of" + id);
        }
    }
