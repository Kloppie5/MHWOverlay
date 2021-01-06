using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MHWOverlay {
	partial class Data {
		public static String version = "421470";
	}

	static class Program {
		[STAThread]
		static void Main() {
			Console.WriteLine($"Initializing MHWOverlay({Data.version})");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Overlay overlay = new Overlay();
			Application.Run(overlay);
		}
	}
}
