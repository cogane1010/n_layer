using System.Collections.Generic;

namespace App.Core.Domain
{
    public class RespondData
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string MsgCode { get; set; }
        public List<string> MsgParams { get; set; }
        public object Data { get; set; }
        public string CurrentUser { get; set; }
    }
}
