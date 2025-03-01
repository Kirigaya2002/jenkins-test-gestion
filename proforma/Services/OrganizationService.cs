using Microsoft.EntityFrameworkCore;
using proforma.Models;
using proforma.Models.DTO;
using proforma.Services;

namespace proforma.Services
{
    public class OrganizationService
    {
        private readonly ProformaContext _context;
        private readonly InventoryService _inventoryService;

        public OrganizationService(ProformaContext context, InventoryService inventoryService)
        {
            _context = context;
            _inventoryService = inventoryService;
        }

        // Obtener todas las organizaciones
        public async Task<IEnumerable<OrganizationDTO>> GetOrganizationsAsync()
        {
            return await _context.Organizations
                .Where(o => o.DeletedAt == null)
                .Select(o => new OrganizationDTO
                {
                    Id = o.Id,
                    Name = o.Name,
                    ManagerIdentification = o.ManagerIdentification,
                    Phone = o.Phone,
                    Email = o.Email,
                    Address = o.Address,
                    UserId = o.UserId
                })
                .ToListAsync();
        }

        // Obtener una organización por ID
        public async Task<OrganizationDTO?> GetOrganizationByIdAsync(ulong id)
        {
            return await _context.Organizations
                .Where(o => o.Id == id && o.DeletedAt == null)
                .Select(o => new OrganizationDTO
                {
                    Id = o.Id,
                    Name = o.Name,
                    ManagerIdentification = o.ManagerIdentification,
                    Phone = o.Phone,
                    Email = o.Email,
                    Address = o.Address,
                    UserId = o.UserId
                })
                .FirstOrDefaultAsync();
        }

        // Verificar si el nombre de la organización ya existe
        public async Task<bool> NameExistsAsync(string name)
        {
            return await _context.Organizations
                .AnyAsync(o => o.Name == name && o.DeletedAt == null);
        }

        // Crear una nueva organización
        public async Task<Organization> CreateOrganizationAsync(CreateOrganizationDTO createOrganizationDto)
        {
            var organization = new Organization
            {
                Name = createOrganizationDto.Name,
                ManagerIdentification = createOrganizationDto.ManagerIdentification,
                Phone = createOrganizationDto.Phone,
                Email = createOrganizationDto.Email,
                Address = createOrganizationDto.Address,
                UserId = createOrganizationDto.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync();

            //Cuando se crea una organización, se crea un inventario por defecto, llamando al servicio de inventario
            await _inventoryService.CreateAsync(new InventoryCreateDto
            {
                OrganizationId = (long)organization.Id,
                Details = null
            });
            await _context.SaveChangesAsync();


            return organization;
        }

        // Eliminar una organización por ID
        public async Task<bool> DeleteOrganizationAsync(ulong id)
        {
            var organization = await _context.Organizations
                .Where(o => o.Id == id && o.DeletedAt == null)
                .FirstOrDefaultAsync();
            if (organization == null)
            {
                return false;
            }
            organization.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // Restaurar una organización eliminada por ID
        public async Task<bool> RestoreOrganizationAsync(ulong id)
        {
            var organization = await _context.Organizations
                .Where(o => o.Id == id && o.DeletedAt != null)
                .FirstOrDefaultAsync();
            if (organization == null)
            {
                return false;
            }
            organization.DeletedAt = null;
            await _context.SaveChangesAsync();
            return true;
        }

        // Actualizar una organización por ID junto con el updateOrganizationDto
        public async Task<bool> UpdateOrganizationAsync(ulong id, UpdateOrganizationDTO updateOrganizationDto)
        {
            var organization = await _context.Organizations
                .Where(o => o.Id == id && o.DeletedAt == null)
                .FirstOrDefaultAsync();
            if (organization == null)
            {
                return false;
            }
            organization.Name = updateOrganizationDto.Name;
            organization.Phone = updateOrganizationDto.Phone;
            organization.Email = updateOrganizationDto.Email;
            organization.Address = updateOrganizationDto.Address;
            organization.ManagerIdentification = updateOrganizationDto.ManagerIdentification;
            organization.UserId = updateOrganizationDto.UserId;
            organization.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }


        // Obtener todas las organizaciones eliminadas de un usuario
        public async Task<IEnumerable<OrganizationDTO>> GetDeletedOrganizationsAsync(ulong userId)
        {
            return await _context.Organizations
                .Where(o => o.UserId == userId && o.DeletedAt != null)
                .Select(o => new OrganizationDTO
                {
                    Id = o.Id,
                    Name = o.Name,
                    ManagerIdentification = o.ManagerIdentification,
                    Phone = o.Phone,
                    Email = o.Email,
                    Address = o.Address,
                    UserId = o.UserId
                })
                .ToListAsync();
        }

        // Listar organizaciones que contengan el dato de busqueda, es por usuario y no general, además se lista solo las organizaciones que no han sido eliminadas
        public async Task<IEnumerable<OrganizationDTO>> SearchOrganizationsAsync(string searchText, ulong userId)
        {
            return await _context.Organizations
                .Where(o => o.UserId == userId && o.DeletedAt == null && (o.Name.Contains(searchText) || o.ManagerIdentification.Contains(searchText) || o.Phone.Contains(searchText) || o.Email.Contains(searchText) || o.Address.Contains(searchText)))
                .Select(o => new OrganizationDTO
                {
                    Id = o.Id,
                    Name = o.Name,
                    ManagerIdentification = o.ManagerIdentification,
                    Phone = o.Phone,
                    Email = o.Email,
                    Address = o.Address,
                    UserId = o.UserId
                })
                .ToListAsync();
        }

        //Validar correo electronico repetido
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Organizations.AnyAsync(u => u.Email == email);
        }

        //Validar telefono repetido
        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Organizations.AnyAsync(u => u.Phone == phone);
        }

        // Obtener todas las organizaciones de un usuario
        public async Task<IEnumerable<OrganizationDTO>> GetOrganizationsByUserIdAsync(ulong userId)
        {
            return await _context.Organizations
                .Where(o => o.UserId == userId && o.DeletedAt == null)
                .Select(o => new OrganizationDTO
                {
                    Id = o.Id,
                    Name = o.Name,
                    ManagerIdentification = o.ManagerIdentification,
                    Phone = o.Phone,
                    Email = o.Email,
                    Address = o.Address,
                    UserId = o.UserId
                })
                .ToListAsync();
        }
    }
}
