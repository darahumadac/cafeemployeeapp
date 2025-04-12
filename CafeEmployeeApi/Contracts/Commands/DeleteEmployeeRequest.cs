using CafeEmployeeApi.Extensions;
using CafeEmployeeApi.Models;
using MediatR;

namespace CafeEmployeeApi.Contracts.Commands;

public record DeleteEmployeeRequest(string Id) : IRequest<bool>;
public class DeleteEmployeeRequestHandler : IRequestHandler<DeleteEmployeeRequest, bool>
{
    private readonly IDeleteService<string> _deleteService;

    public DeleteEmployeeRequestHandler(IDeleteService<string> deleteService)
    {
        _deleteService = deleteService;
    }

    public async Task<bool> Handle(DeleteEmployeeRequest request, CancellationToken cancellationToken)
    {
        return await _deleteService.DeleteAsync(request.Id);
    }
}