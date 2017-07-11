using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.Responses.Negotiation;

namespace Csvialize
{
    public class CsvProcessor : IResponseProcessor
    {
        private readonly ISerializer _serializer;

        public CsvProcessor(IEnumerable<ISerializer> serializers)
        {
            _serializer = serializers.First(x => x.CanSerialize(ContentTypes.Csv));
        }

        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            if (requestedMediaRange.Matches(ContentTypes.Csv))
            {
                return new ProcessorMatch
                {
                    ModelResult = MatchResult.DontCare,
                    RequestedContentTypeResult = MatchResult.ExactMatch
                };
            }

            return new ProcessorMatch
            {
                ModelResult = MatchResult.DontCare,
                RequestedContentTypeResult = MatchResult.NoMatch
            };
        }

        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            return new CsvResponse(model, _serializer);
        }

        private static readonly IEnumerable<Tuple<string, MediaRange>> _extensionMappings = new[] { Tuple.Create("csv", new MediaRange(ContentTypes.Csv))};

        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings => _extensionMappings;
    }
}