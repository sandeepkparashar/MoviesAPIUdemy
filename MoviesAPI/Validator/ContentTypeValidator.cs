using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Validator
{
    public class ContentTypeValidator:ValidationAttribute
    {
        private readonly string[] validContentTypes;
        private readonly string[] imageContentTypes = ["image/jpeg", "image/gif", "image/png"];
        public ContentTypeValidator(ContentType contentType) {
            
            switch (contentType)
            {
                case ContentType.Image:
                    validContentTypes = imageContentTypes;
                    break;
            }
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var file = value as IFormFile;
            if (file == null) return ValidationResult.Success;            

            if(!validContentTypes.Contains(file.ContentType))
            {
                return new ValidationResult($"File should be one of following image types:{string.Join(',',imageContentTypes)}.");
            }
            return ValidationResult.Success;
        }
    }

    public enum ContentType
    {
        Image
    }
}
