using AuctionApp.Application.ApiResponses;

using FluentValidation.Results;

namespace AuctionApp.Application.Extensions;

public static class ErrorOrExtensions
{
    public static ApiResponse<T> ToSuccessfulApiResponse<T>(this ErrorOr<T> errorOr)
    {
        return new ApiResponse<T>(data: errorOr.Value, message: "Success", success: true);
    }

    /// <summary>
    /// Extension method to convert a ValidationResult object to a list of Error objects.
    /// </summary>
    /// <param name="validationResult">The ValidationResult object to convert.</param>
    /// <returns>A list of Error objects.</returns>
    public static List<Error> ToErrorList(this ValidationResult validationResult)
    {
        return validationResult.Errors.Select(x => Error.Validation(x.ErrorCode, x.ErrorMessage))
                               .ToList();
    }
}