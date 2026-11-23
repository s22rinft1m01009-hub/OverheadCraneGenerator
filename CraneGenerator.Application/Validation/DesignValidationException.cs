namespace CraneGenerator.Application.Validation;

public sealed class DesignValidationException : Exception
{
    public DesignValidationException(IReadOnlyCollection<string> errors)
        : base("The design request is invalid.")
    {
        Errors = errors;
    }

    public IReadOnlyCollection<string> Errors { get; }
}
