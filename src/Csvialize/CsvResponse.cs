using Nancy;

namespace Csvialize
{
    public class CsvResponse : Response
    {
        public CsvResponse(object model, ISerializer serializer)
        {
            Contents = stream => serializer.Serialize(ContentTypes.Csv, model, stream);
            ContentType = ContentTypes.Csv;
            StatusCode = HttpStatusCode.OK;
        }
    }
}