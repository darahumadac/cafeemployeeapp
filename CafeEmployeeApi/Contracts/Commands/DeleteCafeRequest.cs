using CafeEmployeeApi.Extensions;
using CafeEmployeeApi.Services;
using MediatR;

namespace CafeEmployeeApi.Contracts.Commands;

public record DeleteCafeRequest(string Id) : IRequest<bool>;
public class DeleteCafeRequestHandler : IRequestHandler<DeleteCafeRequest, bool>
{
    private readonly IDeleteService<Guid> _cafeDeleteService;

    public DeleteCafeRequestHandler(IDeleteService<Guid> deleteService)
    {
        _cafeDeleteService = deleteService;
    }

    public async Task<bool> Handle(DeleteCafeRequest request, CancellationToken cancellationToken)
    {
        var validGuid = request.Id.TryToGuid(out Guid cafeId);
        if (!validGuid)
        {
            return false;
        }

        return await _cafeDeleteService.DeleteAsync(cafeId);
    }
}

