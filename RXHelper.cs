using System;
using System.IO;
using System.Reactive.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace hwapp
{
    public static class RXHelper
    {
        public static IObservable<XmlReader> XmlReaderObserver<T>(this TextReader TextReader, string Tag) =>
          Observable.Generate(
              XmlReader.Create(TextReader),
              reader => !reader.EOF && reader.ReadToFollowing(Tag),
              reader => reader,
              reader => reader.ReadSubtree()
          );
        public static IObservable<T> Import<T>(this StreamReader StreamReader, XmlSerializer XmlSerializer, string Tag) =>
            StreamReader
            .XmlReaderObserver<T>(Tag)
            .Select(reader => (T)XmlSerializer.Deserialize(reader));
    }
}