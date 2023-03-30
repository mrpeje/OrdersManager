using FluentValidation.Results;

namespace OrdersManager.Business_layer
{
    public class SaveResult
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public List<ValidationResult> ValidationsErrors { get; set; } 
    }
}
