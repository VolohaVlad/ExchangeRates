using ExchangeRates.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRates.Server.Filters
{
    public sealed class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public ApiExceptionFilter(ILogger logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.Error(context.Exception.Message, "ApiExceptionFilter");
            SetResult(context);
            base.OnException(context);
        }

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.Error(context.Exception.Message, "ApiExceptionFilter");
            SetResult(context);
            return base.OnExceptionAsync(context);
        }

        private static void SetResult(ExceptionContext context)
        {
            ICollection<ErrorModel> errors;
            switch (context.Exception)
            {
                case ValidationException ex:
                    var mes = ex.ValidationResult.ErrorMessage;
                    errors = ex.ValidationResult.MemberNames.Select(x => new ErrorModel(x, mes)).ToList();
                    break;
                default:
                    errors = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => new ErrorModel(e.ErrorMessage)).ToList();
                    break;
            }

            if (!errors.Any())
            {
                errors = new[] { new ErrorModel(context.Exception.Message) };
            }

            context.Result = new BadRequestObjectResult(errors);
        }
    }
}
