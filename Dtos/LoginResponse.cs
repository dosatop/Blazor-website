namespace UserManagementBlazor.Dtos
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
        public int ExpiresIn { get; set; }
    }
}