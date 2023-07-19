using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendForIDO.Models;
using Microsoft.IdentityModel.Tokens;
using BackendForIDO.Repositories;
using Microsoft.AspNetCore.Http;


namespace BackendForIDO.Services
{
    public class AuthenticationService
    {
        private string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly UserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(string secretKey, 
                                     string issuer, 
                                     string audience, 
                                     UserRepository userRepository,
                                     IHttpContextAccessor httpContextAccessor)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public void UpdateSecretKey(string secretKey)
        {
            _secretKey = secretKey;
        }


       public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            // Ensure the key size is at least 128 bits (16 bytes)
            if (key.Length < 32)
            {
                throw new InvalidOperationException("Invalid secret key size. The key size must be at least 256 bits (32 bytes).");
            }

            // Define the token's content and properties
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Set the subject of the token, which represents the user
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    // You can add more claims here if needed, such as roles or additional user information
                }),

                // Set the token's expiration date
                Expires = DateTime.UtcNow.AddDays(7),

                // Set the issuer and audience of the token
                Issuer = _issuer,
                Audience = _audience,

                // Set the signing credentials using a secret key
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Create the token based on the token descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return the generated token as a string
            return tokenHandler.WriteToken(token);
        }

        public int GetLoggedInUserId()
        {
            var user = _httpContextAccessor.HttpContext.User;

            // Check if the user is authenticated
            if (user.Identity.IsAuthenticated)
            {
                // Get the user ID from the claims
                var userId = user.FindFirstValue(ClaimTypes.Name);

                // Parse the user ID to an integer and return it
                return int.Parse(userId);
            }

            // If the user is not authenticated, you can return a default or throw an exception depending on your application's requirements
            // For example, you can return -1 or throw a custom exception indicating that the user is not authenticated
            throw new ApplicationException("User is not authenticated.");
        }
    }
}
