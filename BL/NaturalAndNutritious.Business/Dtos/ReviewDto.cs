namespace NaturalAndNutritious.Business.Dtos
{
    public class ReviewDto
    {
        public Guid ProductId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string ReviewText { get; set; }
        public AppUserDto User { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
