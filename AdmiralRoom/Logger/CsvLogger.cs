using System;
using System.IO;

#pragma warning disable CC0022

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public abstract class CsvLogger : Logger
    {
        private readonly FileInfo file;
        protected CsvLogger(string filename)
        {
            if (Path.GetExtension(filename) != ".csv") throw new ArgumentException();
            file = new FileInfo(filename);
            if (!file.Exists)
            {
#pragma warning disable RECS0021
                using (var sw = file.CreateText())
                    Initialize(sw);
#pragma warning restore RECS0021
            }
        }
        protected abstract void Initialize(TextWriter writer);
        protected sealed override void Log(params object[] data)
        {
            using (var fs = file.Open(FileMode.Append, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs);
                Log(sw, data);
            }
        }
        protected void Log(TextWriter writer, params object[] data)
        {
            for (int i = 0; i < data.Length - 1; i++)
            {
                writer.Write(data[i]);
                writer.Write(',');
            }
            writer.WriteLine(data[data.Length - 1]);
        }
    }
}
