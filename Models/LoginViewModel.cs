namespace TippPlattform.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public LoginViewModel(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public LoginViewModel()
        {
            Username = "";
            Password = "";
        }   
    }

    public class AnmeldenViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public DateTime Geburtsdatum { get; set; }
        public string Ort { get; set; }
        public string Land { get; set; }
    }
}
