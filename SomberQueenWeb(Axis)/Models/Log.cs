namespace SomberQueenWeb_Axis_.Models
{
    public class Log
    {
        public int id { get; set; }
        public Guid user_id { get; set; }
        public string action { get; set; }
        public DateTime created_at { get; set; }
    }
}
