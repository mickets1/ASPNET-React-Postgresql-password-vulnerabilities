namespace WebAPI.Models
{
    public class Category
    {
        public int Category_id { get; set; }
        public string Category_name { get; set; } = "";
        public List<Password> Passwords { get; set; } = new List<Password>();
    }
}