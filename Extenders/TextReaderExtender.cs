using System;
using System.IO;
using System.Reactive.Linq;
using System.Xml;

namespace Extentions
{
    public static class TextReaderExtender
    {
        public static IObservable<XmlReader> XmlReaderObserver<T>(this TextReader TextReader, string Tag) =>
          Observable.Generate(
              XmlReader.Create(TextReader),
              x => !x.EOF && x.ReadToFollowing(Tag),
              x => x,
              x => x.ReadSubtree()
          );
    }
}