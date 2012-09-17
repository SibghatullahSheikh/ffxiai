using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FFXIAI
{
    public class MemoryManager
    {
        // custom stuff
        public IntPtr process_handle;

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, ref uint lpBuffer, int dwSize, int lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        private static extern Boolean WriteProcessMemory(IntPtr hProcess, uint lpBaseAddress, byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hObject);

        public void init(int processid)
        {
            //@TODO dispose of this!
            process_handle = OpenProcess(ProcessAccessFlags.PROCESS_VM_READ | ProcessAccessFlags.PROCESS_VM_WRITE | ProcessAccessFlags.PROCESS_VM_OPERATION, false, (uint)processid);
        }

        public void cleanup()
        {
            CloseHandle(process_handle);
        }

        public void debug_byte_array(byte[] b)
        {
            Debugger.Log(0, null, "Writing Location: with " + b.Length + " bytes." + "\n");
            for (int i = 0; i < b.Length; ++i)
            {
                Debugger.Log(0, null, "b[" + i + "] - " + b[i] + "\n");
            }
        }

        public void write_memory(uint address, byte[] Buffer)
        {
            IntPtr num_bytes = IntPtr.Zero;
            WriteProcessMemory(process_handle, address, Buffer, Buffer.Length, num_bytes);
        }

        public void read_memory(uint memory_address, ref uint return_value, int size)
        {
            ReadProcessMemory(process_handle, memory_address, ref return_value, size, 0);
        }

        public void read_int(uint memory_address, ref uint return_value)
        {
            read_memory(memory_address, ref return_value, 4);
        }

        public void read_short(uint memory_address, ref uint return_value)
        {
            read_memory(memory_address, ref return_value, 2);
        }

        public void read_byte(uint memory_address, ref uint return_value)
        {  
            read_memory(memory_address, ref return_value, 1);
        }

    }

}