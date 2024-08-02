using System.Configuration;
using System.Data;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace WindowLocker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static event EventHandler? ApplicationExit;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Handle unhandled exceptions on the UI thread
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Handle unhandled exceptions on non-UI threads
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Handle application exit
            this.Exit += App_Exit;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ApplicationExit?.Invoke(this, e);

            // Handle UI thread exceptions
            MessageBox.Show("An unhandled exception occurred: " + e.Exception.Message);
            // Prevent default unhandled exception processing
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ApplicationExit?.Invoke(this, e);

            // Handle non-UI thread exceptions
            Exception ?ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                MessageBox.Show("An unhandled exception occurred: " + ex.Message);
            }
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            ApplicationExit?.Invoke(this, e);
        }
    }

}
