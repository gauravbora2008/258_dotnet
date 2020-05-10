namespace MlaWebApi.Controllers
{
    public class GetJoinConfirmationsModel
    {
        public string group_owner_id;
        public string requester_id;
        public string encryptedGroupKey;
        public string group_id;
        public string signature;
        public string public_key;
    }
}