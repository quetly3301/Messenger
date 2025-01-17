using FluentValidation;

namespace Messenger.Domain.Entities.Validation;

public class SessionEntityValidator : AbstractValidator<SessionEntity>
{
    public SessionEntityValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AccessToken).NotEmpty();
        RuleFor(x => x.RefreshToken).NotNull();
        RuleFor(x => x.Ip).NotNull();
        RuleFor(x => x.UserAgent).NotNull();
        RuleFor(x => x.ExpiresAt).NotNull();
        RuleFor(x => x.CreateAt).NotNull();
    }
}