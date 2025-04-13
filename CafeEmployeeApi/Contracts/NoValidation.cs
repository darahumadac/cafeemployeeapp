using CafeEmployeeApi.Contracts.Commands;
using FluentValidation;

namespace CafeEmployeeApi.Contracts;

public class NoValidation<TRequest> : AbstractValidator<TRequest>
{
    public NoValidation(){}
}
