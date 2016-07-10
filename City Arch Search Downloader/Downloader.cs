using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace City_Arch_Search_Downloader
{
    public static class Downloader
    {
        public const string URI = "https://kraken99.blob.core.windows.net/images1000xn/";
        private static int ULTIMAIMAGEMBAIXADA;
        private static int BLOCOPARADOWNLOAD = 100;

        public static void BaixarImagens(string local)
        {
            bool continuarBaixando;
            do
            {
                continuarBaixando = BaixarBlocoDeImagens(local);
            } while (continuarBaixando);
        }

        private static bool BaixarBlocoDeImagens(string local, int imagemInicial = 6500)
        {
            var tasks = new List<Task>();

            if (imagemInicial < ULTIMAIMAGEMBAIXADA)
                imagemInicial = ULTIMAIMAGEMBAIXADA;

            ULTIMAIMAGEMBAIXADA = imagemInicial + BLOCOPARADOWNLOAD;

            for (; imagemInicial < ULTIMAIMAGEMBAIXADA; imagemInicial++)
            {
                var imagem = new Imagem()
                {
                    Extensao = ".jpg",
                    Local = local,
                    Nome = imagemInicial.ToString()
                };

                if (!File.Exists(imagem.LocalCompleto))
                    tasks.Add(BaixarImagemAsync(imagem));
            }

            return SalvarImagemEmDiscoAsync(tasks, local);
        }

        private static bool SalvarImagemEmDiscoAsync(List<Task> tasks, string local)
        {
            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
                if (e.GetType() == typeof(AggregateException))
                {
                    var excecoes = e.InnerExceptions.Count;

                    if (excecoes > (BLOCOPARADOWNLOAD / 10))
                    {
                        Console.WriteLine("Sanitizando imagens não baixadas");
                        SanitizarImagensNaoBaixadas(local);
                        return false;
                    }
                }
                return true;
            }

            return true;
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

        private static void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var t = e.UserState as TaskCompletionSource<object>;
            Console.WriteLine("Download terminado {0}", t.Task.Id);
        }

        private static Task BaixarImagemAsync(Imagem imagem)
        {
            using (var webClient = new WebClient())
            {
                Console.WriteLine("Iniciando Download {0}", imagem.Nome);
                webClient.DownloadFileCompleted += DownloadCompleted;

                return webClient.DownloadFileTaskAsync(new Uri(URI + imagem.Nome + imagem.Extensao),
                imagem.LocalCompleto);
            }
        }
    }
}
