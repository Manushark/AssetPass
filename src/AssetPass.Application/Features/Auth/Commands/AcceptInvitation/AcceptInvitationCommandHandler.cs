using AssetPass.Application.Common;
using AssetPass.Application.Interfaces;
using AssetPass.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Features.Auth.Commands.AcceptInvitation;

public sealed class AcceptInvitationCommandHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identityService;
    private readonly IValidator<AcceptInvitationCommand> _validator;

    public AcceptInvitationCommandHandler(
        IApplicationDbContext db,
        IIdentityService identityService,
        IValidator<AcceptInvitationCommand> validator)
    {
        _db = db;
        _identityService = identityService;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(AcceptInvitationCommand command, CancellationToken ct = default)
    {
        // 1. Validation
        var valResult = await _validator.ValidateAsync(command, ct);
        if (!valResult.IsValid)
            return Result.Failure(valResult.Errors.Select(e => e.ErrorMessage).ToArray());

        // 2. Find invitation (using IgnoreQueryFilters since caller is unauthenticated)
        var invitation = await _db.UserInvitations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(i => i.Token == command.Token, ct);

        if (invitation is null || invitation.IsAccepted || invitation.IsExpired)
            return Result.Failure("La invitación no es válida o ha expirado.");

        // 3. Check if user with that email already exists in identity
        var existingIdentity = await _identityService.FindByEmailAsync(invitation.Email, ct);
        if (existingIdentity is not null)
            return Result.Failure("Ya existe un usuario registrado con este correo electrónico.");

        // 4. Create Domain ApplicationUser
        var firstName = !string.IsNullOrWhiteSpace(command.FirstName)
            ? command.FirstName.Trim()
            : (!string.IsNullOrWhiteSpace(invitation.FirstName) ? invitation.FirstName : string.Empty);

        var lastName = !string.IsNullOrWhiteSpace(command.LastName)
            ? command.LastName.Trim()
            : (!string.IsNullOrWhiteSpace(invitation.LastName) ? invitation.LastName : string.Empty);

        var appUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            TenantId = invitation.TenantId,
            Email = invitation.Email,
            FirstName = firstName,
            LastName = lastName,
            Role = invitation.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.ApplicationUsers.Add(appUser);

        // 5. Create ASP.NET Core Identity user
        var (identityUserId, identityErrors) = await _identityService.CreateUserAsync(
            email: invitation.Email,
            password: command.Password,
            tenantId: invitation.TenantId,
            applicationUserId: appUser.Id,
            role: invitation.Role.ToString(),
            ct: ct);

        if (identityUserId is null)
            return Result.Failure(identityErrors);

        appUser.IdentityUserId = identityUserId;

        // 6. Mark invitation as accepted
        invitation.IsAccepted = true;
        invitation.AcceptedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
