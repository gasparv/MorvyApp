using MorvyApp.Models;
using System;
using System.Diagnostics;

namespace MorvyApp.PohodaLogic
{
    public class PohodaImportExportTool
    {
        PohodaFileManager _fileManager;
        //default path for pohoda import export executable can be set here, in app settings, constructor or via settings.
        private readonly string _pohodaImportExportExecutable;
        private readonly string _args;
        public PohodaImportExportTool(string eximExecutablePath = @"c:\test.exe", string args = "")
        {
            _fileManager = new PohodaFileManager();
            _pohodaImportExportExecutable = eximExecutablePath;
            _args = args;
        }

        public (bool isSuccessful, string errorMessage) ProcessPohodaImportExport(FileHandlingSettings downloadSettings, FileHandlingSettings uploadSettings)
        {
            var downloadResult = _fileManager.DownloadFile(downloadSettings);
            if (downloadResult.isSuccessful)
            {
                //TODO: Start POHODA executable and fetch result
                RunPohodaImportExportProcess();

                var uploadResult = _fileManager.UploadFile(uploadSettings);
                return (uploadResult.isSuccessful, uploadResult.errorMessage);
            }
            else
            {
                //TODO: Handle exceptions
                HandleImportExportExceptions();
                return (downloadResult.isSuccessful, downloadResult.errorMessage);
            }
        }
        private void HandleImportExportExceptions()
        {
            throw new NotImplementedException("Method not yet implemented");
        }

        private (bool isSuccessful, string errorMessage) RunPohodaImportExportProcess()
        {
            try
            {
                //If an object is disposable (does implement IDisposable) it is a good practice to free the object after its not used anymore
                // In this case enclosing the new process object in the using clause disposes the object after the application passes through WaitForExit (process termination);
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo()
                    {
                        FileName = _pohodaImportExportExecutable,
                        Arguments = _args,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    process.Start();
                    process.ErrorDataReceived += Process_ErrorDataReceived;
                    //Creates process "monitoring" loop if it is still writing something to the console output stream.
                    while (!process.StandardOutput.EndOfStream)
                    {
                        //If there is any output redirection/filtering/checking needed.
                    }
                    process.WaitForExit();
                    return (true, string.Empty);
                }
            }
            catch(Exception e)
            {
                return (false, e.Message);
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //What happens when an error is received from the process output instead of standard output or end of stream
            throw new NotImplementedException();
        }
    }
}
