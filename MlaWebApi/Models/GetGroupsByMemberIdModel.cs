using System.Security.Cryptography.X509Certificates;

namespace MlaWebApi.Controllers
{
    public class GetGroupsByMemberIdModel
    {
        public int group_id;
        public string group_name;
        public string group_key;
        public string owner_fullname;
        public string group_owner_id;
        public string signature;
        public string grp_ownrs_pub_key;

        public string public_key { get; internal set; }
    }
}