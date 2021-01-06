using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWOverlay {
	partial class Data {
		public static UInt64 MonsterBasePointer = 0x5074180;

		public static Dictionary<String, String> emTranslate = new Dictionary<String, String>() {
			{ "em\\em001\\00\\mod\\em001_00", "Rathian" },
			{ "em\\em001\\01\\mod\\em001_01", "Pink Rathian" },
			{ "em\\em001\\02\\mod\\em001_02", "Gold Rathian" },
			{ "em\\em002\\00\\mod\\em002_00", "Rathalos" },
			{ "em\\em002\\01\\mod\\em002_01", "Azure Rathalos" },
			{ "em\\em002\\02\\mod\\em002_02", "Silver Rathalos" },
			{ "em\\em007\\00\\mod\\em007_00", "Diablos" },
			{ "em\\em007\\01\\mod\\em007_01", "Black Diablos" },
			{ "em\\em011\\00\\mod\\em011_00", "Kirin" },
			{ "em\\em013\\00\\mod\\em013_00", "Fatalis" },
			{ "em\\em018\\00\\mod\\em018_00", "Yian Garuga" },
			{ "em\\em018\\05\\mod\\em018_05", "Scarred Yian Garuga" },
			{ "em\\em023\\00\\mod\\em023_00", "Rajang" },
			{ "em\\em023\\05\\mod\\em023_05", "Furious Rajang" },
			{ "em\\em024\\00\\mod\\em024_00", "Kushala Daora" },
			{ "em\\em026\\00\\mod\\em026_00", "Lunastra" },
			{ "em\\em027\\00\\mod\\em027_00", "Teostra" },
			{ "em\\em032\\00\\mod\\em032_00", "Tigrex" },
			{ "em\\em032\\01\\mod\\em032_01", "Brute Tigrex" },
			{ "em\\em036\\00\\mod\\em036_00", "Lavasioth" },
			{ "em\\em037\\00\\mod\\em037_00", "Nargacuga" },
			{ "em\\em042\\00\\mod\\em042_00", "Barioth" },
			{ "em\\em042\\05\\mod\\em042_05", "Frostfang Barioth" },
			{ "em\\em043\\00\\mod\\em043_00", "Deviljho" },
			{ "em\\em043\\05\\mod\\em043_05", "Savage Deviljho" },
			{ "em\\em044\\00\\mod\\em044_00", "Barroth" },
			{ "em\\em045\\00\\mod\\em045_00", "Uragaan" },
			{ "em\\em050\\00\\mod\\em050_00", "Alatreon" },
			{ "em\\em057\\00\\mod\\em057_00", "Zinogre" },
			{ "em\\em057\\01\\mod\\em057_01", "Stygian Zinogre" },
			{ "em\\em063\\00\\mod\\em063_00", "Brachydios" },
			{ "em\\em063\\05\\mod\\em063_05", "Raging Brachydios" },
			{ "em\\em080\\00\\mod\\em080_00", "Glavenus" },
			{ "em\\em080\\01\\mod\\em080_01", "Acidic Glavenus" },
			{ "em\\em100\\00\\mod\\em100_00", "Anjanath" },
			{ "em\\em100\\01\\mod\\em100_01", "Fulgar Anjanath" },
			{ "em\\em101\\00\\mod\\em101_00", "Great Jagras" },
			{ "em\\em102\\00\\mod\\em102_00", "Pukei-Pukei" },
			{ "em\\em102\\01\\mod\\em102_01", "Coral Pukei-Pukei" },
			{ "em\\em103\\00\\mod\\em103_00", "Nergigante" },
			{ "em\\em103\\05\\mod\\em103_05", "Ruiner Nergigante" },
			{ "em\\em104\\00\\mod\\em104_00", "Safi'jiiva" },
			{ "em\\em105\\00\\mod\\em105_00", "Xeno'jiiva" },
			{ "em\\em106\\00\\mod\\em106_00", "Zorah Magdaros" },
			{ "em\\em107\\00\\mod\\em107_00", "Kulu-Ya-Ku" },
			{ "em\\em108\\00\\mod\\em108_00", "Jyuratodus" },
			{ "em\\em109\\00\\mod\\em109_00", "Tobi-Kadachi" },
			{ "em\\em109\\01\\mod\\em109_01", "Viper Tobi-Kadachi" },
			{ "em\\em110\\00\\mod\\em110_00", "Paolumu" },
			{ "em\\em110\\01\\mod\\em110_01", "Nightshade Paolumu" },
			{ "em\\em111\\00\\mod\\em111_00", "Legiana" },
			{ "em\\em111\\05\\mod\\em111_05", "Shrieking Legiana" },
			{ "em\\em112\\00\\mod\\em112_00", "Great Girros" },
			{ "em\\em113\\00\\mod\\em113_00", "Odogaron" },
			{ "em\\em113\\01\\mod\\em113_01", "Ebony Odogaron" },
			{ "em\\em114\\00\\mod\\em114_00", "Radobaan" },
			{ "em\\em115\\00\\mod\\em115_00", "Vaal Hazak" },
			{ "em\\em115\\05\\mod\\em115_05", "Blackveil Vaal Hazak" },
			{ "em\\em116\\00\\mod\\em116_00", "Dodogama" },
			{ "em\\em117\\00\\mod\\em117_00", "Kulve Taroth" },
			{ "em\\em118\\00\\mod\\em118_00", "Bazelgeuse" },
			{ "em\\em118\\05\\mod\\em118_05", "Seething Bazelgeuse" },
			{ "em\\em120\\00\\mod\\em120_00", "TziTzi-Ya-Ku" },
			{ "em\\em121\\00\\mod\\em121_00", "Behemoth" },
			{ "em\\em122\\00\\mod\\em122_00", "Beotodus" },
			{ "em\\em123\\00\\mod\\em123_00", "Banbaro" },
			{ "em\\em124\\00\\mod\\em124_00", "Velkhana" },
			{ "em\\em125\\00\\mod\\em125_00", "Namielle" },
			{ "em\\em126\\00\\mod\\em126_00", "Shara Ishvalda" },
			{ "em\\em127\\00\\mod\\em127_00", "Leshen" },
			{ "em\\em127\\01\\mod\\em127_01", "Ancient Leshen" },
		};
	}

	class Monster {

		MemoryManager memoryManager;
		UInt64 address;

		public Monster ( MemoryManager memoryManager, UInt64 address ) {
			this.memoryManager = memoryManager;
			this.address = address;
		}

		public void Farm ( ) {
			if ( address == 0 )
				return;
			Break();
			UInt64 HPComponent = memoryManager.Read<UInt64>(address + 0x7670);
			Single MAX_HP = memoryManager.Read<Single>(HPComponent + 0x60);
			Single HP = memoryManager.Read<Single>(HPComponent + 0x64);
			if ( HP > 1000.0f )
				SetHealth(1.0f);
		}

		public void Kill ( ) {
			if ( address == 0 )
				return;
			UInt64 HPComponent = memoryManager.Read<UInt64>(address + 0x7670);
			Single HP = memoryManager.Read<Single>(HPComponent + 0x64);
			if ( HP > 1.0f )
				memoryManager.Write(HPComponent + 0x64, 1.0f);
		}

		public void SetHealth ( Single health ) {
			if ( address == 0 )
				return;
			UInt64 HPComponent = memoryManager.Read<UInt64>(address + 0x7670);
			memoryManager.Write(HPComponent + 0x64, health);
		}

		public void Break ( ) {
			if ( address == 0 )
				return;

			for ( // 14538-164B8
				UInt64 PartAddress = address + 0x14538;
				PartAddress < address + 0x164B8;
				PartAddress += 0x1F8
			) {
				Single PartHPMax = memoryManager.Read<Single>(PartAddress + 0xC);
				Single PartHPCurr = memoryManager.Read<Single>(PartAddress + 0x10);

				if ( PartHPCurr > 0.5f )
					memoryManager.Write(PartAddress + 0x10, 1.0f);
			}

			for ( // 164C0-16C40
				UInt64 PartAddress = address + 0x164C0;
				PartAddress < address + 0x16C40;
				PartAddress += 0x78
			) {
				Single PartHPMax = memoryManager.Read<Single>(PartAddress + 0xC);
				Single PartHPCurr = memoryManager.Read<Single>(PartAddress + 0x10);

				if ( PartHPCurr > 0.5f )
					memoryManager.Write(PartAddress + 0x10, 1.0f);
			}

			for ( // 16C48-173C8
				UInt64 PartAddress = address + 0x16C48;
				PartAddress < address + 0x173C8;
				PartAddress += 0x78
			) {
				Single PartHPMax = memoryManager.Read<Single>(PartAddress + 0xC);
				Single PartHPCurr = memoryManager.Read<Single>(PartAddress + 0x10);

				if ( PartHPCurr > 0.5f )
					memoryManager.Write(PartAddress + 0x10, 1.0f);
			}
		}

		public String PartsToString ( ) {
			if ( address == 0 )
				return "";

			String r = "";

			for ( // 14538-164B8
				UInt64 PartAddress = address + 0x14538;
				PartAddress < address + 0x164B8;
				PartAddress += 0x1F8
			) {
				Single PartHPMax = memoryManager.Read<Single>(PartAddress + 0xC);
				Single PartHPCurr = memoryManager.Read<Single>(PartAddress + 0x10);

				r += $"    Part<1> @{PartAddress:X08}: {PartHPCurr}/{PartHPMax}\n";
			}

			for ( // 164C0-16C40
				UInt64 PartAddress = address + 0x164C0;
				PartAddress < address + 0x16C40;
				PartAddress += 0x78
			) {
				Single PartHPMax = memoryManager.Read<Single>(PartAddress + 0xC);
				Single PartHPCurr = memoryManager.Read<Single>(PartAddress + 0x10);

				r += $"    Part<2> @{PartAddress:X08}: {PartHPCurr}/{PartHPMax}\n";
			}

			for ( // 16C48-173C8
				UInt64 PartAddress = address + 0x16C48;
				PartAddress < address + 0x173C8;
				PartAddress += 0x78
			) {
				Single PartHPMax = memoryManager.Read<Single>(PartAddress + 0xC);
				Single PartHPCurr = memoryManager.Read<Single>(PartAddress + 0x10);

				r += $"    Part<3> @{PartAddress:X08}: {PartHPCurr}/{PartHPMax}\n";
			}

			return r;
		}

		public override String ToString ( ) {
			if ( address == 0 )
				return "No Monster";

			String r = $"Monster @{address:X08}\n";

			String emString = memoryManager.ReadString(new MultiLevelPointer(address + 0x2A0, 0x0C), 64);
			String name = Data.emTranslate.ContainsKey(emString) ? Data.emTranslate[emString] : emString;
			r += $"  Name: {name}\n";

			UInt64 HPComponent = memoryManager.Read<UInt64>(address + 0x7670);
				Single MAX_HP = memoryManager.Read<Single>(HPComponent + 0x60);
				Single HP = memoryManager.Read<Single>(HPComponent + 0x64);
			r += $"  HP:   {HP}/{MAX_HP} ({HP / MAX_HP:P})\n";

			Single size = memoryManager.Read<Single>(address + 0x188);
			Single sizeModifier = memoryManager.Read<Single>(address + 0x7730);
			r += $"  Size: {sizeModifier}|{size}\n";

			// UInt64 alatreon_element = memoryManager.Read<UInt64>(address + 0x20920);

			return r;
		}
	}
}
