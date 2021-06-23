using MorvyApp.PohodaLogic;
using System.Configuration;
using System.Windows.Forms;

namespace MorvyApp
{
    public partial class Form1 : Form
    {
        // PohodaJobWorker je trieda, ktora ma na starosti vytvorenie noveho threadu, kt. bude pravidelne robit import/export cez proces pohody
        // Ak planujes pouzivat objekt pohodaJob na viacerych miestach, je dobre ho deklarovat na urovni triedy a pouzivat ho ako referenciu v metodach.
        // Musis ho ale niekde aj definovat -> inicializovat aby nebol NULL ked s nim budes narabat -> inac riskujes NullObjectReference exception
        PohodaJobWorker pohodaJob;
        public Form1()
        {
            InitializeComponent();
            //trayApp je typ NotificationIcon, kt. bol pridany cez toolbox priamo do Formu - je to nieco ako vizualny objekt, kt. nevidno
            trayApp.Visible = true;

            //TODO:Get input value from setting inputtext in a form (settings screen)

            // Tu som si inicializoval novy job, kde na vstupe som mu dal "nejaku hodnotu". Hodnota moze byt vytiahnuta aj napr. z App.config suboru, z nejakeho inputu na forme, atd.
            // Urobil som to tak aby mi methoda StartJob vratila na vystupe cely objekt pohodaJob. Je to tzv. fluent pattern, t.j. mozes retazit volanie metod zasebou.
            var interval = int.Parse(ConfigurationManager.AppSettings["TimeInterval"].ToString());
            pohodaJob = new PohodaJobWorker(interval).StartJob();
        }

        // Toto je event, ktory je nahaknuty na zatvorenie Form1, resp. idealnejsi by bol even kedy je forma znicena, cize nieco v style OnDisposed
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            pohodaJob.TerminateJob();
        }
    }
}
