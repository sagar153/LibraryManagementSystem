namespace LibraryManagementSystem.Repository.Interfaces;

using Models;

/// <summary>
/// Interface for member repository operations
/// </summary>
public interface IMemberRepository
{
    /// <summary>
    /// Get all active members
    /// </summary>
    Task<List<Member>> GetAllMembersAsync();
    
    /// <summary>
    /// Get member by ID
    /// </summary>
    Task<Member?> GetMemberByIdAsync(int id);
    
    /// <summary>
    /// Get member by membership ID
    /// </summary>
    Task<Member?> GetMemberByMembershipIdAsync(string membershipId);
    
    /// <summary>
    /// Search members by name or email
    /// </summary>
    Task<List<Member>> SearchMembersAsync(string searchTerm);
    
    /// <summary>
    /// Create new member
    /// </summary>
    Task<Member> CreateMemberAsync(Member member);
    
    /// <summary>
    /// Update existing member
    /// </summary>
    Task<Member?> UpdateMemberAsync(int id, Member member);
    
    /// <summary>
    /// Deactivate member
    /// </summary>
    Task<bool> DeactivateMemberAsync(int id);
    
    /// <summary>
    /// Delete member (soft delete)
    /// </summary>
    Task<bool> DeleteMemberAsync(int id);
    
    /// <summary>
    /// Check if email exists
    /// </summary>
    Task<bool> EmailExistsAsync(string email);
    
    /// <summary>
    /// Get expired memberships
    /// </summary>
    Task<List<Member>> GetExpiredMembershipsAsync();
    
    /// <summary>
    /// Generate new membership ID
    /// </summary>
    Task<string> GenerateNewMembershipIdAsync();
}
