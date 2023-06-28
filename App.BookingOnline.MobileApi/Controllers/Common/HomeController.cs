using App.BookingOnline.AppApi.Controllers;
using App.BookingOnline.Data.Paging;
using App.BookingOnline.Service;
using App.BookingOnline.Service.DTO.Common;
using App.Core;
using App.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.Context;
using System;
using System.IO;
using System.Threading.Tasks;

namespace App.BookingOnline.MobileApi.Controllers.Common
{
    public class HomeController : GridController<HomeDTO, UserPagingModel, IHomeService>
    {
        private readonly ILogger _log;
        public HomeController(IHomeService service, ILogger<HomeController> logger) : base(service)
        {
            _log = logger;
        }

        /// <summary>
        /// Sau khi đăng nhập sẽ gọi api này để lấy data màn hình trang chủ
        /// </summary>
        /// <returns></returns>
        [HttpGet("Home")]
        public async Task<RespondData> Home(string lang = "vn")
        {
            return await _service.GetHomeData(UserId, lang, UserName);
        }

        /// <summary>
        /// Api để lưu ngôn ngữ người dùng chọn
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        [HttpGet("SettingLanguage")]
        public async Task<RespondData> SettingLanguage(string lang = "vn")
        {
            try
            {
                _service.SettingLanguage(lang, UserId);
                return new RespondData { IsSuccess = true };
            }
            catch (Exception e)
            {
                using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
                {
                    _log.LogError(e.Message);
                }
            }

            return new RespondData { IsSuccess = false };
        }

