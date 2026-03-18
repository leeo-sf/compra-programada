using CompraProgramada.Domain.Exceptions.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OperationResult;

namespace CompraProgramada.Api.Controllers.Base;

[ApiController]
public class BaseController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    protected async Task<ActionResult<T>> SendCommand<T>(IRequest<Result<T>> request, int statusCode = 200)
    {
        var response = await _mediator.Send(request);
        return response.IsSuccess
            ? StatusCode(statusCode, response.Value)
            : HandleError(response.Exception!);
    }

    protected async Task<ActionResult> SendCommand(IRequest<Result> request, int statusCode = 200)
    {
        var response = await _mediator.Send(request);
        return response.IsSuccess ? StatusCode(statusCode) : HandleError(response.Exception!);
    }

    protected ActionResult HandleError(Exception error)
        => error switch
        {
            DomainException e => StatusCode((int)e.StatusCode, new { Mensagem = e.Message, e.Codigo }),
            ApplicationException applicationException => StatusCode(StatusCodes.Status422UnprocessableEntity, new { applicationException.Message }),
            _ => StatusCode(500)
        };
}