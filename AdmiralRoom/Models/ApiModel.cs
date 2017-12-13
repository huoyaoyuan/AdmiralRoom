using Newtonsoft.Json.Linq;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    class ApiModel
    {
        public CachedSession Session { get; }
        public JToken _json;
        public JToken Json => _json ?? (_json = JToken.Parse(Session.JsonResponse));
        public ApiModel(CachedSession oSession)
        {
            Session = oSession;
        }
    }
}
