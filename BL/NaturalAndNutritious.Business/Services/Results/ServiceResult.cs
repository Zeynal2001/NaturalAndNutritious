namespace NaturalAndNutritious.Business.Services.Results
{
    public class ServiceResult
    {
        public bool IsNull { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> CustomErrors { get; set; }


        public ServiceResult()
        {
            IsNull = false;
            Succeeded = true;
            Message = string.Empty;
            CustomErrors = new List<string>();
        }
    }
}
