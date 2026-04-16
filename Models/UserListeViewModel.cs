using Microsoft.AspNetCore.Mvc;

namespace TippPlattform.Models
{
    public class UserListeViewModel 
    {
        public List<User> Users { get; set; } = new List<User>();
    }
}
