using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvHelper;
using Nancy;
using Nancy.IO;
using Nancy.Responses.Negotiation;

namespace Csvialize
{
    public class CsvSerializer : Nancy.ISerializer
    {
        public bool CanSerialize(MediaRange mediaRange)
        {
            string contentType = mediaRange;
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals(ContentTypes.Csv, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Serialize<TModel>(MediaRange mediaRange, TModel model, Stream outputStream)
        {
            using (var writer = new CsvWriter(new StreamWriter(new UnclosableStreamWrapper(outputStream))))
            {
                var enumerable = model as IEnumerable;

                try
                {
                    if (enumerable != null)
                    {
                        writer.WriteRecords(enumerable);
                    }
                    else
                    {
                        writer.WriteRecord(model);
                    }
                }
                catch
                {
                    var message = Encoding.ASCII.GetBytes("An error occurred.");
                    outputStream.Write(message, 0, message.Length);
                }
            }
        }

        public IEnumerable<string> Extensions
        {
            get
            {
                yield return "csv";
            }
        }
}
}