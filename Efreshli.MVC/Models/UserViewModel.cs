using Efreshli.Application.DTOs.IdentityDTOs;

namespace Efreshli.MVC.Models
{
    public class UserViewModel
    {
        public int TotlaUsers { get; set; } = 0;
        public int TotalAdmins { get; set; } = 0;
        public int TotalCustomers { get; set; } = 0;
        public List<UserProfileResponseDto>? Users { get; set; } = default;
    }
}
