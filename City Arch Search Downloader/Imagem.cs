namespace City_Arch_Search_Downloader
{
    public class Imagem
    {
        public string Local { get; set; }
        public string Nome { get; set; }
        public string Extensao { get; set; }

        public string LocalCompleto
        {
            get { return Local + Nome + Extensao; }
        }
    }
}