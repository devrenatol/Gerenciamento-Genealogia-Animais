using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ProjetoLO.Infra.IoC.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Ocorreu uma excessao nao tratada : Status Code 500");

        context.Result = new ObjectResult("Ocorreu um problema ao tratar sua solicitacao : Status Code 500")
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}
