namespace TutorialRestApi.Models
{
    public record Visit
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}