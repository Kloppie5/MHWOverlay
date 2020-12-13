using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MHWOverlay {
	public class AddressRange {
		public UInt64 Start { get; private set; }
		public UInt64 End { get; private set; }

		public AddressRange( UInt64 start, UInt64 end ) {
			Start = start;
			End = end;
		}
	}
	public class BytePattern {
		public String String { get; private set; }
		public Byte?[] Bytes { get; private set; }

		public BytePattern( String ByteString ) {
			String = ByteString;

			List<Byte?> ByteList = new List<Byte?>();
			var singleByteStrings = ByteString.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			foreach ( var singleByteString in singleByteStrings )
				ByteList.Add(
					Byte.TryParse(singleByteString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out Byte parsedByte)
					? (Byte?) parsedByte
					: null
				);

			Bytes = ByteList.ToArray();
		}
	}

	public abstract class AddressDescription {
		public abstract UInt64 Resolve( MemoryManager mr );
	}
	public class DirectAddress : AddressDescription {
		private UInt64 _address;

		public DirectAddress ( UInt64 address ) {
			_address = address;
		}

		public override UInt64 Resolve (  MemoryManager mr ) {
			return _address;
		}
	}
	public class RelativeAddress : AddressDescription {
		private UInt64 _address;

		public RelativeAddress ( UInt64 address ) {
			_address = address;
		}

		public override UInt64 Resolve( MemoryManager mr ) {
			return mr.BaseAddress + _address;
		}
	}
	public class MultiLevelPointer : AddressDescription {
		private UInt64 _address;
		private Int64[] _offsets;

		public MultiLevelPointer ( UInt64 address, params Int64[] offsets ) {
			_address = address;
			_offsets = offsets;
		}

		public override UInt64 Resolve ( MemoryManager mr ) {
			UInt64 result = _address;
			foreach ( var offset in _offsets )
				result = (UInt64) ((Int64) mr.Read<UInt64>(result) + offset);
			return result;
		}
	}
	public class RelativeMultiLevelPointer : AddressDescription {
		private UInt64 _address;
		private Int64[] _offsets;

		public RelativeMultiLevelPointer ( UInt64 address, params Int64[] offsets ) {
			_address = address;
			_offsets = offsets;
		}

		public override UInt64 Resolve ( MemoryManager mr ) {
			UInt64 result = mr.BaseAddress + _address;
			foreach ( var offset in _offsets )
				result = (UInt64) ((Int64) mr.Read<UInt64>(result) + offset);
			return result;
		}
	}

	public class MemoryManager {
		Process process;
		public UInt64 BaseAddress => (UInt64) process.MainModule.BaseAddress;

		public MemoryManager( String processName ) {
			process = Process.GetProcessesByName(processName).First();
			Console.WriteLine($"Initialized MemoryReader for process {process.Id} ({processName})");
		}

		public struct MEMORY_BASIC_INFORMATION64 {
			public UInt64 BaseAddress;
			public UInt64 AllocationBase;
			public UInt32 AllocationProtect;
			public UInt32 __alignment1;
			public UInt64 RegionSize;
			public UInt32 State;
			public UInt32 Protect;
			public UInt32 Type;
			public UInt32 __alignment2;
		}

		[DllImport("user32.dll")]
		public static extern Boolean SetForegroundWindow( IntPtr hWnd );
		[DllImport("user32.dll")]
		public static extern Int32 SetWindowLong( IntPtr hWnd, Int32 nIndex, Int32 dwNewLong );
		[DllImport("user32.dll", SetLastError = true)]
		public static extern Int32 GetWindowLong( IntPtr hWnd, Int32 nIndex );

		[DllImport("kernel32.dll")]
		public static extern Boolean ReadProcessMemory( IntPtr hProcess, IntPtr lpBaseAddress, Byte[] lpBuffer, Int32 dwSize, ref Int32 lpNumberOfBytesRead );
		[DllImport("kernel32.dll")]
		public static extern Boolean WriteProcessMemory( IntPtr hProcess, IntPtr lpBaseAddress, Byte[] lpBuffer, Int32 dwSize, ref Int32 lpNumberOfBytesWritten );
		[DllImport("kernel32.dll")]
		public static extern Int32 VirtualQueryEx( IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION64 lpBuffer, UInt32 dwLength );

		public List<UInt64> FindPatternAddresses( AddressRange addressRange, BytePattern pattern ) {
			List<UInt64> matchAddresses = new List<UInt64>();

			UInt64 currentAddress = addressRange.Start;

			List<Byte[]> byteArrays = new List<Byte[]>();

			while ( currentAddress < addressRange.End ) {
				if ( VirtualQueryEx(process.Handle, (IntPtr) currentAddress, out MEMORY_BASIC_INFORMATION64 memoryRegion, (UInt32) Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION64))) > 0
					&& memoryRegion.RegionSize > 0
					&& memoryRegion.State == 0x1000 // MEM_COMMIT
					&& (memoryRegion.Protect & 0x20) > 0 ) { // PAGE_EXECUTE_READ

					var regionStartAddress = memoryRegion.BaseAddress;
					if ( addressRange.Start > regionStartAddress )
						regionStartAddress = addressRange.Start;

					var regionEndAddress = memoryRegion.BaseAddress + memoryRegion.RegionSize;
					if ( addressRange.End < regionEndAddress )
						regionEndAddress = addressRange.End;

					UInt64 regionBytesToRead = regionEndAddress - regionStartAddress;
					Byte[] regionBytes = new Byte[regionBytesToRead];

					Int32 lpNumberOfBytesRead = 0;
					ReadProcessMemory(process.Handle, (IntPtr) regionStartAddress, regionBytes, regionBytes.Length, ref lpNumberOfBytesRead);

					byteArrays.Add(regionBytes);

					if ( regionBytes.Length == 0 || pattern.Bytes.Length == 0 || regionBytes.Length < pattern.Bytes.Length )
						continue;

					List<Int32> matchedIndices = new List<Int32>();
					Int32[] longestPrefixSuffices = new Int32[pattern.Bytes.Length];

					GetLongestPrefixSuffices(pattern, ref longestPrefixSuffices);

					Int32 textIndex = 0;
					Int32 patternIndex = 0;

					while ( textIndex < regionBytes.Length ) {
						if ( !pattern.Bytes[patternIndex].HasValue
							|| regionBytes[textIndex] == pattern.Bytes[patternIndex] ) {
							textIndex++;
							patternIndex++;
						}

						if ( patternIndex == pattern.Bytes.Length ) {
							matchedIndices.Add(textIndex - patternIndex);
							patternIndex = longestPrefixSuffices[patternIndex - 1];
						} else if ( textIndex < regionBytes.Length
								&& (pattern.Bytes[patternIndex].HasValue
								&& regionBytes[textIndex] != pattern.Bytes[patternIndex]) ) {
							if ( patternIndex != 0 )
								patternIndex = longestPrefixSuffices[patternIndex - 1];
							else
								textIndex++;
						}
					}

					foreach ( var matchIndex in matchedIndices ) {
						matchAddresses.Add(regionStartAddress + (UInt64) matchIndex);
					}
				}
				currentAddress = memoryRegion.BaseAddress + memoryRegion.RegionSize;
			}

			return matchAddresses;
		}
		public List<UInt64> FindFirstPatternAddresses( AddressRange addressRange, BytePattern pattern ) {
			List<UInt64> matchAddresses = new List<UInt64>();

			UInt64 currentAddress = addressRange.Start;

			List<Byte[]> byteArrays = new List<Byte[]>();

			while ( currentAddress < addressRange.End ) {
				if ( VirtualQueryEx(process.Handle, (IntPtr) currentAddress, out MEMORY_BASIC_INFORMATION64 memoryRegion, (UInt32) Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION64))) > 0
					&& memoryRegion.RegionSize > 0
					&& memoryRegion.State == 0x1000 // MEM_COMMIT
					&& (memoryRegion.Protect & 0x20) > 0 ) { // PAGE_EXECUTE_READ

					var regionStartAddress = memoryRegion.BaseAddress;
					if ( addressRange.Start > regionStartAddress )
						regionStartAddress = addressRange.Start;

					var regionEndAddress = memoryRegion.BaseAddress + memoryRegion.RegionSize;
					if ( addressRange.End < regionEndAddress )
						regionEndAddress = addressRange.End;

					UInt64 regionBytesToRead = regionEndAddress - regionStartAddress;
					Byte[] regionBytes = new Byte[regionBytesToRead];

					Int32 lpNumberOfBytesRead = 0;
					ReadProcessMemory(process.Handle, (IntPtr) regionStartAddress, regionBytes, regionBytes.Length, ref lpNumberOfBytesRead);

					byteArrays.Add(regionBytes);

					if ( regionBytes.Length == 0 || pattern.Bytes.Length == 0 || regionBytes.Length < pattern.Bytes.Length )
						continue;

					List<Int32> matchedIndices = new List<Int32>();
					Int32[] longestPrefixSuffices = new Int32[pattern.Bytes.Length];
					GetLongestPrefixSuffices(pattern, ref longestPrefixSuffices);

					Int32 textIndex = 0;
					Int32 patternIndex = 0;
					while ( textIndex < regionBytes.Length ) {
						if ( !pattern.Bytes[patternIndex].HasValue
							|| regionBytes[textIndex] == pattern.Bytes[patternIndex] ) {
							textIndex++;
							patternIndex++;
						}

						if ( patternIndex == pattern.Bytes.Length ) {
							matchedIndices.Add(textIndex - patternIndex);
							patternIndex = longestPrefixSuffices[patternIndex - 1];
							break;
						} else if ( textIndex < regionBytes.Length
								&& (pattern.Bytes[patternIndex].HasValue
								&& regionBytes[textIndex] != pattern.Bytes[patternIndex]) ) {
							if ( patternIndex != 0 )
								patternIndex = longestPrefixSuffices[patternIndex - 1];
							else
								textIndex++;
						}
					}
					
					if ( matchedIndices.Any() )
						matchAddresses.Add(regionStartAddress + (UInt64) matchedIndices.First());
				}
				currentAddress = memoryRegion.BaseAddress + memoryRegion.RegionSize;
			}

			return matchAddresses;
		}
		static void GetLongestPrefixSuffices( BytePattern pattern, ref Int32[] longestPrefixSuffices ) {
			Int32 patternLength = pattern.Bytes.Length;
			Int32 length = 0;
			Int32 patternIndex = 1;

			longestPrefixSuffices[0] = 0;

			while ( patternIndex < patternLength ) {
				if ( pattern.Bytes[patternIndex] == pattern.Bytes[length] ) {
					length++;
					longestPrefixSuffices[patternIndex] = length;
					patternIndex++;
				} else {
					if ( length == 0 ) {
						longestPrefixSuffices[patternIndex] = 0;
						patternIndex++;
					} else
						length = longestPrefixSuffices[length - 1];
				}
			}
		}

		
		public UInt64 Resolve ( AddressDescription address ) {
			return address.Resolve(this);
		}
		public T Read<T> ( AddressDescription address ) where T : struct {
			return Read<T>(address.Resolve(this));
		}
		public T Read<T>( UInt64 address ) where T : struct {
			Byte[] Bytes = new Byte[Marshal.SizeOf(typeof(T))];

			Int32 lpNumberOfBytesRead = 0;
			ReadProcessMemory(process.Handle, (IntPtr) address, Bytes, Bytes.Length, ref lpNumberOfBytesRead);

			T result;
			GCHandle handle = GCHandle.Alloc(Bytes, GCHandleType.Pinned);

			try {
				result = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
			} finally {
				handle.Free();
			}

			return result;
		}
		public String ReadString ( AddressDescription address, UInt32 length ) {
			return ReadString(address.Resolve(this), length);
		}
		public String ReadString ( UInt64 address, UInt32 length ) {
			Byte[] Bytes = new Byte[length];

			Int32 lpNumberOfBytesRead = 0;
			ReadProcessMemory(process.Handle, (IntPtr) address, Bytes, Bytes.Length, ref lpNumberOfBytesRead);

			Int32 nullTerminatorIndex = Array.FindIndex(Bytes, ( Byte b ) => b == 0);
			if ( nullTerminatorIndex >= 0 ) {
				Array.Resize(ref Bytes, nullTerminatorIndex);
				return Encoding.UTF8.GetString(Bytes);
			}

			return null;
		}
		public void Write<T> ( AddressDescription address, T t ) where T : struct {
			Write<T>(address.Resolve(this), t);
		}
		public void Write<T>( UInt64 address, T t ) where T : struct {
			Int32 size = Marshal.SizeOf(t);
			Byte[] Bytes = new Byte[size];

			IntPtr ptr = Marshal.AllocHGlobal(size);
			Marshal.StructureToPtr(t, ptr, true);
			Marshal.Copy(ptr, Bytes, 0, size);
			Marshal.FreeHGlobal(ptr);

			Int32 lpNumberOfBytesWritten = 0;
			WriteProcessMemory(process.Handle, (IntPtr) address, Bytes, Bytes.Length, ref lpNumberOfBytesWritten);

		}
	}
}
