using Api_Orbis_Project.Data;
using Api_Orbis_Project.Dtos;
using Api_Orbis_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Api_Orbis_Project.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Obtener todos los usuarios
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    CountryOfOrigin = u.CountryOfOrigin,
                    PreferredLanguage = u.PreferredLanguage,
                    BirthDate = u.BirthDate,
                    Role = u.UserRole.ToString()
                })
                .ToListAsync();
        }

        // Obtener un usuario por Id
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => u.UserId == id)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    CountryOfOrigin = u.CountryOfOrigin,
                    PreferredLanguage = u.PreferredLanguage,
                    BirthDate = u.BirthDate,
                    Role = u.UserRole.ToString()
                })
                .FirstOrDefaultAsync();
        }

        // Crear un usuario
        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password, // encriptar
                CountryOfOrigin = dto.CountryOfOrigin,
                PreferredLanguage = dto.PreferredLanguage,
                BirthDate = dto.BirthDate,
                UserRole = User.Role.Passenger
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                CountryOfOrigin = user.CountryOfOrigin,
                PreferredLanguage = user.PreferredLanguage,
                BirthDate = user.BirthDate,
                Role = user.UserRole.ToString()
            };
        }

        // Actualizar un usuario
        public async Task<bool> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.CountryOfOrigin = dto.CountryOfOrigin;
            user.PreferredLanguage = dto.PreferredLanguage;
            user.BirthDate = dto.BirthDate;

            if (Enum.TryParse<User.Role>(dto.Role, out var parsedRole))
            {
                user.UserRole = parsedRole;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // Eliminar un usuario
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
