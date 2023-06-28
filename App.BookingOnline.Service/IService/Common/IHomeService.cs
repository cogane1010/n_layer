using App.BookingOnline.Data.Paging;
using App.BookingOnline.Service.Base;
using App.BookingOnline.Service.DTO.Common;
using App.Core.Domain;
using System.Threading.Tasks;

namespace App.BookingOnline.Service
{
    public interface IHomeService : IBaseGridDataService<HomeDTO, UserPagingModel>
    {
        Task<RespondData> GetHomeData(string userId, string lang, string userName);
        Task<RespondData> GetContactAllCourse();
        Task<RespondData> GetMemberCard(string userId);
        void SettingLanguage(string lang, string userId);
        void InsertFcmTokenDevice(string fcmToken, string lang, string userId);
        RespondData GetContactAllOrg();
        bool GetStatusMessageVnByUser(string userId);
        bool UpdateStatusMessageVnByUser(string userId, bool v);
        RespondData UpdateAppUserVersion(string version, string userId);
        string GetAppVersion(bool v);
       
        RespondData CheckVersionApp(int platform, string currVer, string userId);
    }
}