namespace LibraryManagementSystem.Repository;

using Data;
using Models;
using Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Member repository implementation using Entity Framework Core
/// </summary>
public class MemberRepository : IMemberRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MemberRepository> _logger;

    public MemberRepository(ApplicationDbContext context, ILogger<MemberRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all active members
    /// </summary>
    public async Task<List<Member>> GetAllMembersAsync()
    {
        try
        {
            return await _context.Members
                .AsNoTracking()
                .Where(m => m.IsActive)
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.LastName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all members: {ex.Message}");
            return new List<Member>();
        }
    }

    /// <summary>
    /// Get member by ID
    /// </summary>
    public async Task<Member?> GetMemberByIdAsync(int id)
    {
        try
        {
            return await _context.Members
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting member by ID {id}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get member by membership ID
    /// </summary>
    public async Task<Member?> GetMemberByMembershipIdAsync(string membershipId)
    {
        try
        {
            return await _context.Members
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MembershipId == membershipId && m.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting member by membership ID {membershipId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Search members by name or email
    /// </summary>
    public async Task<List<Member>> SearchMembersAsync(string searchTerm)
    {
        try
        {
            var lowerSearchTerm = searchTerm.ToLower();
            return await _context.Members
                .AsNoTracking()
                .Where(m => m.IsActive && (
                    m.FirstName.ToLower().Contains(lowerSearchTerm) ||
                    m.LastName.ToLower().Contains(lowerSearchTerm) ||
                    m.Email.ToLower().Contains(lowerSearchTerm)
                ))
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.LastName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching members with term '{searchTerm}': {ex.Message}");
            return new List<Member>();
        }
    }

    /// <summary>
    /// Create new member
    /// </summary>
    public async Task<Member> CreateMemberAsync(Member member)
    {
        try
        {
            member.CreatedAt = DateTime.UtcNow;
            member.JoinDate = DateTime.UtcNow;
            member.MembershipExpiryDate = DateTime.UtcNow.AddYears(1);
            member.IsActive = true;
            
            // Generate membership ID if not provided
            if (string.IsNullOrEmpty(member.MembershipId))
            {
                member.MembershipId = await GenerateNewMembershipIdAsync();
            }
            
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Member created: {member.FirstName} {member.LastName} (ID: {member.MembershipId})");
            return member;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating member: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Update existing member
    /// </summary>
    public async Task<Member?> UpdateMemberAsync(int id, Member member)
    {
        try
        {
            var existingMember = await _context.Members.FirstOrDefaultAsync(m => m.Id == id);
            if (existingMember == null)
            {
                _logger.LogWarning($"Member with ID {id} not found");
                return null;
            }

            existingMember.FirstName = member.FirstName;
            existingMember.LastName = member.LastName;
            existingMember.Email = member.Email;
            existingMember.PhoneNumber = member.PhoneNumber;
            existingMember.Address = member.Address;
            existingMember.City = member.City;
            existingMember.PostalCode = member.PostalCode;
            existingMember.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Member updated: {existingMember.FirstName} {existingMember.LastName} (ID: {id})");
            return existingMember;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating member {id}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Deactivate member
    /// </summary>
    public async Task<bool> DeactivateMemberAsync(int id)
    {
        try
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                _logger.LogWarning($"Member with ID {id} not found");
                return false;
            }

            member.IsActive = false;
            member.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Member deactivated: {member.FirstName} {member.LastName} (ID: {id})");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deactivating member {id}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Delete member (soft delete)
    /// </summary>
    public async Task<bool> DeleteMemberAsync(int id)
    {
        try
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                _logger.LogWarning($"Member with ID {id} not found");
                return false;
            }

            member.IsActive = false;
            member.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Member deleted (soft): {member.FirstName} {member.LastName} (ID: {id})");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting member {id}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Check if email exists
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email)
    {
        try
        {
            return await _context.Members.AnyAsync(m => m.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error checking email existence: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Get expired memberships
    /// </summary>
    public async Task<List<Member>> GetExpiredMembershipsAsync()
    {
        try
        {
            return await _context.Members
                .AsNoTracking()
                .Where(m => m.IsActive && m.MembershipExpiryDate < DateTime.UtcNow)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting expired memberships: {ex.Message}");
            return new List<Member>();
        }
    }

    /// <summary>
    /// Generate new membership ID
    /// </summary>
    public async Task<string> GenerateNewMembershipIdAsync()
    {
        try
        {
            var lastMember = await _context.Members
                .AsNoTracking()
                .OrderByDescending(m => m.Id)
                .FirstOrDefaultAsync();

            int nextNumber = (lastMember?.Id ?? 1000) + 1;
            return $"LIB{nextNumber}";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating membership ID: {ex.Message}");
            return $"LIB{DateTime.UtcNow.Ticks}";
        }
    }
}
