using System.Security.Cryptography;
using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using AssetPass.Domain.Entities;
using AssetPass.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Users.Commands.InviteUser;

public sealed class InviteUserCommandHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly IValidator<InviteUserCommand> _validator;

    public InviteUserCommandHandler(
        IApplicationDbContext db,
        IIdentityService identityService,
        IEmailService emailService,
        IValidator<InviteUserCommand> validator)
    {
        _db = db;
        _identityService = identityService;
        _emailService = emailService;
        _validator = validator;
    }

    public async Task<Result<InviteUserResponse>> HandleAsync(
        InviteUserCommand command,
        Guid callerTenantId,
        Guid callerApplicationUserId,
        UserRole callerRole,
        CancellationToken ct = default)
    {
        // 1. Validation
        var valResult = await _validator.ValidateAsync(command, ct);
        if (!valResult.IsValid)
            return Result<InviteUserResponse>.Failure(valResult.Errors.Select(e => e.ErrorMessage).ToArray());

        // 2. Caller permission check
        if (callerRole != UserRole.Owner && callerRole != UserRole.Administrator)
            return Result<InviteUserResponse>.Forbidden("No tienes permiso para invitar usuarios.");

        if (callerRole == UserRole.Administrator && command.Role == UserRole.Owner)
            return Result<InviteUserResponse>.Forbidden("Un Administrador no puede invitar a un usuario como Owner.");

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        // 3. Check if user already exists in identity
        var existingUser = await _identityService.FindByEmailAsync(normalizedEmail, ct);
        if (existingUser is not null)
            return Result<InviteUserResponse>.Failure("Ya existe un usuario registrado con ese correo electrónico.");

        // 4. Check if active pending invitation exists
        var existingInvitation = await _db.UserInvitations
            .FirstOrDefaultAsync(i => i.TenantId == callerTenantId && i.Email == normalizedEmail && !i.IsAccepted && i.ExpiresAt > DateTime.UtcNow, ct);

        if (existingInvitation is not null)
            return Result<InviteUserResponse>.Failure("Ya existe una invitación pendiente y activa para este correo electrónico.");

        // 5. Generate secure random token
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var invitation = new UserInvitation
        {
            Id = Guid.NewGuid(),
            TenantId = callerTenantId,
            Email = normalizedEmail,
            FirstName = command.FirstName?.Trim(),
            LastName = command.LastName?.Trim(),
            Role = command.Role,
            Token = token,
            ExpiresAt = expiresAt,
            IsAccepted = false,
            InvitedByUserId = callerApplicationUserId,
            CreatedAt = DateTime.UtcNow
        };

        _db.UserInvitations.Add(invitation);
        await _db.SaveChangesAsync(ct);

        // 6. Send invitation email
        var inviter = await _db.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Id == callerApplicationUserId && u.TenantId == callerTenantId, ct);
        var tenant = await _db.Tenants
            .FirstOrDefaultAsync(t => t.Id == callerTenantId, ct);

        var inviterName = inviter != null ? $"{inviter.FirstName} {inviter.LastName}".Trim() : "Administrador";
        var companyName = tenant?.CommercialName ?? "AssetPass";
        var acceptLink = $"https://app.assetpass.com/auth/accept-invitation?token={token}";

        await _emailService.SendInvitationEmailAsync(normalizedEmail, inviterName, companyName, acceptLink, ct);

        return Result<InviteUserResponse>.Success(new InviteUserResponse
        {
            InvitationId = invitation.Id,
            Email = invitation.Email,
            Role = invitation.Role.ToString(),
            ExpiresAt = invitation.ExpiresAt
        });
    }
}
