namespace MlaWebApi.Controllers
{
    public class GetPendingAddRequestsModel
    {
        public string requester_id;
        public string public_key;
        public string group_id;
        public string group_key;
        public string group_name;
        public string group_owner_id;
        public string requester_fullname;

        public string signature { get; set; }
        public string group_owners_pub_key { get; set; }
        public string requesters_pub_key { get; set; }
        public string join_request_hash { get; internal set; }
    }
}