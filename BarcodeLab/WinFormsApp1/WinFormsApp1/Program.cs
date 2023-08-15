using System;
using System.Diagnostics;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace WinFormsApp1
{

	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			var logger = new Logger();
			logger.AppStarted();
			try
			{
				RunTests();

				var form = new Form1();
				InjectDarkModeStyles(form);
				{
					// show on 1st monitor
					form.StartPosition = FormStartPosition.CenterScreen;
					form.Location = new System.Drawing.Point(0, 0);
					// maximized
					form.WindowState = FormWindowState.Maximized;
					// bring to front
					form.TopMost = true;
				}

				{
					Trace.AutoFlush = true;
					Console.WriteLine("Console World");
					Trace.WriteLine("Trace World");
					// write to the event log
					EventLog.WriteEntry("Application", "Application started", EventLogEntryType.Information, 1001);
				}

				Application.Run(form);
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}
			finally
			{
				logger.AppStopped();
			}
		}

		private static void InjectDarkModeStyles(Form1 form)
		{
			var darkMode = new DarkMode();
			darkMode.ApplyTo(form);
		}

		private static void RunTests()
		{
			var tests = new Tests.BarcodeDecoderTests();
			tests.TestImageAsBarcode();
		}
	}

	internal class DarkMode
	{

		public void ApplyTo(Control form)
		{
			form.BackColor = Color.FromArgb(255, 31, 31, 31);
			form.ForeColor = Color.FromArgb(255, 180, 180, 180);

			// style every control
			foreach (var control in form.Controls)
			{
				if (control is Control c)
				{
					ApplyTo(c);
				}
			}
		}
	}
}