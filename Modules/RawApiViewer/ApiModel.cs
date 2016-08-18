using Huoyaoyuan.AdmiralRoom;
using Newtonsoft.Json.Linq;

namespace RawApiViewer
{
    class ApiModel
    {
        public CachedSession Session { get; }
        public JToken _json;
        public JToken Json => _json != null ? _json : (_json = JToken.Parse(Session.JsonResponse));
        public ApiModel(CachedSession oSession)
        {
            Session = oSession;
        }
    }
}
