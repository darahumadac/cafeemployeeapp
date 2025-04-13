using CafeEmployeeApi.Extensions;
using CafeEmployeeApi.Services;
using MediatR;

namespace CafeEmployeeApi.Contracts.Commands;

public record DeleteCafeRequest : GuidRequest, IRequest<Result<bool>>
{
    public DeleteCafeRequest(string Id) : base(Id)
    {
        this.Id = Id;
    }

    public string Id { get; private set; }
}

public class DeleteCafeRequestHandler : IRequestHandler<DeleteCafeRequest, Result<bool>>
{
    private readonly IDeleteService<Guid> _cafeDeleteService;

    public DeleteCafeRequestHandler(IDeleteService<Guid> deleteService)
    {
        _cafeDeleteService = deleteService;
    }

    public async Task<Result<bool>> Handle(DeleteCafeRequest request, CancellationToken cancellationToken)
    {
        request.Id.TryToGuid(out Guid cafeId);
        return Result<bool>.Success(await _cafeDeleteService.DeleteAsync(cafeId));
    }
}

