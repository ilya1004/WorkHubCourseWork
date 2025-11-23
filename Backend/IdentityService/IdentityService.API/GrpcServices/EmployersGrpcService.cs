using Employers;
using Grpc.Core;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetEmployerUserById;

namespace IdentityService.API.GrpcServices;

public class EmployersGrpcService : Employers.Employers.EmployersBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EmployersGrpcService> _logger;

    public EmployersGrpcService(IMediator mediator,
        ILogger<EmployersGrpcService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Authorize]
    public override async Task<GetEmployerByIdResponse> GetEmployerById(GetEmployerByIdRequest request, ServerCallContext context)
    {
        var appUser = await _mediator.Send(new GetEmployerUserByIdQuery(Guid.Parse(request.Id)));
        
        _logger.LogInformation("Successfully returned employer data for {EmployerId}", request.Id);
        
        return new GetEmployerByIdResponse
        {
            Id = appUser.Id.ToString(), 
            EmployerCustomerId = appUser.StripeCustomerId ?? string.Empty,
        };
    }
}