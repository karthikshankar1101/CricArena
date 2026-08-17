using CricArena.Business.DTOs.Auth;
using CricArena.Business.Exceptions;
using CricArena.Business.Services.Interfaces;
using CricArena.Core.Entities;
using CricArena.Core.Enums;
using CricArena.Data.Context;
using CricArena.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CricArena.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IPlayerRepository _playerRepository;

        public AuthService(
            IUserRepository userRepository,
            AppDbContext context,
            IConfiguration configuration,
            IPlayerRepository playerRepository)
        {
            _userRepository = userRepository;
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
            _playerRepository = playerRepository;
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Password is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                throw new ArgumentException("Phone number is required.");
            }

            if (await _userRepository.EmailExistsAsync(email))
            {
                throw new InvalidOperationException(
                    "A user with this email already exists.");
            }

            var phoneNumber = request.PhoneNumber.Trim();
            if (await _playerRepository.PhoneNumberExistsAsync(phoneNumber))
            {
                throw new DuplicatePhoneNumberException(phoneNumber);
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Role = ClubRole.Player,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            await _userRepository.AddAsync(user);

            var player = new Player
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = request.Name.Trim(),
                Email = email,
                PhoneNumber = phoneNumber,
                IsActive = true,
                CreatedOn = DateTime.UtcNow
            };

            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Your account is deactivated.");
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            return GenerateJwtToken(user);
        }

        private LoginResponse GenerateJwtToken(User user)
        {
            var key =
            _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT Key is not configured.");

            var issuer =
                _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT Issuer is not configured.");

            var audience =
                _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "JWT Audience is not configured.");

            var expirationMinutes =
                int.Parse(
                    _configuration["Jwt:ExpirationMinutes"]
                    ?? "60");

            var claims = new[]
            {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.Email,
                user.Email),

            new Claim(
                ClaimTypes.Role,
                user.Role.ToString())
        };

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key));

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);

            var expiresAt =
                DateTime.UtcNow.AddMinutes(
                    expirationMinutes);

            var token =
                new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: expiresAt,
                    signingCredentials: credentials);

            var tokenString =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);

            return new LoginResponse
            {
                Token = tokenString,
                ExpiresAt = expiresAt,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
