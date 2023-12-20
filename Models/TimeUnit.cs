namespace WebAPI.Models
{
    public class TimeUnit
    {
        public int Time_unit_id { get; set; }
        public string Time_unit_name { get; set; } = "";
        public List<Password> Passwords { get; set; } = new List<Password>();
    }
}