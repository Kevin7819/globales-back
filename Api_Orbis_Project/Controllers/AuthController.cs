using api.Constants;
using api.Custome;
using Api_Orbis_Project.Data;
using Api_Orbis_Project.Dtos;
using Api_Orbis_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly Utils _utils;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AppDbContext dbContext,
            Utils utils,
            ILogger<AuthController> logger)
        {
            _dbContext = dbContext;
            _utils = utils;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user.
        /// </summary>
        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Validation error",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                // Check unique username
                var existingUserByName = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Name == dto.UserName);
                if (existingUserByName != null)
                {
                    return Conflict(new
                    {
                        isSuccess = false,
                        message = UserConstants.UsernameExists
                    });
                }

                // Check unique email
                var existingUserByEmail = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (existingUserByEmail != null)
                {
                    return Conflict(new
                    {
                        isSuccess = false,
                        message = "Email is already registered"
                    });
                }

                var user = new User
                {
                    Name = dto.UserName,
                    Email = dto.Email,
                    Password = _utils.EncryptSHA256(dto.Password),
                    CountryOfOrigin = dto.CountryOfOrigin,
                    PreferredLanguage = dto.PreferredLanguage,
                    UserRole = Api_Orbis_Project.Models.User.Role.Passenger
                };


                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    isSuccess = true,
                    message = MessageConstants.EntityCreated("User"),
                    userId = user.UserId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering user");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    isSuccess = false,
                    message = "Internal server error",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Authenticate user and return JWT.
        /// </summary>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = MessageConstants.Generic.RequiredFields
                });
            }

            var hashed = _utils.EncryptSHA256(dto.Password);

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Name == dto.UserName && u.Password == hashed);

            if (user == null)
            {
                return Unauthorized(new
                {
                    isSuccess = false,
                    message = AuthConstants.InvalidCredentials
                });
            }

            var loginResponse = new LoginResponseDto
            {
                id = user.UserId,
                userName = user.Name!,
                email = user.Email ?? string.Empty,
                token = _utils.GenerateJWT(user), 
                role = user.UserRole.ToString()
            };

            return Ok(new
            {
                isSuccess = true,
                message = AuthConstants.LoginSuccess,
                user = loginResponse
            });
        }
    }
}
