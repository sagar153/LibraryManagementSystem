namespace LibraryManagementSystem.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using Services.Interfaces;

/// <summary>
/// API controller for member management operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;
    private readonly ILogger<MembersController> _logger;

    public MembersController(IMemberService memberService, ILogger<MembersController> logger)
    {
        _memberService = memberService;
        _logger = logger;
    }

    /// <summary>
    /// Get all members
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllMembers()
    {
        _logger.LogInformation("Fetching all members");
        var members = await _memberService.GetAllMembersAsync();
        var memberDtos = members.Select(MapToDto).ToList();
        return Ok(memberDtos);
    }

    /// <summary>
    /// Get member by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberDto>> GetMemberById(int id)
    {
        _logger.LogInformation($"Fetching member with ID: {id}");
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
        {
            _logger.LogWarning($"Member with ID {id} not found");
            return NotFound();
        }
        return Ok(MapToDto(member));
    }

    /// <summary>
    /// Get member by membership ID
    /// </summary>
    [HttpGet("membership/{membershipId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberDto>> GetMemberByMembershipId(string membershipId)
    {
        _logger.LogInformation($"Fetching member with membership ID: {membershipId}");
        var member = await _memberService.GetMemberByMembershipIdAsync(membershipId);
        if (member == null)
        {
            _logger.LogWarning($"Member with membership ID {membershipId} not found");
            return NotFound();
        }
        return Ok(MapToDto(member));
    }

    /// <summary>
    /// Search members by name or email
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MemberDto>>> SearchMembers([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest("Search term is required");

        _logger.LogInformation($"Searching members with term: {searchTerm}");
        var members = await _memberService.SearchMembersAsync(searchTerm);
        var memberDtos = members.Select(MapToDto).ToList();
        return Ok(memberDtos);
    }

    /// <summary>
    /// Create a new member
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MemberDto>> CreateMember([FromBody] CreateUpdateMemberDto createMemberDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation($"Creating new member: {createMemberDto.FirstName} {createMemberDto.LastName}");
        
        var member = new Member
        {
            FirstName = createMemberDto.FirstName,
            LastName = createMemberDto.LastName,
            Email = createMemberDto.Email,
            PhoneNumber = createMemberDto.PhoneNumber,
            Address = createMemberDto.Address,
            City = createMemberDto.City,
            PostalCode = createMemberDto.PostalCode
        };

        var createdMember = await _memberService.CreateMemberAsync(member);
        return CreatedAtAction(nameof(GetMemberById), new { id = createdMember.Id }, MapToDto(createdMember));
    }

    /// <summary>
    /// Update an existing member
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MemberDto>> UpdateMember(int id, [FromBody] CreateUpdateMemberDto updateMemberDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation($"Updating member with ID: {id}");
        
        var member = new Member
        {
            FirstName = updateMemberDto.FirstName,
            LastName = updateMemberDto.LastName,
            Email = updateMemberDto.Email,
            PhoneNumber = updateMemberDto.PhoneNumber,
            Address = updateMemberDto.Address,
            City = updateMemberDto.City,
            PostalCode = updateMemberDto.PostalCode
        };

        var updatedMember = await _memberService.UpdateMemberAsync(id, member);
        if (updatedMember == null)
        {
            _logger.LogWarning($"Member with ID {id} not found for update");
            return NotFound();
        }
        return Ok(MapToDto(updatedMember));
    }

    /// <summary>
    /// Deactivate a member
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeactivateMember(int id)
    {
        _logger.LogInformation($"Deactivating member with ID: {id}");
        var result = await _memberService.DeactivateMemberAsync(id);
        if (!result)
        {
            _logger.LogWarning($"Member with ID {id} not found for deactivation");
            return NotFound();
        }
        return Ok(new { message = "Member deactivated successfully" });
    }

    /// <summary>
    /// Delete a member
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteMember(int id)
    {
        _logger.LogInformation($"Deleting member with ID: {id}");
        var result = await _memberService.DeleteMemberAsync(id);
        if (!result)
        {
            _logger.LogWarning($"Member with ID {id} not found for deletion");
            return NotFound();
        }
        return NoContent();
    }

    private static MemberDto MapToDto(Member member)
    {
        return new MemberDto
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Email = member.Email,
            PhoneNumber = member.PhoneNumber,
            MembershipId = member.MembershipId,
            JoinDate = member.JoinDate,
            MembershipExpiryDate = member.MembershipExpiryDate,
            Address = member.Address,
            City = member.City,
            PostalCode = member.PostalCode,
            IsActive = member.IsActive,
            CreatedAt = member.CreatedAt,
            UpdatedAt = member.UpdatedAt
        };
    }
}
