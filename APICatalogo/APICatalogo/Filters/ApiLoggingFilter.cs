using Microsoft.AspNetCore.Mvc.Filters;

namespace APICatalogo.Filters;

public class ApiLoggingFilter : IActionFilter
{
    private readonly ILogger<ApiLoggingFilter> _logger;

    public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
        =>  _logger = logger;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        //executa antes da Action(Método do controller)
        _logger.LogInformation($"- Executando -> {nameof(OnActionExecuting)}");
        _logger.LogInformation($"-------------------------------------------");
        _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
        _logger.LogInformation($"ModelState : {context.ModelState.IsValid}");
        _logger.LogInformation($"Details In : {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");
        _logger.LogInformation($"-------------------------------------------");
    }
    public void OnActionExecuted(ActionExecutedContext context)
    {
        //executa depois da Action(Método do controller)
        _logger.LogInformation($"- Executando -> {nameof(OnActionExecuted)}");
        _logger.LogInformation($"-------------------------------------------");
        _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
        _logger.LogInformation($"Details Out: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");
        _logger.LogInformation($"Status Code : {context.HttpContext.Response.StatusCode}");
        _logger.LogInformation($"-------------------------------------------");
    }
}
