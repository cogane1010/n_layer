using App.BookingOnline.Data.Models;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App.BookingOnline.Service.IService.Common
{
    public interface ISftpService
    {
        IEnumerable<SftpFile> ListAllFiles(string remoteDirectory = ".");
        void UploadFile(string localFilePath, string remoteFilePath);
        void DownloadFile(string remoteFilePath, string localFilePath);
        void DeleteFile(string remoteFilePath);

        string UploadFileTransc(string localFilePath, string filename, string userId, DateTime dateTrans);

    }
}
