﻿namespace NaturalAndNutritious.Data.Results
{
    public class RepositoryResult
    {
        public bool IsNull { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> CustomErrrors { get; set; }


        public RepositoryResult()
        {
            IsNull = false;
            Success = false;
            Message = "";//string.Empty;
            CustomErrrors = new List<string>();
        }
    }
}
