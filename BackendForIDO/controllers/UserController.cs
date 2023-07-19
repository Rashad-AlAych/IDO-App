using System;
using BackendForIDO.Models;
using BackendForIDO.Repositories;
using BackendForIDO.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace BackendForIDO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly AuthenticationService _authenticationService;

        public UserController(UserRepository userRepository, AuthenticationService authenticationService)
        {
            _userRepository = userRepository;
            _authenticationService = authenticationService;

            // Update the secret key used by the authentication service
            var secretKey = "b39e69edc87048b58688598f0704e6a8"; // Replace with your valid secret key
            _authenticationService.UpdateSecretKey(secretKey);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            // Perform validation on the request
            if (loginRequest == null)
            {
                return BadRequest("Invalid request body");
            }

            // Retrieve the user from the repository based on the provided username or email
            User user = _userRepository.GetUserByUsernameOrEmail(loginRequest.UsernameOrEmail);

            // Check if the user exists
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            // Verify the user's password
            bool isPasswordValid = user.Password == loginRequest.Password;

            if (!isPasswordValid)
            {
                return Unauthorized("Invalid credentials");
            }

            // Generate and return the JWT token
            string token = _authenticationService.GenerateToken(user);
            return Ok(new { Token = token });
        }

        [HttpGet("token")]
        [Authorize]
        public IActionResult GetUserByToken()
        {
            // Get the user ID from the authenticated user's claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name);
            
            if (userIdClaim == null)
            {
                return BadRequest("Invalid token");
            }

            // Parse the user ID from the claim
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid token");
            }

            // Get the user from the repository using the user ID
            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Return the user as the response
            return Ok(user);
        }
    

    }
}
