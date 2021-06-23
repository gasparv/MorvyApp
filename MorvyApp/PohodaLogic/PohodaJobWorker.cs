using MorvyApp.Models;
using System.Threading;

namespace MorvyApp.PohodaLogic
{
    /// <summary>
    /// Tato trieda ma na starosti spravu timera a volanie jobu.
    /// 
    /// Logika pri vsetkych triedach by mala byt v zmysle, ze kazda trieda je zodpovedna len za jednu vec.
    /// Tato trieda preto riesi len spravu noveho vlakna v ktorom bezi import/export a volanie inej triedy ktora ten proces dalej manazuje (PohodaImportExportTool).
    /// Ak by sme pouzivali koncept DependencyInjection tak vlastne triedy, ktore nieco budu robit su volane ako sluzby.
    /// T.j. mas nejaky main flow a ten main flow si vola pocas svojho behu sluzby podla potreby, ktore nieco urobia a pokracuje dalej main flow.
    /// 
    /// </summary>
    public class PohodaJobWorker
    {
        //Kedze mame nejaky job ktory bezi na pozadi a vkuse, je idealne aby nam neblokoval hlavny thread, t.j. Form a NotificationIcon aplikacie.
        //Preto ho spustime spolu s aplikaciou ako samostatne vlakno.
        Thread _timerThread;
        
        //Definicia sluzby PohodaImportExportTool, ktoru budeme volat na import/export
        PohodaImportExportTool _pohodaImportExportTool;

        //Toto je premenna, ktora ma aj deklaraciu s defaultnym nastavenim, ktore mozes overridnut cez volanie konstruktora, kde zadas casovy interval
        private readonly double timerInterval = 5000;

        //Hlavny konstruktor, t.j. co sa ma udiat ked sa vytvara objekt bez zadanych parametrov.
        public PohodaJobWorker()
        {
            //Definujem co sa ma spustat v timerThread.
            _timerThread = new Thread(StartThreadTimer);

            //Definujem PohodaImportExportTool
            _pohodaImportExportTool = new PohodaImportExportTool();
        }
        
        /// <summary>
        /// Tymto konstruktorom mozes overridnut nastavenie intervalu opakovania.
        /// </summary>
        /// <param name="interval"></param>
        public PohodaJobWorker(double interval)
        {
            //Tu sa zmeni defaultna hodota na vstupnu
            timerInterval = interval;
            //Definujem co sa ma spustat v timerThread
            _timerThread = new Thread(StartThreadTimer);
            _pohodaImportExportTool = new PohodaImportExportTool();
        }
        
        /// <summary>
        /// Startne thread a vrati mi aktualne existujuci objekt celej PohodaJobWorker triedy. Vdaka tomu mozem pouzivat fluent method chaining, vid. Form1.cs
        /// </summary>
        /// <returns></returns>
        public PohodaJobWorker StartJob()
        {
            _timerThread.Start();
            return this;
        }

        /// <summary>
        /// Toto je privatna metoda, nepotrebujem ju volat nikde mimo tejto triedy a predstavuje len nejaku lokalnu logiku na spustenie timera a nastavenie eventu ked timer zbehne casovy limit
        /// </summary>
        private void StartThreadTimer()
        {
            //Vyrobil som si timer a priradil som mu udalost. Tento timer bude vlastne bezat po celu zivotnost vlakna, cize nemusim riesit jeho stopnutie ani nic podobne.
            System.Timers.Timer ftpDownloadTimer =
                new System.Timers.Timer
                {
                    Interval = timerInterval,
                    Enabled = true
                };
            ftpDownloadTimer.Elapsed += FtpDownloadTimer_Elapsed;

        }

        /// <summary>
        /// Toto je tiez privatna metoda, event ktory je callback vyvolany vtedy keed timer interval zbehne.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FtpDownloadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _pohodaImportExportTool.ProcessPohodaImportExport(new FileHandlingSettings(), new FileHandlingSettings());
        }

        /// <summary>
        /// Pomocou tejto metody mozem killnut moj thread ked ukoncujem aplikaciu. 
        /// Niektore thready mozu ostat zive aj po vypnuti aplikacie, hlavne ak robia nejaky task ktory ma thread lock viazany na databazu alebo siet.
        /// Preto je idealne JOINnut thread, co ho vlastne spoji po ukoknceni cinnosti este pred ukoncenim aplikacie s hlavnym vlakom a potom sa ukoknci hlavne vlakno spolu s aplikaciou.
        ///
        /// Thread manazment vie byt niekedy o drzku
        /// </summary>
        public void TerminateJob()
        {
            if (_timerThread != null && _timerThread.IsAlive)
                _timerThread.Join();
        }
    }
}
