using IdentityService.API.Contracts.CommonContracts;
using IdentityService.BLL.DTOs;
using IdentityService.BLL.UseCases.CvUseCases.Commands.CreateCv;
using IdentityService.BLL.UseCases.CvUseCases.Commands.DeleteCv;
using IdentityService.BLL.UseCases.CvUseCases.Commands.UpdateCv;
using IdentityService.BLL.UseCases.CvUseCases.Queries.GetAllCvs;
using IdentityService.BLL.UseCases.CvUseCases.Queries.GetCvById;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/cvs")]
public class CvsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CvsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.FreelancerPolicy)]
    public async Task<IActionResult> Create([FromBody] CvDto cvDto)
    {
        await _mediator.Send(new CreateCvCommand(cvDto), HttpContext.RequestAborted);

        return Created();
    }

    [HttpGet]
    [Route("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var cv = await _mediator.Send(new GetCvByIdQuery(id), cancellationToken);

        return Ok(cv);
    }

    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPaginatedListRequest request,
        CancellationToken cancellationToken = default)
    {
        var cvs = await _mediator.Send(
            new GetAllCvsQuery(request.PageNo, request.PageSize),
            cancellationToken);

        return Ok(cvs);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllPublicCvsByUserId(
        [FromQuery] GetPaginatedListRequest request,
        CancellationToken cancellationToken = default)
    {
        var cvs = await _mediator.Send(
            new GetAllCvsQuery(request.PageNo, request.PageSize),
            cancellationToken);

        return Ok(cvs);
    }

    [HttpPut]
    [Route("{id:guid}")]
    [Authorize(Policy = AuthPolicies.FreelancerOrAdminPolicy)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CvDto cvDto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateCvCommand(
            id,
            cvDto.Title,
            cvDto.UserSpecialization,
            cvDto.UserEducation,
            cvDto.IsPublic,
            cvDto.CvLanguages,
            cvDto.CvSkills,
            cvDto.CvWorkExperiences), cancellationToken);

        return NoContent();
    }

    [HttpDelete]
    [Route("{id:guid}")]
    [Authorize(Policy = AuthPolicies.FreelancerOrAdminPolicy)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCvCommand(id), cancellationToken);

        return NoContent();
    }
}