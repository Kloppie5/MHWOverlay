using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MHWOverlay {
	public partial class Overlay : Form {

		Model model = null;

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow( IntPtr hWnd );
		[DllImport("user32.dll")]
		public static extern int SetWindowLong( IntPtr hWnd, int nIndex, int dwNewLong );
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetWindowLong( IntPtr hWnd, int nIndex );

		public Overlay() {
			Rectangle rect = Screen.PrimaryScreen.Bounds;
			TransparencyKey = Color.Turquoise;
			BackColor = Color.Turquoise;
			FormBorderStyle = FormBorderStyle.None;
			StartPosition = FormStartPosition.Manual;
			DoubleBuffered = true;
			Location = new Point(rect.Left, rect.Top);
			Size = rect.Size;
			TopMost = true;
			SetForegroundWindow(Handle);

			model = new Model();
			model.PropertyChanged += (memoryManager, propertyName) => {
				Invalidate();
			};
			Console.WriteLine($"Initialized Overlay");
		}

		Boolean printparts = false;
		protected override void OnPaint ( PaintEventArgs e ) {
			base.OnPaint(e);
			if ( model.session != null ) {
				e.Graphics.DrawString(
						model.session,
					new Font("Consolas", 8),
					new SolidBrush(Color.White),
					400f,
					400f,
					new StringFormat() { }
				);
			}
			if ( model.monster0 != null ) {
				e.Graphics.DrawString(
						model.monster0.ToString(),
					new Font("Consolas", 8),
					new SolidBrush(Color.White),
					700f,
					100f,
					new StringFormat() { }
				);
				if (printparts)
					e.Graphics.DrawString(
							model.monster0.PartsToString(),
						new Font("Consolas", 8),
						new SolidBrush(Color.White),
						700f,
						150f,
						new StringFormat() { }
					);
			}
			if ( model.monster1 != null ) {
				e.Graphics.DrawString(
						model.monster1.ToString(),
					new Font("Consolas", 8),
					new SolidBrush(Color.White),
					900f,
					100f,
					new StringFormat() { }
				);
				if (printparts)
					e.Graphics.DrawString(
							model.monster1.PartsToString(),
						new Font("Consolas", 8),
						new SolidBrush(Color.White),
						900f,
						150f,
						new StringFormat() { }
					);
			}
			if ( model.monster2 != null ) {
				e.Graphics.DrawString(
						model.monster2.ToString(),
					new Font("Consolas", 8),
					new SolidBrush(Color.White),
					1100f,
					100f,
					new StringFormat() { }
				);
				if (printparts)
					e.Graphics.DrawString(
							model.monster2.PartsToString(),
						new Font("Consolas", 8),
						new SolidBrush(Color.White),
						1100f,
						150f,
						new StringFormat() { }
					);
			}
		}

		protected override void OnLoad ( EventArgs e ) {
			base.OnLoad(e);
			var style = GetWindowLong(Handle, -20); // GWL_EXSTYLE 
			SetWindowLong(Handle, -20, style | 0x80020); // WS_EX_LAYERED, WS_EX_TRANSPARENT
		}
	}
}
