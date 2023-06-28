using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace App.BookingOnline.Service.IService.Common
{
    public interface IFileService
    {
        string SaveFile(string fileName, string folderPath, byte[] content,DateTime? transDate= null);
        string GetRootPath();
    }
}
