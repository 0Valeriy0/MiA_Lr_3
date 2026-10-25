
using FluentValidation;
using MiA_Lr_3.DTOs.Cars;
using MiA_Lr_3.Models;

namespace MiA_Lr_3.Validators
{
    public class CreateCarDtoValidator : AbstractValidator<CreateCarDto>
    {
        public CreateCarDtoValidator()
        {
            RuleFor(car => car.Brand)
                .NotEmpty()
                .WithMessage("Вкажіть марку автомобіля")
                .MaximumLength(50)
                .WithMessage("Марка автомобіля має містити максимум 50 символів");

            RuleFor(car => car.Model)
                .NotEmpty()
                .WithMessage("Вкажіть модель автомобіля")
                .MaximumLength(50)
                .WithMessage("Модель автомобіля має містити максимум 50 символів");

            RuleFor(car => car.RegistrationNumber)
                .NotEmpty()
                .WithMessage("Вкажіть реєстраційний номер автомобіля")
                .Matches(@"^[A-Z]{2}[0-9]{4}[A-Z]{2}$")
                .WithMessage("Реєстраційний номер має відповідати формату AA9999AA");

            RuleFor(car => car.Class)
                .IsInEnum()
                .WithMessage("Вкажіть коректний клас автомобіля");

            RuleFor(car => car.DailyRate)
                .GreaterThan(0)
                .WithMessage("Денна ставка має бути більшою за 0");

            RuleFor(car => car.CurrentMileage)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Пробіг не може бути від'ємним");
        }
    }
        public class UpdateCarDtoValidator : AbstractValidator<UpdateCarDto>
        {
            public UpdateCarDtoValidator()
            {
                RuleFor(car => car.RegistrationNumber)
                    .NotEmpty()
                    .WithMessage("Реєстраційний номер автомобіля є обов'язковим")
                    .Matches(@"^[A-Z]{2}[0-9]{4}[A-Z]{2}$")
                    .WithMessage("Номер авто повинен відповідати формату AA9999AA");
                RuleFor(car => car.DailyRate)
                    .GreaterThan(0)
                    .WithMessage("Добова ставка повинна бути більшою за 0");
                RuleFor(car => car.CurrentMileage)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Пробіг не може бути від'ємним");
            }
        }
}

