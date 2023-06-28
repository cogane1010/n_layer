using Renci.SshNet;

namespace App.BookingOnline.Service.DTO.Common
{
    public class SftpConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public PrivateKeyFile PrivateKey { get; set; }
    }
}
