using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using proforma.Models;
using proforma.Models.DTO;

namespace proforma.Services
{
    public class ClientService
    {
        private readonly ProformaContext _context;

        public ClientService(ProformaContext context)
        {
            _context = context;
        }

        //Agregar clientes (modificacion: relacion cliente y organizacion)
        public async Task<Models.Client> CreateClientAsync(CreateClientDTO createClientDto, ulong organizationId)
        {
            var client = new Models.Client
            {
                Identification = createClientDto.Identification,
                Email = createClientDto.Email,
                Name = createClientDto.Name,
                Lastname = createClientDto.Lastname,
                Phone = createClientDto.Phone,
                Active = true, 
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            // Relacionar cliente con organización
            var organizationClient = new OrganizationClient
            {
                ClientId = client.Id,
                OrganizationId = organizationId
            };

            _context.OrganizationClients.Add(organizationClient);
            await _context.SaveChangesAsync(); // Guardar relación

            return client;
        }

        //Validar correo electronico repetido
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Clients.AnyAsync(u => u.Email == email);
        }

        //Validar telefono repetido
        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Clients.AnyAsync(u => u.Phone == phone);
        }

        //Validar identificacion dentro del sistema repetido
        public async Task<bool> DNIExistsAsync(string dni)
        {
            return await _context.Clients.AnyAsync(u => u.Identification == dni);
        }

        //Listado por medio de paginacion (nuevo)
        public async Task<(IEnumerable<ClientDTO> Clients, int TotalPages)> GetClientsByOrganizationAsync(ulong organizationId, int page, int pageSize)
        {
            var query = _context.OrganizationClients
               .Where(oc => oc.OrganizationId == organizationId) // Filtrar por organización
               .Select(oc => oc.Client) // Obtener los clientes relacionados
               .Where(c => c.DeletedAt == null); // Filtrar clientes activos

            int totalClients = await query.CountAsync(); 
            int totalPages = (int)Math.Ceiling(totalClients / (double)pageSize); 

            var clients = await query
               .OrderBy(c => c.Name) // Ordenar por nombre
               .Skip((page - 1) * pageSize) // Saltar los clientes de páginas anteriores
               .Take(pageSize) 
               .Select(c => new ClientDTO
               {
                   Id = c.Id,
                   Identification = c.Identification,
                   Name = c.Name,
                   Lastname = c.Lastname,
                   Email = c.Email,
                   Phone = c.Phone,
                   Active = c.Active
               })
               .ToListAsync();

            return (clients, totalPages);
        }

        //Listar todos los clientes deshabilitados (modificado)
        public async Task<(IEnumerable<ClientDTO> Clients, int TotalPages)> GetClientsEliminatedByOrganizationAsync(ulong organizationId, int page, int pageSize)
        {
            var query = _context.OrganizationClients
                .Where(oc => oc.OrganizationId == organizationId) // Filtrar organización
                .Select(oc => oc.Client) // Clientes relacionados
                .Where(c => c.DeletedAt != null); // Filtrar clientes eliminados

            int totalClients = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalClients / (double)pageSize);

            var clients = await query
                .OrderBy(c => c.Name) 
                .Skip((page - 1) * pageSize) 
                .Take(pageSize) 
                .Select(c => new ClientDTO
                {
                    Id = c.Id,
                    Identification = c.Identification,
                    Name = c.Name,
                    Lastname = c.Lastname,
                    Email = c.Email,
                    Phone = c.Phone,
                    Active = c.Active
                })
                .ToListAsync();

            return (clients, totalPages);
        }

        // Listar clientes que contengan las mismas caracteristicas (modificado)
        public async Task<IEnumerable<ClientDTO>> SearchClientsAsync(string searchText, ulong organizationId)
        {
            return await _context.OrganizationClients
                .Where(oc => oc.OrganizationId == organizationId) // Filtrar por organizacion
                .Select(oc => oc.Client) // Obtener los clientes relacionados a organizacion
                .Where(c => c.DeletedAt == null &&
                    (c.Name.Contains(searchText) ||
                     c.Identification.Contains(searchText) ||
                     c.Lastname.Contains(searchText) ||
                     c.Email.Contains(searchText) ||
                     c.Phone.Contains(searchText)))  
                .Select(c => new ClientDTO
                {
                    Id = c.Id,
                    Identification = c.Identification,
                    Name = c.Name,
                    Lastname = c.Lastname,
                    Email = c.Email,
                    Phone = c.Phone,
                    Active = c.Active
                })
                .ToListAsync();
        }

        // Obtener un cliente por ID
        public async Task<ClientDTO?> GetClientByIdAsync(ulong id)
        {
            return await _context.Clients
                .Where(o => o.Id == id && o.DeletedAt == null)
                .Select(o => new ClientDTO
                {
                    Id = o.Id,
                    Identification = o.Identification,
                    Name = o.Name,
                    Lastname = o.Lastname,
                    Email = o.Email,
                    Phone = o.Phone,
                    Active = o.Active,
                })
                .FirstOrDefaultAsync();
        }

        // Actualizar un cliente por ID
        public async Task<bool> UpdateClientAsync(ulong id, UpdateClientDTO updateClientDto)
        {
            var client = await _context.Clients
                .Where(o => o.Id == id && o.DeletedAt == null)
                .FirstOrDefaultAsync();
            if (client == null)
            {
                return false;
            }
            client.Name = updateClientDto.Name;
            client.Phone = updateClientDto.Phone;
            client.Email = updateClientDto.Email;
            client.Identification = updateClientDto.Identification;
            client.Lastname = updateClientDto.Lastname;
            client.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // Eliminar un cliente (temporal) (modificado)
        public async Task<bool> DeleteClientOrganizationAsync(ulong clientId, ulong organizationId)
        {
            // Buscar la relación en OrganizationClients (Organizacion y Cliente)
            var organizationClient = await _context.OrganizationClients
                .FirstOrDefaultAsync(oc => oc.ClientId == clientId && oc.OrganizationId == organizationId);

            // No hay relación entre el cliente y la organizacion
            if (organizationClient == null)
            {
                return false; 
            }

            organizationClient.DeletedAt = DateTime.UtcNow; // Se marca la fecha de eliminacion
            await _context.SaveChangesAsync();
           
            return true;
        }

        public async Task<bool> RestoreClientOrganizationAsync(ulong clientId, ulong organizationId)
        {
            // Buscar la relación en OrganizationClients (Organización y Cliente)
            var organizationClient = await _context.OrganizationClients
                .FirstOrDefaultAsync(oc => oc.ClientId == clientId && oc.OrganizationId == organizationId && oc.DeletedAt != null);

            // Si no se encuentra o no esta eliminado, no se puede restaurar
            if (organizationClient == null)
            {
                return false;
            }

            organizationClient.DeletedAt = null; // Se revierte la eliminacion logica
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ClientDTO?> GetClientByIdentificationAsync(string identification)
        {
            return await _context.Clients
                .Where(o => o.Identification == identification && o.DeletedAt == null)
                .Select(o => new ClientDTO
                {
                    Id = o.Id,
                    Identification = o.Identification,
                    Name = o.Name,
                    Lastname = o.Lastname,
                    Email = o.Email,
                    Phone = o.Phone,
                    Active = o.Active,
                })
                .FirstOrDefaultAsync();
        }

        //Metodo para relacionar cliente con organizacion
        public async Task<bool> AddClientToOrganizationAsync(ulong clientId, ulong organizationId)
        {
            var organizationClient = new OrganizationClient
            {
                ClientId = clientId,
                OrganizationId = organizationId
            };
            _context.OrganizationClients.Add(organizationClient);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
