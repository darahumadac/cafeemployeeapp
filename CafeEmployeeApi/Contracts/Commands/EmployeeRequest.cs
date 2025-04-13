using System.Text.RegularExpressions;
using CafeEmployeeApi.Database;
using CafeEmployeeApi.Models;
using CafeEmployeeApi.Services;
using FluentValidation;
using MediatR;

namespace CafeEmployeeApi.Contracts.Commands;

public record EmployeeRequest(
    string Name, 
    string EmailAddress, 
    string PhoneNumber, 
    int Gender, 
    string? AssignedCafeId) : IRequest<Result<CreateEmployeeResponse>>;

public class EmployeeRequestValidator : AbstractValidator<EmployeeRequest>
{
    public EmployeeRequestValidator(AppDbContext dbContext)
    {
        RuleFor(r => r.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(6, 10);

        RuleFor(r => r.EmailAddress)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(r => r.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(number => Regex.IsMatch(number, @"^[8|9][0-9]{7}$"))
                .WithMessage("'Phone Number' is not a valid SG phone number");
        
        
        RuleFor(r => r.Gender)
            .InclusiveBetween(0,1);
            

        When(r => r.AssignedCafeId != null, () => {
            RuleFor(r => r.AssignedCafeId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("'AssignedCafeId' must not be empty. Remove 'AssignedCafeId' from the request to remove assigned cafe")
                .Must(cafeId => 
                    Guid.TryParse(cafeId, out Guid parsedCafeId) 
                    && dbContext.Cafes.Find(parsedCafeId) != null)
                    .WithMessage("'AssignedCafeId' must be an existing cafe id");
        });
    }
}

public class CreateEmployeeRequestHandler : IRequestHandler<EmployeeRequest, Result<CreateEmployeeResponse>>
{
    private readonly IAddService<Employee, CreateEmployeeResponse> _addService;
    public CreateEmployeeRequestHandler(IAddService<Employee, CreateEmployeeResponse> addService)
    {
        _addService = addService;
    }
    public async Task<Result<CreateEmployeeResponse>> Handle(EmployeeRequest request, CancellationToken cancellationToken)
    {
        _addService.CreateEntity = () =>
            new Employee
            {
                Name = request.Name,
                Email = request.EmailAddress,
                PhoneNumber = request.PhoneNumber,
                Gender = Convert.ToBoolean(request.Gender),
                CafeId = request.AssignedCafeId != null ? Guid.Parse(request.AssignedCafeId) : null,
                StartDate = request.AssignedCafeId != null ? DateTime.UtcNow : null
            };

        _addService.CreateResponse = (Employee newEmployee) =>
            new CreateEmployeeResponse(
                Id: newEmployee.Id,
                Name: newEmployee.Name,
                Email: newEmployee.Email,
                PhoneNumber: newEmployee.PhoneNumber,
                Gender: Convert.ToInt16(newEmployee.Gender),
                ETag: Convert.ToBase64String(newEmployee.ETag),
                CafeId: newEmployee.CafeId
            );


        return await _addService.AddAsync();
    }
}