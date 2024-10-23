namespace dsc_backend.DAO
{
    public class ChangePasswordRequestDAO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}
