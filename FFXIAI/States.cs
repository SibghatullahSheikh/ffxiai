namespace FFXIAI
{

    enum ProcessAccessFlags
    {
        PROCESS_ALL_ACCESS = 0x1F0FFF,
        PROCESS_CREATE_THREAD = 0x2,
        PROCESS_DUP_HANDLE = 0x40,
        PROCESS_QUERY_INFORMATION = 0x400,
        PROCESS_SET_INFORMATION = 0x200,
        PROCESS_TERMINATE = 0x1,
        PROCESS_VM_OPERATION = 0x8,
        PROCESS_VM_READ = 0x10,
        PROCESS_VM_WRITE = 0x20,
        SYNCHRONIZE = 0x100000
    }

    
}