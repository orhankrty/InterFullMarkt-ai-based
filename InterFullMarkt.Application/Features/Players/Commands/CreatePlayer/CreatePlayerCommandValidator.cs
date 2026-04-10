namespace InterFullMarkt.Application.Features.Players.Commands.CreatePlayer;

using FluentValidation;
using InterFullMarkt.Domain.Enums;

/// <summary>
/// CreatePlayerCommand validatörü.
/// FluentValidation kullanarak DTO'daki verileri profesyonel kurallar ile doğrular.
/// </summary>
public sealed class CreatePlayerCommandValidator : AbstractValidator<CreatePlayerCommand>
{
    public CreatePlayerCommandValidator()
    {
        RuleFor(x => x.PlayerData)
            .NotNull()
            .WithMessage("Oyuncu verisi boş olamaz");

        RuleFor(x => x.PlayerData.FullName)
            .NotEmpty()
            .WithMessage("Oyuncu adı boş olamaz")
            .MinimumLength(3)
            .WithMessage("Oyuncu adı en az 3 karakter olmalıdır")
            .MaximumLength(150)
            .WithMessage("Oyuncu adı 150 karakteri geçemez")
            .Matches(@"^[a-zA-Z\s\-'àâäéèêëïîôùûüœæçñ]+$")
            .WithMessage("Oyuncu adı yalnızca harfler, boşluklar, tire ve kesme işaretleri içerebilir");

        RuleFor(x => x.PlayerData.Position)
            .Must(position => Enum.IsDefined(typeof(PlayerPosition), position))
            .WithMessage("Geçersiz pozisyon değeri (1=GK, 2=CB, 3=CM, 4=ST)");

        RuleFor(x => x.PlayerData.NationalityCode)
            .NotEmpty()
            .WithMessage("Milliyeti kodu boş olamaz")
            .Length(2)
            .WithMessage("Milliyeti kodu 2 karakter olmalıdır (ISO 3166-1)");

        RuleFor(x => x.PlayerData.DateOfBirth)
            .NotEmpty()
            .WithMessage("Doğum tarihi boş olamaz")
            .LessThan(DateTime.UtcNow.Date)
            .WithMessage("Doğum tarihi gelecekte olamaz")
            .GreaterThan(DateTime.UtcNow.AddYears(-50).Date)
            .WithMessage("Doğum tarihi 50 yıldan daha eski olamaz (ya da veri hatası var)");

        RuleFor(x => x.PlayerData.Height)
            .GreaterThanOrEqualTo(150)
            .WithMessage("Boy 150 cm'den küçük olamaz")
            .LessThanOrEqualTo(220)
            .WithMessage("Boy 220 cm'den büyük olamaz");

        RuleFor(x => x.PlayerData.Weight)
            .GreaterThan(40)
            .WithMessage("Kilo 40 kg'dan az olamaz")
            .LessThan(150)
            .WithMessage("Kilo 150 kg'dan fazla olamaz");

        RuleFor(x => x.PlayerData.PreferredFoot)
            .NotEmpty()
            .WithMessage("Tercih edilen ayak boş olamaz")
            .Must(foot => foot == "Left" || foot == "Right" || foot == "Ambidextrous")
            .WithMessage("Tercih edilen ayak 'Left', 'Right' veya 'Ambidextrous' olmalıdır");

        RuleFor(x => x.PlayerData.Currency)
            .NotEmpty()
            .WithMessage("Para birimi boş olamaz")
            .Length(3)
            .WithMessage("Para birimi kodu 3 karakter olmalıdır (örn: EUR, USD, GBP)");

        RuleFor(x => x.PlayerData.InitialMarketValue)
            .GreaterThan(0)
            .When(x => x.PlayerData.InitialMarketValue.HasValue)
            .WithMessage("Piyasa değeri negatif olamaz ve 0'dan büyük olmalıdır");

        RuleFor(x => x.PlayerData.JerseyNumber)
            .GreaterThanOrEqualTo(1)
            .When(x => x.PlayerData.JerseyNumber.HasValue)
            .WithMessage("Forma numarası 1'den küçük olamaz")
            .LessThanOrEqualTo(99)
            .When(x => x.PlayerData.JerseyNumber.HasValue)
            .WithMessage("Forma numarası 99'dan büyük olamaz");

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty()
            .WithMessage("Oluşturan kullanıcı ID'si boş olamaz");
    }
}