        /// <summary>
        /// Sau khi login thì gọi api này để lưu fcmtoken mới nhất và ngôn ngữ
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("SettingUser")]
        public async Task<RespondData> SettingUser(SettingUser data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.FcmToken) || !string.IsNullOrEmpty(data.Lang))
                {
                    _service.InsertFcmTokenDevice(data.FcmToken, data.Lang, UserId);
                }
                //if (!string.IsNullOrEmpty(data.Lang))
                //{
                //    _service.SettingLanguage(data.Lang, UserId);
                //}
                return new RespondData { IsSuccess = true };
            }
            catch (Exception e)
            {
                using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
                {
                    _log.LogError(e.Message);
                }
            }

            return new RespondData { IsSuccess = false };
        }

        /// <summary>
        /// lưu fcm token device của firebase
        /// </summary>
        /// <param name="fcmToken"></param>
        /// <returns></returns>
        //[HttpGet("InsertFcmTokenDevice")]
        //public async Task<RespondData> InsertFcmTokenDevice(string fcmToken)
        //{
        //    try
        //    {
        //        _service.InsertFcmTokenDevice(fcmToken, UserId);

        //    }
        //    catch (Exception e)
        //    {
        //        using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
        //        {
        //            _log.LogError(e.Message);
        //        }
        //    }

        //    return new RespondData { IsSuccess = false };
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetContactAllCourse")]
        public async Task<RespondData> GetContactAllCourse()
        {
            return await _service.GetContactAllCourse();
        }

        /// <summary>
        /// Lấy thông tin contact đơn vị quản lý sân
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetContactAllOrg")]
        [AllowAnonymous]
        public RespondData GetContactAllOrg()
        {
            return _service.GetContactAllOrg();
        }

        /// <summary>
        /// Lấy thông tin member card của user đã login
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMemberCard")]
        public async Task<RespondData> GetMemberCard()
        {
            return await _service.GetMemberCard(UserId);
        }

        /// <summary>
        /// Gọi hàm này để lấy dữ liệu message tiếng việt = vn, tiếng anh = en
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMessageError")]
        [AllowAnonymous]
        public RespondData GetMessageError(string lang)
        {
            try
            {
                var folderName = Path.Combine("Assets\\Message", "message.vn.json");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (lang == Constants.LangEn)
                {
                    folderName = Path.Combine("Assets\\Message", "message.en.json");
                    pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                }

                using (StreamReader file = System.IO.File.OpenText(pathToSave))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        JObject o2 = (JObject)JToken.ReadFrom(reader);
                        var jsonObj1 = JsonConvert.SerializeObject(o2);
                        //var jsonObj2 = JsonConvert.DeserializeObject<ErrorObj>(jsonObj1);
                        return new RespondData { IsSuccess = true, Data = jsonObj1 };
                    }
                }
            }
            catch (Exception e)
            {
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }
        public class ErrorObj
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        /// <summary>
        /// Kiểm tra nếu message lỗi bằng tiếng việt có thay đổi nội dung không.
        /// </summary>
        /// <response code="IsSuccess=true">Nếu có thay đổi so với lần update trước</response>
        /// <response code="IsSuccess=false">Không có thay đổi</response>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("CheckUpdateMessageVn")]
        public RespondData CheckUpdateMessageVn()
        {
            try
            {
                var folderName = Path.Combine("Assets\\Message", "message.vn.json");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var JSON = System.IO.File.ReadAllText(pathToSave);
                dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                var isUpdated = Convert.ToBoolean(jsonObj["isUpdated"].ToString());
                return new RespondData { IsSuccess = isUpdated };
            }
            catch (Exception e)
            {
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Sau khi clone message xuống app thành công sẽ gọi API này để đánh dấu không cần update nữa
        /// </summary>
        /// <returns></returns>
        [HttpGet("UpdateMessageVnSuccesful")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public RespondData UpdateMessageVnSuccesful()
        {
            try
            {
                var folderName = Path.Combine("Assets\\Message", "message.vn.json");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var JSON = System.IO.File.ReadAllText(pathToSave);
                dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                jsonObj["isUpdated"] = "false";
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                System.IO.File.WriteAllText(pathToSave, output);
                return new RespondData { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }
        /// <summary>
        /// Sau khi clone message xuống app thành công sẽ gọi API này để đánh dấu không cần update nữa
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("UpdateMessageEnSuccesful")]
        public RespondData UpdateMessageEnSuccesful()
        {
            try
            {
                var folderName = Path.Combine("Assets\\Message", "message.en.json");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var JSON = System.IO.File.ReadAllText(pathToSave);
                dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                jsonObj["isUpdated"] = "false";
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                System.IO.File.WriteAllText(pathToSave, output);
                return new RespondData { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }
        /// <summary>
        /// Gọi hàm này để lấy dữ liệu message tiếng anh
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetMessageEn")]
        public RespondData GetMessageEn()
        {
            try
            {
                var folderName = Path.Combine("Assets\\Message", "message.en.json");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var JSON = System.IO.File.ReadAllText(pathToSave);
                var jsonObj = JsonConvert.DeserializeObject(JSON);

                return new RespondData { IsSuccess = true, Data = jsonObj };
            }
            catch (Exception e)
            {
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }

        /// <summary>
        /// ContentUpdateResult.MessageVnResult = true => chuyển trạng thái đã update content
        /// <br /> 
        /// ContentUpdateResult.MessageEnResult = true => chuyển trạng thái đã update content
        /// <br /> 
        /// ContentUpdateResult.TermsConditionsVnResult = true => chuyển trạng thái đã update content
        /// <br /> 
        /// ContentUpdateResult.TermsConditionsEnResult = true => chuyển trạng thái đã update content
        /// <br /> 
        /// ContentUpdateResult.CancelPolicyVnResult = true => chuyển trạng thái đã update content
        /// <br /> 
        /// ContentUpdateResult.CancelPolicyEnResult = true => chuyển trạng thái đã update content
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("UpdateContentSuccesful")]
        [AllowAnonymous]
        public RespondData UpdateContentSuccesful(ContentUpdateResult model)
        {
            try
            {
                if (model.MessageVnResult)
                {
                    var folderName = Path.Combine("Assets\\Message", "message.vn.json");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    var JSON = System.IO.File.ReadAllText(pathToSave);
                    dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                    jsonObj["isUpdated"] = "false";
                    string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    System.IO.File.WriteAllText(pathToSave, output);

                    //bool MessageVn = _service.UpdateStatusMessageVnByUser(UserId, false);

                }
                if (model.MessageEnResult)
                {
                    var folderName = Path.Combine("Assets\\Message", "message.vn.json");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    var JSON = System.IO.File.ReadAllText(pathToSave);
                    dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                    jsonObj["isUpdated"] = "false";
                    string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    System.IO.File.WriteAllText(pathToSave, output);
                    //bool MessageVn = _service.UpdateStatusMessageVnByUser(UserId, false);
                }
                if (model.TermsConditionsVnResult)
                {
                    var folderName = Path.Combine("Assets\\TernCondition", "TernCondition.vn.json");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    var JSON = System.IO.File.ReadAllText(pathToSave);
                    dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                    jsonObj["isUpdated"] = "false";
                    string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    System.IO.File.WriteAllText(pathToSave, output);
                }
                if (model.TermsConditionsEnResult)
                {
                    var folderName = Path.Combine("Assets\\TernCondition", "TernCondition.en.json");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    var JSON = System.IO.File.ReadAllText(pathToSave);
                    dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                    jsonObj["isUpdated"] = "false";
                    string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    System.IO.File.WriteAllText(pathToSave, output);
                }
                if (model.CancelPolicyVnResult)
                {
                    var folderName = Path.Combine("Assets\\CancelPolicy", "CancelPolicy.vn.json");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    var JSON = System.IO.File.ReadAllText(pathToSave);
                    dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                    jsonObj["isUpdated"] = "false";
                    string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    System.IO.File.WriteAllText(pathToSave, output);
                }
                if (model.CancelPolicyEnResult)
                {
                    var folderName = Path.Combine("Assets\\CancelPolicy", "CancelPolicy.en.json");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    var JSON = System.IO.File.ReadAllText(pathToSave);
                    dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                    jsonObj["isUpdated"] = "false";
                    string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    System.IO.File.WriteAllText(pathToSave, output);
                }
                return new RespondData { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }

        public class ContentUpdateResult
        {
            public bool MessageVnResult { get; set; }
            public bool MessageEnResult { get; set; }
            public bool TermsConditionsVnResult { get; set; }
            public bool TermsConditionsEnResult { get; set; }
            public bool CancelPolicyVnResult { get; set; }
            public bool CancelPolicyEnResult { get; set; }

        }

        /// <summary>
        /// IsAppUpdate = true, user need update app again from store
        /// </summary>
        public class ContentUpdateStatus
        {
            public bool MessageVn { get; set; }
            public string MessageVnVersion { get; set; }
            public bool MessageEn { get; set; }
            public string MessageEnVersion { get; set; }
            public bool TermsConditionsVn { get; set; }
            public string TermsConditionsVnVersion { get; set; }
            public bool TermsConditionsEn { get; set; }
            public string TermsConditionsEnVersion { get; set; }
            public bool CancelPolicyVn { get; set; }
            public string CancelPolicyVnVersion { get; set; }
            public bool CancelPolicyEn { get; set; }
            public string CancelPolicyEnVersion { get; set; }
        }
        /// <summary>
        /// if MessageVn = true => update message tiếng việt
        /// <br /> 
        /// if MessageEn = true => update message tiếng anh
        /// <br /> 
        /// if TermsConditionsVn = true => update message tiếng việt
        /// <br /> 
        /// if TermsConditionsEn = true => update message tiếng anh
        /// <br /> 
        /// if CancelPolicyVn = true => update message tiếng việt
        /// <br /> 
        /// if CancelPolicyEn = true => update message tiếng anh
        /// </summary>        
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("CheckUpdateContentMessage")]
        public RespondData CheckUpdateContentMessage()
        {
            try
            {
                var status = new ContentUpdateStatus();

                var folderName = Path.Combine("Assets\\Message", "message.vn.json");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var JSON = System.IO.File.ReadAllText(pathToSave);
                dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                var MessageVn = Convert.ToBoolean(jsonObj["isUpdated"].ToString());
                status.MessageVn = MessageVn;
                var MessageVnVersion = jsonObj["version"].ToString();
                status.MessageVnVersion = MessageVnVersion;

                var folderName1 = Path.Combine("Assets\\Message", "message.en.json");
                var pathToSave1 = Path.Combine(Directory.GetCurrentDirectory(), folderName1);
                var JSON1 = System.IO.File.ReadAllText(pathToSave1);
                dynamic jsonObj1 = JsonConvert.DeserializeObject(JSON1);
                var MessageEn = Convert.ToBoolean(jsonObj["isUpdated"].ToString());
                status.MessageEn = MessageEn;
                var MessageEnVersion = jsonObj["version"].ToString();
                status.MessageEnVersion = MessageEnVersion;

                var TernCondition = Path.Combine("Assets\\TernCondition", "TernCondition.vn.json");
                var TernConditionPath = Path.Combine(Directory.GetCurrentDirectory(), TernCondition);
                var TernConditionJSON = System.IO.File.ReadAllText(TernConditionPath);
                dynamic TernConditionjsonObj = JsonConvert.DeserializeObject(TernConditionJSON);
                var TermsConditionsVn = Convert.ToBoolean(TernConditionjsonObj["isUpdated"].ToString());
                status.TermsConditionsVn = TermsConditionsVn;

                var CancelPolicy = Path.Combine("Assets\\CancelPolicy", "CancelPolicy.vn.json");
                var CancelPolicyPath = Path.Combine(Directory.GetCurrentDirectory(), CancelPolicy);
                var CancelPolicyJSON = System.IO.File.ReadAllText(CancelPolicyPath);
                dynamic CancelPolicyjsonObj = JsonConvert.DeserializeObject(CancelPolicyJSON);
                var CancelPolicyVn = Convert.ToBoolean(CancelPolicyjsonObj["isUpdated"].ToString());
                status.CancelPolicyVn = CancelPolicyVn;

                var TernCondition1 = Path.Combine("Assets\\TernCondition", "TernCondition.en.json");
                var TernConditionPath1 = Path.Combine(Directory.GetCurrentDirectory(), TernCondition1);
                var TernConditionJSON1 = System.IO.File.ReadAllText(TernConditionPath1);
                dynamic TernConditionjsonObj1 = JsonConvert.DeserializeObject(TernConditionJSON1);
                var TermsConditionsEn = Convert.ToBoolean(TernConditionjsonObj1["isUpdated"].ToString());
                status.TermsConditionsEn = TermsConditionsEn;

                var CancelPolicy1 = Path.Combine("Assets\\CancelPolicy", "CancelPolicy.en.json");
                var CancelPolicyPath1 = Path.Combine(Directory.GetCurrentDirectory(), CancelPolicy1);
                var CancelPolicyJSON1 = System.IO.File.ReadAllText(CancelPolicyPath1);
                dynamic CancelPolicyjsonObj1 = JsonConvert.DeserializeObject(CancelPolicyJSON1);
                var CancelPolicyEn = Convert.ToBoolean(CancelPolicyjsonObj1["isUpdated"].ToString());
                status.CancelPolicyEn = CancelPolicyEn;

                return new RespondData { IsSuccess = true, Data = status };
            }
            catch (Exception e)
            {
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }

        [HttpPost("UpdateAppUserVersion")]
        public RespondData UpdateAppUserVersion(string version)
        {
            try
            {
                RespondData RespondData = _service.UpdateAppUserVersion(version, UserId);
                return RespondData;
            }
            catch (Exception e)
            {
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }

        /// <summary>
        /// input: platform (0: ios, 1: android), current version
        /// Output: 0(không update), 1(update không bắt buộc), 2(update bắt buộc)
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="currVer"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("CheckVersionApp")]
        public RespondData CheckVersionApp(int platform, string currVer)
        {
            try
            {
                RespondData RespondData = _service.CheckVersionApp(platform, currVer,UserId);
                return RespondData;
            }
            catch (Exception e)
            {
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }




        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("UpdateTermsConditionsVnSuccesful")]
        public RespondData UpdateTermsConditionsVnSuccesful()
        {
            try
            {
                var folderName = Path.Combine("Assets\\TernCondition", "TernCondition.vn.json");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var JSON = System.IO.File.ReadAllText(pathToSave);
                dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                jsonObj["isUpdated"] = "false";
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                System.IO.File.WriteAllText(pathToSave, output);
                return new RespondData { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("UpdateTermsConditionsEnSuccesful")]
        public RespondData UpdateTermsConditionsEnSuccesful()
        {
            try
            {
                var folderName = Path.Combine("Assets\\TernCondition", "TernCondition.en.json");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var JSON = System.IO.File.ReadAllText(pathToSave);
                dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                jsonObj["isUpdated"] = "false";
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                System.IO.File.WriteAllText(pathToSave, output);
                return new RespondData { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Điều kiện và điều khoản
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("TermsConditionsApply")]
        public RespondData TermsConditionsApply(string lang)
        {
            try
            {
                var pathToSave = string.Empty;
                if (lang == Constants.LangEn)
                {
                    var folderName = Path.Combine("Assets\\TernCondition", "TernCondition.en.json");
                    pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                }
                else
                {
                    var folderName = Path.Combine("Assets\\TernCondition", "TernCondition.vn.json");
                    pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                }

                using (StreamReader file = System.IO.File.OpenText(pathToSave))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        JObject o2 = (JObject)JToken.ReadFrom(reader);
                        var content = o2.Last.Last.ToString();
                        var jsonObj1 = JsonConvert.SerializeObject(o2);
                        //var jsonObj2 = JsonConvert.DeserializeObject<ErrorObj>(jsonObj1);
                        return new RespondData { IsSuccess = true, Data = content };
                    }
                }
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("UpdateCancelPolicyVnSuccesful")]
        public RespondData UpdateCancelPolicyVnSuccesful()
        {
            try
            {
                var folderName = Path.Combine("Assets\\CancelPolicy", "CancelPolicy.vn.json");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var JSON = System.IO.File.ReadAllText(pathToSave);
                dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                //jsonObj["isUpdated"] = "false";
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                // System.IO.File.WriteAllText(pathToSave, output);
                return new RespondData { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("UpdateCancelPolicyEnSuccesful")]
        public RespondData UpdateCancelPolicyEnSuccesful()
        {
            try
            {
                var folderName = Path.Combine("Assets\\CancelPolicy", "CancelPolicy.en.json");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var JSON = System.IO.File.ReadAllText(pathToSave);
                dynamic jsonObj = JsonConvert.DeserializeObject(JSON);
                //jsonObj["isUpdated"] = "false";
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                //System.IO.File.WriteAllText(pathToSave, output);
                return new RespondData { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Chính sách hoàn hủy 
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetCancelPolicy")]
        public RespondData GetCancelPolicy(string lang)
        {
            try
            {
                var pathToSave = string.Empty;
                if (lang == Constants.LangEn)
                {
                    var folderName = Path.Combine("Assets\\CancelPolicy", "CancelPolicy.en.json");
                    pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                }
                else
                {
                    var folderName = Path.Combine("Assets\\CancelPolicy", "CancelPolicy.vn.json");
                    pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                }
                using (StreamReader file = System.IO.File.OpenText(pathToSave))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        JObject o2 = (JObject)JToken.ReadFrom(reader);
                        var content = o2.Last.Last.ToString();
                        //var jsonObj1 = JsonConvert.SerializeObject(content);
                        //var jsonObj2 = JsonConvert.DeserializeObject<ErrorObj>(jsonObj1);
                        return new RespondData { IsSuccess = true, Data = content };
                    }
                }
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                return new RespondData { IsSuccess = false, Message = e.Message };
            }
        }
    }

    public class SettingUser
    {
        public string FcmToken { get; set; }
        public string Lang { get; set; }
    }
}
