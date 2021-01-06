using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWOverlay {
	partial class Data {
		public static UInt64 HunterBasePointer = 0x5073E80;
		public static UInt64 InstanceBasePointer = 0x5224BF8;
		public static UInt64 HunterSize = 0x27E9F0;
	}

	class Hunter {

		MemoryManager memoryManager;
		UInt64 address;

		public Hunter ( MemoryManager memoryManager, UInt64 address ) {
			this.memoryManager = memoryManager;
			this.address = address;
		}

		public void PrintGuidingLands ( ) {
			// UInt32 Playtime = memoryManager.Read<UInt32>(address + 0x17FDE8);   // 17FDE8 Playtime
			// UInt32 Quests = memoryManager.Read<UInt32>(address +	0x17FDEC);     // 17FDEC Quests
			// UInt32 Tracks = memoryManager.Read<UInt32>(address + 0x17FDF0);     // 17FDF0 Tracks

			UInt32 AncientForest  = memoryManager.Read<UInt32>(address + 0x27B928); // 0x27B928 Forest
			UInt32 WildspireWaste = memoryManager.Read<UInt32>(address + 0x27B92C); // 0x27B92C Wildspire
			UInt32 CoralHighlands = memoryManager.Read<UInt32>(address + 0x27B930); // 0x27B930 Coral
			UInt32 RottenVale     = memoryManager.Read<UInt32>(address + 0x27B934); // 0x27B934 Rotten
			UInt32 Volcanic       = memoryManager.Read<UInt32>(address + 0x27B938); // 0x27B938 Volcano
			UInt32 Tundra         = memoryManager.Read<UInt32>(address + 0x27B93C); // 0x27B93C Tundra
			UInt32 Max            = memoryManager.Read<UInt32>(address + 0x27B948); // 0x27B948 Max
			UInt32 Sum = AncientForest + WildspireWaste + CoralHighlands + RottenVale + Volcanic + Tundra;

			Console.WriteLine(
				$"Guiding Lands {Sum / 10000.0f + 6:00.0000}/{Max / 10000.0f + 12:00.0000}\n" +
				$"  Ancient Forest:  {AncientForest / 10000.0f + 1:0.0000}\n" +
				$"  Wildspire Waste: {WildspireWaste / 10000.0f + 1:0.0000}\n" +
				$"  Coral Highlands: {CoralHighlands / 10000.0f + 1:0.0000}\n" +
				$"  Rotten Vale:     {RottenVale / 10000.0f + 1:0.0000}\n" +
				$"  Volcanic:        {Volcanic / 10000.0f + 1:0.0000}\n" +
				$"  Tundra:          {Tundra / 10000.0f + 1:0.0000}\n"
			);
		}

		public override String ToString ( ) {
			String HunterName     = memoryManager.ReadString(address + 0x50, 16); // 50-90 Hunter Name
			UInt32 HR             = memoryManager.Read<UInt32>(address + 0x90);   // 90-94 HR
			UInt32 Zenny          = memoryManager.Read<UInt32>(address + 0x94);   // 94-98 Zenny
			UInt32 ResearchPoints = memoryManager.Read<UInt32>(address + 0x98);   // 98-9C Research Points
			UInt32 HRExperience   = memoryManager.Read<UInt32>(address + 0x9C);   // 9C-A0 HR Experience
			UInt32 PlayTime       = memoryManager.Read<UInt32>(address + 0xA0);   // A0-A4 Playtime (seconds)
																			   // A4-D4 <<<<
			UInt32 MR             = memoryManager.Read<UInt32>(address + 0xD4);   // D4-D8 MR
			UInt32 MRExperience   = memoryManager.Read<UInt32>(address + 0xD8);   // D8-DC MR Experience

			UInt32 steam = memoryManager.Read<UInt32>(address + 0x102FE0);

			return $"Hunter @{address:X08}\n" +
				   $"  Name:            {HunterName}\n" +
				   $"  HR:              {HRExperience} ({HR})\n" +
				   $"  MR:              {MRExperience} ({MR})\n" +
				   $"  Zenny:           {Zenny}\n" +
				   $"  Research Points: {ResearchPoints}\n" +
				   $"  Playtime:        {PlayTime / 3600:d02}:{PlayTime / 60 % 60:d02}:{PlayTime % 60:d02}";
		}
	}
}
