namespace SomberQueenWeb_Axis_.Models
{
    public class EncryptedFile
    {
        public int id { get; set; }
        public Guid user_id { get; set; }
        public string file_name { get; set; }
        public DateTime created_at { get; set; }
        public string username { get; set; }

    }
}
