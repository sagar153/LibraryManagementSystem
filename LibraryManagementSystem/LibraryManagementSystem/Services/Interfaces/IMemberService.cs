namespace LibraryManagementSystem.Services.Interfaces;

using Models;

/// <summary>
/// Interface for member service operations
/// </summary>
public interface IMemberService
{
    Task<List<Member>> GetAllMembersAsync();
    
    Task<Member?> GetMemberByIdAsync(int id);
    
    Task<Member?> GetMemberByEmailAsync(string email);
    
    Task<Member?> GetMemberByMembershipIdAsync(string membershipId);
    
    Task<List<Member>> SearchMembersAsync(string searchTerm);
    
    Task<Member> CreateMemberAsync(Member member);
    
    Task<Member?> UpdateMemberAsync(int id, Member member);
    
    Task<bool> DeleteMemberAsync(int id);
    
    Task<bool> DeactivateMemberAsync(int id);
}
