using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace City_Arch_Search_Downloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var local = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Images\\";

            var imagens = GerarImagens(local);
            BaixarImagens(imagens);
            SanitizarImagensNaoBaixadas(local);
        }

        private static void SanitizarImagensNaoBaixadas(string local)
        {
            var files = Directory.EnumerateFiles(local)
                .Select(f => new FileInfo(f))
                .Where(f => f.Length <= 0);

            foreach (var arquivoParaDeletar in files)
            {
                File.Delete(arquivoParaDeletar.FullName);
            }
        }

        private static void BaixarImagens(ICollection<Imagem> imagens)
        {
            var uri = "https://kraken99.blob.core.windows.net/images1000xn/";
            var tasks = new List<Task>();
            try
            {
                foreach (var imagem in imagens)
                {
                    using (var webClient = new WebClient())
                    {
                        Console.WriteLine("Iniciando Download {0}", imagem.Nome);
                        webClient.DownloadFileCompleted += DownloadCompleted;

                        var task = webClient.DownloadFileTaskAsync(new Uri(uri + imagem.Nome + imagem.Extensao),
                            imagem.Local);

                        tasks.Add(task);
                    }
                }

                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception)
            {
                Console.WriteLine("Começando a limpar...");
            }
        }

        private static void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var t = e.UserState as TaskCompletionSource<object>;
            Console.WriteLine("Download terminado {0}", t.Task.Id);
        }


        private static ICollection<Imagem> GerarImagens(string local)
        {
            var extensao = ".jpg";

            var result = new List<Imagem>();

            for (int i = 1; i <= 6700; i++)
            {
                result.Add(new Imagem()
                {
                    Nome = i.ToString(),
                    Extensao = extensao,
                    Local = local + i + extensao
                });
            }

            return result;
        }
    }

    public class Imagem
    {
        public string Local { get; set; }
        public string Nome { get; set; }
        public string Extensao { get; set; }
    }
}
