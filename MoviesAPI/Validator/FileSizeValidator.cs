using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Validator
{
    public class FileSizeValidator:ValidationAttribute
    {
        private readonly int maxFileSizeInMB;

        public FileSizeValidator(int MaxFileSizeInMB) {
            maxFileSizeInMB = MaxFileSizeInMB;
        }
        protected override ValidationResult IsValid(object? value,ValidationContext validationContext)
        {
            if(value == null) return ValidationResult.Success;
            var file = value as IFormFile;
            if(file == null) return ValidationResult.Success;
            if (file.Length > maxFileSizeInMB * 1024 * 1024)
            {
                return new ValidationResult($"File size cannot exceed {maxFileSizeInMB} MB.");
            }
            return ValidationResult.Success;
        }
    }
}
