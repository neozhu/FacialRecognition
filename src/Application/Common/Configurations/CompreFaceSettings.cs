namespace CleanArchitecture.Blazor.Application.Common.Configurations;

/// <summary>
///     Configuration wrapper for the privacy section
/// </summary>
public class CompreFaceSettings
{
    /// <summary>
    ///     CompreFaceSettings key constraint
    /// </summary>
    public const string Key = nameof(CompreFaceSettings);
    

    public string Domain { get; set; } = "https://compreface.blazorserver.com";
    public string Port { get; set; } = "443";
    public string RecKey { get; set; } = "5104fee9-b273-4147-a009-798b114d2942";
    public string DetKey { get; set; } = "89014bd5-32e2-470d-bb5f-3078e19a3a40";
}