using System.Text.Json.Serialization;

namespace WebAPI.Models 
    {
   public class Password
    {
        public int Password_id { get; set; }
        public string Password_value { get; set; } = "";
        public int Category_id { get; set; }

        // Avoid circular references
        [JsonIgnore]
        public Category Category { get; set; } = new Category(); 
        public float Value { get; set; }
        public float Offline_crack_sec { get; set; }
        public int Rank_alt { get; set; }
        public int Strength { get; set; }
        public int Font_size { get; set; } 
        public int Time_unit_id { get; set; }

        // Avoid circular references
        [JsonIgnore]
        public TimeUnit TimeUnit { get; set; } = new TimeUnit();
    }
}