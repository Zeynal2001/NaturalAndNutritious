namespace NaturalAndNutritious.Business.Services.Results
{
    public class SupplierServiceResult
    {
        public bool IsNull { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }

        public SupplierServiceResult()
        {
            IsNull = false;
            Succeeded = true;
            Message = string.Empty;
        }
    }
}
