using System.Net;

namespace MorvyApp.Models
{
    /// <summary>
    /// Toto je tzv. POCO object -> Plain old c# object -> resp. model -> obsahuje len properties na vyskladanie vysledneho objektu + moze obsahovat aj nejaku jednoduchu logiku napr. validaciu alebo transformaciu/konverziu hodnot, prepocty a podobne.
    /// Pouziva sa ak
    /// 1. Chces zmensit pocet inputov v metode
    /// 2. Chces zgrupit viacero logicky suvisiacich veci
    /// 3. Chces odseparovat suvisiace veci od logiky v inej triede
    /// </summary>
    /// 
    public class FileHandlingSettings
    {
        private string _url;
        // Toto je property
        // -> property moze mat getter a setter, mozu byt aj privatne, t.j. nastavit alebo citat ich vies len z triedy kde su definovane
        //     -> Getter vracia objekt podla nejakych podmienok; ak mas len get; znamena to ze ti vrati string hodnotu ulozenu napr. v LocalFileName property
        //     -> Getter pre URL som naschval trochu skomplikoval. Takto moze vyzerat nejaka logika k validacii URL napr. t.j. vzdy ked zadas alebo poziadas o URLcku tak ti preleti validaciou.
        //     -> Setter pouziva default keyword value, co je vlastne hodnota ktora ti pride na vstupe ked chces nastavit URL cez set. Cize value je typu string v tomto pripade
        public string URL {
            get
            {
                return ValidateUrl(_url);
            }
            set
            {
                _url = ValidateUrl(value);
            }
        }
        public string LocalFolderPath { get; set; }
        public string LocalFileName { get; set; }
        // Properties mozes pouzit na akykolvek typ, v tomto pripade ocakavam objekt ktory je akehokolvek typu, ktory implementuje interface ICredentials
        public ICredentials Credentials { get; set; }

        private string ValidateUrl(string url)
        {
            //Nejaka smiesna validacia na ukazku
            if (url.Contains("https"))
                return url.Replace("https", "ftp");
            if (url.Contains("http"))
                return url.Replace("http", "ftp");
            
            return url;
        }
    }
}
