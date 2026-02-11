namespace MindfulMomentsApp.Models
{
    public class QuoteModel
    {
        public int id { get; set; }
        public string text { get; set; }
        public string lang { get; set; }
        public int category_id { get; set; }
        public int author_id { get; set; }
    }
}
