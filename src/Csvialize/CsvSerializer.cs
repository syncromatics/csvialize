using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvHelper;
using Nancy;
using Nancy.IO;

namespace Csvialize
{
    public class CsvSerializer : ISerializer
    {
        public bool CanSerialize(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals(ContentTypes.Csv, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
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