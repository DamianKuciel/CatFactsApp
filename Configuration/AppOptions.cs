using System.ComponentModel.DataAnnotations;

namespace CatFactsApp.Configuration
{
    public sealed class AppOptions
    {
        [Required(ErrorMessage = "ApiUrl is required.")]
        public string ApiUrl { get; init; } = string.Empty;

        [Required(ErrorMessage = "OutputFilePath is required.")]
        public string OutputFilePath { get; init; } = string.Empty;
    }
}