using System;
using System.Linq;

namespace City_Arch_Search_Downloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var local = string.Empty;
            if (!string.IsNullOrEmpty(args.FirstOrDefault()))
                local = args.FirstOrDefault();
            else
                local = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Imagens\\";

            Downloader.BaixarImagens(local);
        }
    }
}
