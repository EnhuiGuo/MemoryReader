using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
 
namespace BlackMemoryLib
{
    struct Matrix4
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;
 
        public float M21;
        public float M22;
        public float M23;
        public float M24;
 
        public float M31;
        public float M32;
        public float M33;
        public float M34;
 
        public float M41;
        public float M42;
        public float M43;
        public float M44;
    }
 
 
 
    public class BlackMemoryApi
    {
        public const uint PROCESS_VM_READ = (0x0010);
        public const uint PROCESS_VM_WRITE = (0x0020);
        public const uint PROCESS_VM_OPERATION = (0x0008);
        public const uint PAGE_READWRITE = 0x0004;
 
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwAccess, bool inherit, int pid);
 
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);
 
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory( IntPtr hProcess, Int64 lpBaseAddress, [In, Out] byte[] lpBuffer, UInt64 dwSize, out IntPtr lpNumberOfBytesRead );
 
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory( IntPtr hProcess, Int64 lpBaseAddress, [In, Out] byte[] lpBuffer, UInt64 dwSize, out IntPtr lpNumberOfBytesWritten);
 
        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, UInt32 dwSize, uint flAllocationType, uint flProtect);
 
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UInt32 dwSize, uint flNewProtect, out uint lpflOldProtect);
    }
 
    public class BlackMemory
    {
        private Process CurProcess;
        private IntPtr ProcessHandle;
        private string ProcessName;
        private int ProcessID;
        public IntPtr BaseModule;
 
        // Destruktor
        ~BlackMemory() { if( ProcessHandle != IntPtr.Zero) BlackMemoryApi.CloseHandle(ProcessHandle); }
 
        // Get Process for work
        public bool AttackProcess( string _ProcessName )
        {
            Process[] Processes = Process.GetProcessesByName(_ProcessName);
            
            if(Processes.Length > 0)
            {
                BaseModule = Processes[0].MainModule.BaseAddress;
                CurProcess = Processes[0];
                ProcessID = Processes[0].Id;
                ProcessName = _ProcessName;
 
                ProcessHandle = BlackMemoryApi.OpenProcess(BlackMemoryApi.PROCESS_VM_READ | BlackMemoryApi.PROCESS_VM_WRITE | BlackMemoryApi.PROCESS_VM_OPERATION, false, ProcessID );
 
                if( ProcessHandle != IntPtr.Zero )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
 
        // If Process attacked
        public bool IsOpen()
        {
            if( ProcessName == string.Empty )
            {
                return false;
            }
            else
            {
                if( AttackProcess(ProcessName) )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
 
 
        #region |- READ MEMORY -|
 
        // Read Int32
        public Int32 ReadInt32(Int64 _lpBaseAddress)
        {
            byte[] Buffer = new byte[4];
            IntPtr ByteRead;
 
            BlackMemoryApi.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, 4, out ByteRead);
 
            return BitConverter.ToInt32(Buffer, 0);
        }
 
        // Read UInt32
        public UInt32 ReadUInt32(Int64 _lpBaseAddress)
        {
            byte[] Buffer = new byte[4];
            IntPtr ByteRead;
 
            BlackMemoryApi.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, 4, out ByteRead);
 
            return BitConverter.ToUInt32(Buffer, 0);
        }
 
        // Read Int64
        public Int64 ReadInt64( Int64 _lpBaseAddress )
        {
            byte[] Buffer = new byte[8];
            IntPtr ByteRead;
 
            BlackMemoryApi.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, 8, out ByteRead);
 
            return BitConverter.ToInt64(Buffer, 0);
        }
 
        // Read UInt64
        public UInt64 ReadUInt64(Int64 _lpBaseAddress)
        {
            byte[] Buffer = new byte[8];
            IntPtr ByteRead;
 
            BlackMemoryApi.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, 8, out ByteRead);
 
            return BitConverter.ToUInt64(Buffer, 0);
        }
 
        // Read Int64
        public float ReadFloat(Int64 _lpBaseAddress)
        {
            byte[] Buffer = new byte[sizeof(float)];
            IntPtr ByteRead;
 
            BlackMemoryApi.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, sizeof(float), out ByteRead);
 
            return BitConverter.ToSingle(Buffer, 0);
        }
 
        // Read String
        public string ReadString( Int64 _lpBaseAddress, UInt64 _Size)
        {
            byte[] Buffer = new byte[_Size];
            IntPtr BytesRead;
 
            BlackMemoryApi.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, _Size, out BytesRead);
 
            return Encoding.UTF8.GetString(Buffer);
        }
 
        // Read Matrix
        public Matrix4 ReadMatrix(Int64 _lpBaseAddress)
        {
            Matrix4 tmp = new Matrix4();
 
            byte[] Buffer = new byte[64];
            IntPtr ByteRead;
 
            BlackMemoryApi.ReadProcessMemory(ProcessHandle, _lpBaseAddress, Buffer, 64, out ByteRead);
            
            tmp.M11 = BitConverter.ToSingle(Buffer, (0 * 4));
            tmp.M12 = BitConverter.ToSingle(Buffer, (1 * 4));
            tmp.M13 = BitConverter.ToSingle(Buffer, (2 * 4));
            tmp.M14 = BitConverter.ToSingle(Buffer, (3 * 4));
 
            tmp.M21 = BitConverter.ToSingle(Buffer, (4 * 4));
            tmp.M22 = BitConverter.ToSingle(Buffer, (5 * 4));
            tmp.M23 = BitConverter.ToSingle(Buffer, (6 * 4));
            tmp.M24 = BitConverter.ToSingle(Buffer, (7 * 4));
 
            tmp.M31 = BitConverter.ToSingle(Buffer, (8 * 4));
            tmp.M32 = BitConverter.ToSingle(Buffer, (9 * 4));
            tmp.M33 = BitConverter.ToSingle(Buffer, (10 * 4));
            tmp.M34 = BitConverter.ToSingle(Buffer, (11 * 4));
 
            tmp.M41 = BitConverter.ToSingle(Buffer, (12 * 4));
            tmp.M42 = BitConverter.ToSingle(Buffer, (13 * 4));
            tmp.M43 = BitConverter.ToSingle(Buffer, (14 * 4));
            tmp.M44 = BitConverter.ToSingle(Buffer, (15 * 4));
 
            return tmp;
        }
 
        #endregion
        
 
        #region |- WRITE MEMORY -|
 
        public void WriteMemory(Int64 MemoryAddress, byte[] Buffer)
        {
            uint oldProtect;
            BlackMemoryApi.VirtualProtectEx(ProcessHandle, (IntPtr)MemoryAddress, (uint)Buffer.Length, BlackMemoryApi.PAGE_READWRITE, out oldProtect);
 
            IntPtr ptrBytesWritten;
            BlackMemoryApi.WriteProcessMemory(ProcessHandle, MemoryAddress, Buffer, (uint)Buffer.Length, out ptrBytesWritten);
        }
 
        public void WriteInt32( Int64 _lpBaseAddress, int _Value )
        {
            byte[] Buffer = BitConverter.GetBytes(_Value);
 
            WriteMemory(_lpBaseAddress, Buffer);
        }
 
        public void WriteUInt32(Int64 _lpBaseAddress, uint _Value)
        {
            byte[] Buffer = BitConverter.GetBytes(_Value);
 
            WriteMemory(_lpBaseAddress, Buffer);
        }
 
        public void WriteFloat(Int64 _lpBaseAddress, float _Value)
        {
            byte[] Buffer = BitConverter.GetBytes(_Value);
 
            WriteMemory(_lpBaseAddress, Buffer);
        }
 
       
        #endregion
    }
}