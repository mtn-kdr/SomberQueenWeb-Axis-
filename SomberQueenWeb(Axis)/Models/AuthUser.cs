namespace SomberQueenWeb_Axis_.Models
{
    public class AuthUser
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public DateTime created_at { get; set; }
    }
} 