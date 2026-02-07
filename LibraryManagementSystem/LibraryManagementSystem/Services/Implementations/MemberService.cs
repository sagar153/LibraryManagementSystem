namespace LibraryManagementSystem.Services.Implementations;

using Models;
using Interfaces;
using Microsoft.Extensions.Logging;
using LibraryManagementSystem.Repository.Interfaces;

/// <summary>
/// Implementation of member service using database transactions
/// </summary>
public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILogger<MemberService> _logger;

    public MemberService(
        IMemberRepository memberRepository,
        ILogger<MemberService> logger)
    {
        _memberRepository = memberRepository;
        _logger = logger;
    }

    public async Task<List<Member>> GetAllMembersAsync()
    {
        try
        {
            return await _memberRepository.GetAllMembersAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all members: {ex.Message}");
            return new List<Member>();
        }
    }

    public async Task<Member?> GetMemberByIdAsync(int id)
    {
        try
        {
            return await _memberRepository.GetMemberByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting member by ID {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Member?> GetMemberByEmailAsync(string email)
    {
        try
        {
            var members = await _memberRepository.GetAllMembersAsync();
            return members.FirstOrDefault(m => m.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting member by email {email}: {ex.Message}");
            return null;
        }
    }

    public async Task<Member?> GetMemberByMembershipIdAsync(string membershipId)
    {
        try
        {
            return await _memberRepository.GetMemberByMembershipIdAsync(membershipId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting member by membership ID {membershipId}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Member>> SearchMembersAsync(string searchTerm)
    {
        try
        {
            return await _memberRepository.SearchMembersAsync(searchTerm);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching members with term '{searchTerm}': {ex.Message}");
            return new List<Member>();
        }
    }

    public async Task<Member> CreateMemberAsync(Member member)
    {
        try
        {
            member.JoinDate = DateTime.UtcNow;
            member.MembershipExpiryDate = DateTime.UtcNow.AddYears(1);
            member.IsActive = true;

            var createdMember = await _memberRepository.CreateMemberAsync(member);
            _logger.LogInformation($"Member created: {member.FirstName} {member.LastName} (Membership ID: {createdMember.MembershipId})");

            return createdMember;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating member: {ex.Message}");
            throw;
        }
    }

    public async Task<Member?> UpdateMemberAsync(int id, Member member)
    {
        try
        {
            var updatedMember = await _memberRepository.UpdateMemberAsync(id, member);
            if (updatedMember != null)
            {
                _logger.LogInformation($"Member updated: {member.FirstName} {member.LastName} (ID: {id})");
            }
            return updatedMember;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating member {id}: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteMemberAsync(int id)
    {
        try
        {
            var result = await _memberRepository.DeleteMemberAsync(id);
            if (result)
            {
                _logger.LogInformation($"Member deleted (soft): ID {id}");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting member {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeactivateMemberAsync(int id)
    {
        try
        {
            var result = await _memberRepository.DeactivateMemberAsync(id);
            if (result)
            {
                _logger.LogInformation($"Member deactivated: ID {id}");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deactivating member {id}: {ex.Message}");
            return false;
        }
    }
}
