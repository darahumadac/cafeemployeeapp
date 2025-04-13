using CafeEmployeeApi.Database;
using CafeEmployeeApi.Extensions;
using CafeEmployeeApi.Models;
using CafeEmployeeApi.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Contracts.Commands;

public record CafeRequest(
    string Name,
    string Description,
    string Location,
    byte[]? Logo = null);

public record CreateCafeRequest : CafeRequest, IRequest<Result<CreateCafeResponse>>
{
    public CreateCafeRequest(
        string Name,
        string Description,
        string Location,
        byte[]? Logo = null
    ) : base(Name, Description, Location, Logo){}
}

public record UpdateCafeRequest : CafeRequest, IRequest<Result<Cafe>>
{
    public UpdateCafeRequest(
        string Id,
        string Name,
        string Description,
        string Location,
        string ETag,
        byte[]? Logo = null
    ) : base(Name, Description, Location, Logo){
        this.Id = Id;
        this.ETag = ETag;
    }

    public string Id { get; private set; }
    public string ETag { get; private set; }
}

public class UpdateCafeIdValidator : AbstractValidator<UpdateCafeRequest>
{
    public UpdateCafeIdValidator()
    {
        When(r => !string.IsNullOrEmpty(r.Id), () => 
        {
            RuleFor(r => r.Id)
            .Must(cafeId => Guid.TryParse(cafeId, out Guid parsedCafeId))
            .WithMessage("Invalid Cafe ID format");
        });
    }
}

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

public class CreateCafeRequestHandler : IRequestHandler<CreateCafeRequest, Result<CreateCafeResponse>>
{
    private readonly IAddService<Cafe, CreateCafeResponse> _addService;
    public CreateCafeRequestHandler(IAddService<Cafe, CreateCafeResponse> addService)
    {
        _addService = addService;
    }
    public async Task<Result<CreateCafeResponse>> Handle(CreateCafeRequest request, CancellationToken cancellationToken)
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


public class UpdateCafeRequestHandler : IRequestHandler<UpdateCafeRequest, Result<Cafe>>
{
    private readonly AppDbContext _dbContext;

    public UpdateCafeRequestHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Result<Cafe>> Handle(UpdateCafeRequest request, CancellationToken cancellationToken)
    {
        request.Id.TryToGuid(out Guid cafeId);

        var cafe = await _dbContext.Cafes.FindAsync(cafeId);
        if (cafe == null)
        {
            return Result<Cafe>.Failure(StatusCodes.Status404NotFound.ToString());
        }

        if (Convert.ToBase64String(cafe.ETag) != request.ETag)
        {
            return Result<Cafe>.Failure(StatusCodes.Status412PreconditionFailed.ToString());
        }

        cafe.Name = request.Name;
        cafe.Description = request.Description;
        cafe.Location = request.Location;
        cafe.Logo = request.Logo;
        cafe.UpdatedDate = DateTime.UtcNow;

        try
        {
            await _dbContext.SaveChangesAsync();
            return Result<Cafe>.Success(cafe);
        }
        catch (DbUpdateException ex)
        {
            return Result<Cafe>.Failure(ex.Message);
        }

    }
}