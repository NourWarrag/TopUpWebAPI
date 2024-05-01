using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;
using TopUp.API.Exceptions;

namespace TopUp.API.Infrastructure
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        public ApiExceptionFilter()
        {
            // Register known exception types and handlers.
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(BeneficiaryAlreadyExistsException), HandleBeneficiaryAlreadyExistsException},
                { typeof(BeneficiaryNotFoundException), HandleBeneficiaryNotFoundException },
                { typeof(BeneficiaryLimitExceededException), HandleBeneficiaryLimitExceededException },
                { typeof(InsufficientBalanceException), HandleInsufficientBalanceException },
                { typeof(UserNotFoundException), HandleUserNotFoundException },
                { typeof(UserBalanceNotFoundException), HandleUserBalanceNotFoundException },
                { typeof(MonthlyLimitExceededException), HandleMonthlyLimitExceededException },


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
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            if (!context.ModelState.IsValid)
            {
                HandleInvalidModelStateException(context);
                return;
            }

            HandleUnknownException(context);
        }

        private void HandleUserNotFoundException(ExceptionContext context)
        {
            var exception = context.Exception as UserNotFoundException;

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "A user was not found.",
                Detail = exception.Message
            };

            context.Result = new NotFoundObjectResult(details);

            context.ExceptionHandled = true;
        }
        private void HandleInsufficientBalanceException(ExceptionContext context)
        {
            var exception = context.Exception as InsufficientBalanceException;

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Insufficient balance.",
                Detail = exception.Message
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }
        private void HandleUserBalanceNotFoundException(ExceptionContext context)
        {
            var exception = context.Exception as UserBalanceNotFoundException;

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "A user balance was not found.",
                Detail = exception.Message
            };

            context.Result = new NotFoundObjectResult(details);

            context.ExceptionHandled = true;
        }
        private void HandleMonthlyLimitExceededException(ExceptionContext context)
        {
            var exception = context.Exception as MonthlyLimitExceededException;

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Monthly limit exceeded.",
                Detail = exception.Message
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }
        private void HandleBeneficiaryLimitExceededException(ExceptionContext context)
        {
            var exception = context.Exception as BeneficiaryLimitExceededException;

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Beneficiary limit exceeded.",
                Detail = exception.Message
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }
        private void HandleBeneficiaryAlreadyExistsException(ExceptionContext context)
        {
            var exception = context.Exception as BeneficiaryAlreadyExistsException;

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "A beneficiary already exists.",
                Detail = exception.Message
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }
        private void HandleBeneficiaryNotFoundException(ExceptionContext context)
        {
            var exception = context.Exception as BeneficiaryNotFoundException;

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "A beneficiary was not found.",
                Detail = exception.Message
            };

            context.Result = new NotFoundObjectResult(details);

            context.ExceptionHandled = true;
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
