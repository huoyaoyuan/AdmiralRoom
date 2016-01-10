using System;
using System.Collections.Generic;
using System.IO;

#pragma warning disable CC0022

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public class CsvLogger<T> : Logger<T>
    {
        private readonly PropertyProvider<T> provider = new PropertyProvider<T>();
        private readonly FileInfo file;
        public CsvLogger(string filename, bool usetextkey = false)
        {
            file = new FileInfo(filename);
            if (!file.Exists)
                using (var writer = file.CreateText())
                {
                    writer.WriteLine(string.Join(",", provider.Titles));
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
        public override void Import(IEnumerable<T> items) => items.ForEach(x => Add(x));
        public override IEnumerable<T> Read()
        {
            throw new NotImplementedException();
        }
    }
}
