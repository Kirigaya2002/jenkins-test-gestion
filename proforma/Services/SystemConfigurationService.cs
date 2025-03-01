using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using proforma.Models;
using proforma.Models.DTO;

namespace proforma.Services
{
    public class SystemConfigurationService
    {
        private readonly ProformaContext _context;

        public SystemConfigurationService(ProformaContext context)
        {
            _context = context;
        }

        public async Task<List<SystemConfigurationDTO>> GetConfigurationsAsync(ulong userId)
        {
            var configurations = await _context.Configurations
                .Where(c => c.UserId == userId)
                .Select(c => new SystemConfigurationDTO
                {
                    Id = c.Id,
                    Key = c.Key,
                    Value = c.Value,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();
            return configurations;
        }

        public async Task<SystemConfigurationDTO?> GetConfigurationByIdAsync(ulong id)
        {
            var configuration = await _context.Configurations.FirstOrDefaultAsync(c => c.Id == id);
            if (configuration == null)
                return null;

            return new SystemConfigurationDTO
            {
                Id = configuration.Id,
                Key = configuration.Key,
                Value = configuration.Value,
                Description = configuration.Description,
                CreatedAt = configuration.CreatedAt,
                UpdatedAt = configuration.UpdatedAt
            };
        }

        public async Task<SystemConfigurationDTO> UpdateConfigurationAsync(ulong id, SystemConfigurationUpdateDTO dto)
        {
            var configuration = await _context.Configurations.FirstOrDefaultAsync(c => c.Id == id);
            if (configuration == null)
            {
                throw new Exception($"Configuration with ID {id} not found.");
            }

            // Solo se actualiza el Value, ya que las únicas opciones son "dark" o "light"
            configuration.Value = dto.Value;
            configuration.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new SystemConfigurationDTO
            {
                Id = configuration.Id,
                Key = configuration.Key,
                Value = configuration.Value,
                Description = configuration.Description,
                CreatedAt = configuration.CreatedAt,
                UpdatedAt = configuration.UpdatedAt
            };
        }
    }
}
