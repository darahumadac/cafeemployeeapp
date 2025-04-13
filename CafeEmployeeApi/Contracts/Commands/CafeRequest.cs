using CafeEmployeeApi.Models;
using CafeEmployeeApi.Services;
using FluentValidation;
using MediatR;

namespace CafeEmployeeApi.Contracts.Commands;

public record CafeRequest(
    string Name,
    string Description,
    string Location,
    byte[]? Logo = null) : IRequest<Result<CreateCafeResponse>>;

public class CafeRequestValidator : AbstractValidator<CafeRequest>
{
    public CafeRequestValidator()
    {
        RuleFor(r => r.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(6, 10);

        RuleFor(r => r.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(256);

        //TODO: Add logo validation

        RuleFor(r => r.Location).NotEmpty();
    }
}

public class CreateCafeRequestHandler : IRequestHandler<CafeRequest, Result<CreateCafeResponse>>
{
    private readonly IAddService<Cafe, CreateCafeResponse> _addService;
    public CreateCafeRequestHandler(IAddService<Cafe, CreateCafeResponse> addService)
    {
        _addService = addService;
    }
    public async Task<Result<CreateCafeResponse>> Handle(CafeRequest request, CancellationToken cancellationToken)
    {
        _addService.CreateEntity = () =>
            new Cafe
            {
                Name = request.Name,
                Description = request.Description,
                Location = request.Location,
                Logo = request.Logo
            };


        _addService.CreateResponse = (Cafe newCafe) =>
            new CreateCafeResponse
            (
                Id: newCafe.Id,
                Name: newCafe.Name,
                Description: newCafe.Description,
                Location: newCafe.Location,
                ETag: Convert.ToBase64String(newCafe.ETag),
                Logo: newCafe.Logo
            );


        return await _addService.AddAsync();
    }
}