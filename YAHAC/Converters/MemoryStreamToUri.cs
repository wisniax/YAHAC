using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace YAHAC.Converters
{

    //hacky wacky

    public class MemoryStreamToUri : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mem = value as MemoryStream;
            mem.Seek(-16, SeekOrigin.End);

            string name = string.Empty;
            for (int i = 0; i < 8; i++)
            {
                name += System.Convert.ToString(mem.ReadByte(), 16);
            }
            name += ".gif";

            var path = Path.GetTempPath() + @"YAHAC\gifs\";
            Directory.CreateDirectory(path);

            var file = System.IO.File.Open(path + name, FileMode.Create);

            mem.Seek(0, SeekOrigin.Begin);
            mem.CopyTo(file);
            file.Flush();
            file.Close();

            return new Uri(path + name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Nah u dont want this");
        }
    }
}