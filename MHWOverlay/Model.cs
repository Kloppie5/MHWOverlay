using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace MHWOverlay {
	class Model : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged ( String propertyName ) {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

		Controller controller;

		public Model ( ) {
			controller = new Controller();
			Console.WriteLine($"Initialized Model");

			Timer timer1 = new Timer {
				Interval = 5000
			};
			timer1.Enabled = true;
			timer1.Elapsed += new ElapsedEventHandler(Update);
		}

		public void Update ( Object source, EventArgs e ) {
			UpdateHunterInfo();
			UpdateMonsterInfo();
			UpdateSessionInfo();
		}

		public String session;
		private void UpdateSessionInfo ( ) {
			session = controller.ReadSessionInfo();
		}

		public Hunter hunter0;
		private void UpdateHunterInfo ( ) {
			hunter0 = controller.ReadHunter(0);
		}

		public Monster monster0;
		public Monster monster1;
		public Monster monster2;
		private void UpdateMonsterInfo ( ) {
			monster0 = controller.ReadMonster(0);
			monster1 = controller.ReadMonster(1);
			monster2 = controller.ReadMonster(2);
			RaisePropertyChanged("MonsterInfo");
		}
	}
}
