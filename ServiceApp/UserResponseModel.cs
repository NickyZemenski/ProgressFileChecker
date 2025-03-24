using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApp
{
    public class UserResponseModel
    {
        public string emailFormat { get; set; }
        public string notes { get; set; }
        public string statusNote { get; set; }
        public DateTime passwordChangeStamp { get; set; }
        public string receivesNotification { get; set; }
        public string forceChangePassword { get; set; }
        public int folderQuota { get; set; }
        public int totalFileSize { get; set; }
        public string authMethod { get; set; }
        public string language { get; set; }
        public int homeFolderID { get; set; }
        public int defaultFolderID { get; set; }
        public string expirationPolicyID { get; set; }
        public PageSize displaySettings { get; set; }
        public string id { get; set; }
        public int orgID { get; set; }
        public string username { get; set; }
        public string fullName { get; set; }
        public string permission { get; set; }
        public string email { get; set; }
        public string status { get; set; }
        public DateTime lastLoginStamp { get; set; }
    }

    public class PageSize
    {
        public int userListPageSize { get; set; }
        public int fileListPageSize { get; set; }
        public int liveViewPageSize { get; set; }
    }

}
