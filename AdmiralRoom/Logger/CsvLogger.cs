using System.Collections.Generic;
using System.IO;
using System.Linq;

#pragma warning disable CC0022

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public class CsvLogger<T> : Logger<T>
    {
        private readonly FileInfo file;
        public CsvLogger(string filename, bool usetextkey = false)
        {
            file = new FileInfo(filename);
            IEnumerable<string> titles = provider.Titles;
            if (usetextkey) titles = titles.Select(x => GetText(x));
            if (!file.Exists)
                using (var writer = file.CreateText())
                {
                    writer.WriteLine(string.Join(",", titles));
                    writer.Flush();
                }
        }
        public override void Add(T item)
        {
            using (var fs = file.Open(FileMode.Append, FileAccess.Write))
            {
                var writer = new StreamWriter(fs);
                writer.WriteLine(string.Join(",", provider.GetValues(item)));
                writer.Flush();
            }
        }
        public override void Import(IEnumerable<T> items) => items.ForEach(Add);
        public override IEnumerable<T> Read()
        {
            using (var reader = file.OpenText())
            {
                reader.ReadLine();//标题
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var values = line.Split(',');
                    yield return provider.GetItem(values);
                }
            }
        }
    }
}
