using Freelancers;
using Grpc.Core;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetFreelancerUserById;

namespace IdentityService.API.GrpcServices;

public class FreelancersGrpcService : Freelancers.Freelancers.FreelancersBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FreelancersGrpcService> _logger;

    public FreelancersGrpcService(
        IMediator mediator,
        ILogger<FreelancersGrpcService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Authorize]
    public override async Task<GetFreelancerByIdResponse> GetFreelancerById(GetFreelancerByIdRequest request, ServerCallContext context)
    {
        var appUser = await _mediator.Send(new GetFreelancerUserByIdQuery(Guid.Parse(request.Id)));
        
        _logger.LogInformation("Successfully returned freelancer data for {FreelancerId}", request.Id);

        return new GetFreelancerByIdResponse
        {
            Id = appUser.Id.ToString(), 
            StripeAccountId = appUser.StripeAccountId ?? string.Empty
        };
    }
}