using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;
using TopUp.API.Exceptions;

namespace TopUp.API.Infrastructure
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Func<Exception, ProblemDetails>> _exceptionHandlers;

        public ApiExceptionFilter()
        {
            
            _exceptionHandlers = new Dictionary<Type, Func<Exception, ProblemDetails>>
            {
                { typeof(BeneficiaryAlreadyExistsException), e => HandleException(e, "A beneficiary already exists.", StatusCodes.Status400BadRequest)},
                { typeof(BeneficiaryNotFoundException), e => HandleException(e, "A beneficiary was not found.", StatusCodes.Status404NotFound) },
                { typeof(BeneficiaryLimitExceededException), e => HandleException(e, "Beneficiary limit exceeded.", StatusCodes.Status400BadRequest) },
                { typeof(InsufficientBalanceException), e => HandleException(e, "Insufficient balance.", StatusCodes.Status400BadRequest) },
                { typeof(UserNotFoundException), e => HandleException(e, "A user was not found.", StatusCodes.Status404NotFound) },
                { typeof(UserBalanceNotFoundException), e => HandleException(e, "A user balance was not found.", StatusCodes.Status404NotFound) },
                { typeof(MonthlyLimitExceededException), e => HandleException(e, "Monthly limit exceeded.", StatusCodes.Status400BadRequest) },
                { typeof(InvalidAmountException), e => HandleException(e, "Invalid amount.", StatusCodes.Status400BadRequest) },
                { typeof(TopUpOptionNotFoundException), e => HandleException(e, "Top up option not found.", StatusCodes.Status400BadRequest) }
               
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);

            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                var details = _exceptionHandlers[type].Invoke(context.Exception);
                context.Result = details.Status == StatusCodes.Status404NotFound ? new NotFoundObjectResult(details) : new BadRequestObjectResult(details);
                context.ExceptionHandled = true;
                return;
            }

            if (!context.ModelState.IsValid)
            {
                HandleInvalidModelStateException(context);
                return;
            }

            HandleUnknownException(context);
        }

        private ProblemDetails HandleException(Exception exception, string title, int status)
        {
            return new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = exception.Message
            };
        }

        private void HandleInvalidModelStateException(ExceptionContext context)
        {
            var details = new ValidationProblemDetails(context.ModelState);

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request."
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }
    }
}

