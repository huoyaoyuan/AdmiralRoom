using System.Collections.Generic;
using System.IO;
using System.Linq;

#pragma warning disable CC0022

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public class CsvLogger<T> : Logger<T>
        where T : ILog
    {
        private readonly string filename;
        private FileInfo file;
        private readonly string fileheader;
        public CsvLogger(string filename, bool useshown = false) : base(useshown)
        {
            this.filename = filename;
            string[] titles = useshown ? provider.Titles.Select(GetText).ToArray() : provider.Titles;
            fileheader = string.Join(",", titles);
            CheckFile();
            using (var reader = file.OpenText())
                if (reader.ReadLine().Trim() == fileheader) return;
            try
            {
                file.MoveTo(filename + ".backup");
                CheckFile();
            }
            catch (IOException) { }
        }
        private void CheckFile()
        {
            file = new FileInfo(filename);
            if (!file.Exists)
                using (var writer = file.CreateText())
                {
                    writer.WriteLine(fileheader);
                    writer.Flush();
                }
        }
        public override void Log(T item)
        {
            CheckFile();
            using (var fs = file.Open(FileMode.Append, FileAccess.Write))
            {
                var writer = new StreamWriter(fs);
                writer.WriteLine(string.Join(",", provider.GetValues(item)));
                writer.Flush();
            }
        }
        public override void Import(IEnumerable<T> items)
        {
            CheckFile();
            using (var fs = file.Open(FileMode.Append, FileAccess.Write))
            {
                var writer = new StreamWriter(fs);
                foreach (var item in items)
                    writer.WriteLine(string.Join(",", provider.GetValues(item)));
                writer.Flush();
            }
        }
        public override IEnumerable<T> Read()
        {
            CheckFile();
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
