using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHWOverlay {

	class Controller {

		MemoryManager memoryManager;
		public Controller ( ) {
			memoryManager = new MemoryManager("MonsterHunterWorld");
			Console.WriteLine($"Initialized Controller");

			UpdateResolver ur = new UpdateResolver(memoryManager);
			// ur.FindIncreasedHunterBasePointer("Lidian");
			// ur.FindIncreasedInstanceBasePointer("Lidian");
			// ur.FindIncreasedMonsterBasePointer();
		}
		/**
		public void PrintActiveHunterData() {
			monster_data = MonsterHunterWorld.exe+506AE60
				A2C -> eax
				
				CF0
				CF4
				CF8
				CFC

				D00


			player_data 0x80 0x7628      base health 
				                    0x60 max health
				             0x7630 0x64 current health
				                    0x13C current stamina
				                    0x144 max stamina
				                    0x14C eat timer
				             0x7D20 debuffs


				12278   1D60C max timer

				MonsterHunterWorld.exe+223EDC0 - 48 8D 05 61BEFD02     - lea rax,[MonsterHunterWorld.exe+521AC28] { (8A68FD00) }

	MonsterHunterWorld.exe+223ECFF - 80 79 2C 00           - cmp byte ptr [rcx+2C],00 { 0 }


		}
	*/
		public String ReadSessionInfo ( ) {
			String r = "";
            for (Int64 i = 0; i < 4; i++) {
				String PartyMemberName = memoryManager.ReadString(  new RelativeMultiLevelPointer(Data.InstanceBasePointer, 0x68, -0x22B7 + i * 0x1C0), 32);
                UInt16 HR              = memoryManager.Read<UInt16>(new RelativeMultiLevelPointer(Data.InstanceBasePointer, 0x68, -0x22B7 + i * 0x1C0 + 0x27));
                UInt16 MR              = memoryManager.Read<UInt16>(new RelativeMultiLevelPointer(Data.InstanceBasePointer, 0x68, -0x22B7 + i * 0x1C0 + 0x29));
                Byte playerWeapon      = memoryManager.Read<Byte>(  new RelativeMultiLevelPointer(Data.InstanceBasePointer, 0x68, -0x22B7 + i * 0x1C0 + 0x33));

				UInt32 playerDamage =  memoryManager.Read<UInt32>(  new RelativeMultiLevelPointer(Data.InstanceBasePointer, 0x258, 0x38, 0x450, 0x8, 0x48 + i * 0x2A0));

				if ( playerDamage > 0 )
					r += $"{PartyMemberName} <{playerWeapon}> ({MR} | {HR}): {playerDamage}\n";
            }
			return r;
        }

		public Hunter ReadHunter ( UInt64 slot ) {
			UInt64 address = memoryManager.Read<UInt64>(new RelativeMultiLevelPointer(Data.HunterBasePointer, 0xA8)) + slot * Data.HunterSize;
			return new Hunter(memoryManager, address);
		}
		public Monster ReadMonster ( UInt64 slot ) {
			UInt64 address = memoryManager.Read<UInt64>(new RelativeMultiLevelPointer(Data.MonsterBasePointer, 0xE58 + (Int64)slot*0x50)); // TODO extract
			return new Monster(memoryManager, address);
		}
	}

	class UpdateResolver {

		MemoryManager memoryManager;

		public UpdateResolver ( MemoryManager memoryManager ) {
			this.memoryManager = memoryManager;
			Console.WriteLine($"Initialized UpdateResolver");
		}

		public void FindIncreasedHunterBasePointer ( String hunter0name ) {
			UInt64 offset = 0;
			while ( true ) {
				UInt64 address = memoryManager.Read<UInt64>(new RelativeMultiLevelPointer(Data.HunterBasePointer + offset, 0xA8));
				String HunterName = memoryManager.ReadString(address + 0x50, 16);
				if ( HunterName != null && HunterName.Equals(hunter0name) ) {
					if ( offset == 0 ) {
						Console.WriteLine($"Hunter base pointer unchanged.");
						return;
					}
					Console.WriteLine($"{offset:X08} : {Data.HunterBasePointer+offset:X08}: {HunterName}");
					return;
				}
				offset += 4;
				if ( offset % 0x1000 == 0 ) {
					Console.WriteLine($"Searching for HunterAddress ({offset})");
				}
			}
			
		}
		public void FindIncreasedInstanceBasePointer ( String huntername ) {
			UInt64 offset = 0;
			while ( true ) {
				String PartyMemberName = memoryManager.ReadString(new RelativeMultiLevelPointer(Data.InstanceBasePointer + offset, 0x68, -0x22B7), 32);
				if ( PartyMemberName != null && PartyMemberName.Equals(huntername) ) {
					if ( offset == 0 ) {
						Console.WriteLine($"Instance base pointer unchanged.");
						return;
					}
					Console.WriteLine($"{offset:X08} : {Data.InstanceBasePointer + offset:X08}: {PartyMemberName}");
					return;
				}
				offset += 4;
				if ( offset % 0x1000 == 0 ) {
					Console.WriteLine($"Searching for InstanceBasePointer ({offset})");
				}
			}
		}
		public void FindIncreasedMonsterBasePointer ( ) {
			UInt64 offset = 0;
			while ( true ) {
				UInt64 address = memoryManager.Read<UInt64>(new RelativeMultiLevelPointer(Data.MonsterBasePointer + offset, 0xE58));
				String emString = memoryManager.ReadString(new MultiLevelPointer(address + 0x2A0, 0x0C), 64);
				if ( emString != null && emString.StartsWith("em\\em") ) {
					if ( offset == 0 ) {
						Console.WriteLine($"Monster base pointer unchanged.");
						return;
					}
					Console.WriteLine($"{offset:X08} : {Data.MonsterBasePointer+offset:X08}: {emString}");
					return;
				}
				offset += 4;
				if ( offset % 0x1000 == 0 ) {
					Console.WriteLine($"Searching for MonsterBasePointer ({offset})");
				}
			}
		}
	}
}
