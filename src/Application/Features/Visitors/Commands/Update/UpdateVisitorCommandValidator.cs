// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Update;

public class UpdateVisitorCommandValidator : AbstractValidator<UpdateVisitorCommand>
{
        public UpdateVisitorCommandValidator()
        {
        RuleFor(v => v.Name).MaximumLength(50).NotEmpty();
        RuleFor(v => v.Interviewee).MaximumLength(50).NotEmpty();
        RuleFor(v => v.PurposeOfVisit).NotEmpty();
        RuleFor(v => v.DateOfVisit).NotEmpty();
        RuleFor(v => v.Photos).NotEmpty();
    }
     public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
     {
        var result = await ValidateAsync(ValidationContext<UpdateVisitorCommand>.CreateWithOptions((UpdateVisitorCommand)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
     };
}

