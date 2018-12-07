using BarcoRota.Models;

namespace BarcoRota.Client.Models
{
    public class BarcoMemberViewModel
    {
        public string Name { get; }
        public BarcoMemberViewModel(BarcoMember barcoMember)
        {
            if (barcoMember.NickName != null)
            {
                Name = barcoMember.NickName;
            }
            else {
                Name = barcoMember.Name;
            }
        }
    }
}
