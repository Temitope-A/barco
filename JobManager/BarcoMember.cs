namespace BarcoRota.Models
{
    public class BarcoMember
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string UserName { get; set; }
        public RotaStatus RotaStatus { get; set; }
    }

    public enum RotaStatus {
        Inactive = 0,
        Active = 1
    }
}
