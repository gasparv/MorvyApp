using MorvyApp.Models;
using System;
using System.IO;
using System.Net;

namespace MorvyApp.PohodaLogic
{
    // Tato trieda ma na starosti upload a download suborov z FTP
    public class PohodaFileManager
    {
        /// <summary>
        /// tato metoda riesi download suboru zo zadanej cesty a ulozi ho pod zadanym nazvom na zadanu cestu. Zaroven checkne ci existuje folder ak nie tak ho vytvori. 
        /// </summary>
        /// <param name="downloadSettings">Trieda obsahujuca URL, filename, folderPath a credentials ak ich treba</param>
        /// <returns>Vrati tzv. tuple. Je to nieco ako rychlokvaseny objekt bez potreby definovat triedu. 
        /// Tuples su dost zakerne ak sa pouzivaju nerozvazne, mali by sa pouzivat len skor ako doplnok vystupu z metody, nie ako nahrada za triedy a objekty.
        /// V tomto pripade tuple pouzivam na to aby som v pripade, ze DL alebo UL neprejde vratil chybovu hlasku s dovodom spolu s vysledkom procesu.
        /// </returns>
        public (bool isSuccessful, string errorMessage) DownloadFile(FileHandlingSettings downloadSettings)
        {
            //Podobne WebClient pouziva IDisposable, takze by mal byt dispoznuty po pouzity preto je uzavrety v using
            using (WebClient client = new WebClient())
            {
                string fullpath = Path.Combine(downloadSettings.LocalFolderPath, Path.DirectorySeparatorChar.ToString(), downloadSettings.LocalFileName);
                if (!Directory.Exists(downloadSettings.LocalFolderPath))
                    Directory.CreateDirectory(downloadSettings.LocalFolderPath);
                if (File.Exists(fullpath))
                {
                    //What to do if the file already exists?! Create a new one?    
                }
                try
                {
                    // Jednoducho skusim stiahnut subor :)
                    client.DownloadFile(downloadSettings.URL, fullpath);
                    return (true, string.Empty);
                }
                // V pripade, ze mi download neprejde z nejakeho dovodu, tak vacsinou dostanem konkretny typ Exceptionu. trieda Exception je ale nieco ako general error.
                // Cize ked nemas implementovanu konkretnu exception, tak tato je firenuta vzdy pri akomkolvek vyskyte chyby, cize vlastne pokryje vsetky specificke typy chyb.
                catch (Exception e)
                {
                    return (false, e.Message);
                }
            }
        }
        public (bool isSuccessful, string errorMessage) UploadFile(FileHandlingSettings uploadSettings)
        {
            using (WebClient client = new WebClient())
            {
                string fullpath = Path.Combine(uploadSettings.LocalFolderPath, Path.DirectorySeparatorChar.ToString(), uploadSettings.LocalFileName);
                if (File.Exists(fullpath))
                {
                    try
                    {
                        if (uploadSettings.Credentials != null)
                            client.Credentials = uploadSettings.Credentials;

                        client.UploadFile(uploadSettings.URL, fullpath);
                        return (true, string.Empty);
                    }
                    catch (Exception e)
                    {
                        return (false, e.Message);
                    }
                }
                else
                {
                    //TODO: Handle case when the processed file is not existing in the expected folder
                    // Interpolacia textu je fajn vec. Ak na zaciatku stringu das $, potom v {} len vlozis one line expression alebo parameter kt. chces substituovat.
                    return (false, $"Pohoda export file does not exist in path {fullpath}");
                }
            }
        }
    }
}
