using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteLua
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int LuaCSFunction(IntPtr L);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int LuaKFunction(IntPtr L, LuaStatus status, IntPtr ctx);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr LuaReader(IntPtr L, IntPtr ud, out nuint size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int LuaWriter(IntPtr L, IntPtr p, nuint size, IntPtr ud);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr LuaAlloc(IntPtr ud, IntPtr ptr, nuint osize, nuint nsize);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LuaWarnFunction(IntPtr ud, IntPtr msg, int tocont);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LuaHook(IntPtr L, IntPtr debug);

    public enum LuaStatus : int
    {
        Ok              = 0,
        Yield           = 1,
        RuntimeError    = 2,
        SyntaxError     = 3,
        MemoryError     = 4,
        Error           = 5,
        FileError       = 6,
    }

    public enum LuaType : int
    {
        None            = -1,
        Nil             = 0,
        Boolean         = 1,
        LightUserdata   = 2,
        Number          = 3,
        String          = 4,
        Table           = 5,
        Function        = 6,
        Userdata        = 7,
        Thread          = 8,
    }

    public enum LuaOp : int
    {
        Add = 0,    // LUA_OPADD
        Sub = 1,    // LUA_OPSUB
        Mul = 2,    // LUA_OPMUL
        Div = 3,    // LUA_OPDIV
        Mod = 4,    // LUA_OPMOD
        Pow = 5,    // LUA_OPPOW
        Unm = 6,    // LUA_OPUNM

        Idiv = 7,   // LUA_OPIDIV

        Band = 8,   // LUA_OPBAND
        Bor = 9,    // LUA_OPBOR
        Bxor = 10,  // LUA_OPBXOR
        Bnot = 11,  // LUA_OPBNOT

        Shl = 12,   // LUA_OPSHL
        Shr = 13    // LUA_OPSHR
    }

    public enum LuaCompareOp
    {
        Eq = 0, // LUA_OPEQ
        Lt = 1, // LUA_OPLT
        Le = 2  // LUA_OPLE
    }

    public enum LuaGC : int
    {
        Stop        = 0,        // LUA_GCSTOP
        Restart     = 1,        // LUA_GCRESTART
        Collect     = 2,        // LUA_GCCOLLECT
        Count       = 3,        // LUA_GCCOUNT
        CountB      = 4,        // LUA_GCCOUNTB
        Step        = 5,        // LUA_GCSTEP
        IsRunning   = 6,        // LUA_GCISRUNNING
        Gen         = 7,        // LUA_GCGEN
        Inc         = 8,        // LUA_GCINC
        Param       = 9,        // LUA_GCPARAM
    }

    public enum LuaGCParam : int
    {
        // generational mode
        PMinorMul   = 0,        // LUA_GCPMINORMUL
        PMajorMinor = 1,        // LUA_GCPMAJORMINOR
        PMinorMajor = 2,        // LUA_GCPMINORMAJOR

        // incremental mode
        Pause       = 3,        // LUA_GCPPAUSE
        StepMul     = 4,        // LUA_GCPSTEPMUL
        StepSize    = 5,        // LUA_GCPSTEPSIZE
    }
    
    public enum LuaHookEvent
    {
        Call = 0,      // LUA_HOOKCALL
        Return = 1,    // LUA_HOOKRET
        Line = 2,      // LUA_HOOKLINE
        Count = 3,     // LUA_HOOKCOUNT
        TailCall = 4   // LUA_HOOKTAILCALL
    }

    [Flags]
    public enum LuaHookMask
    {
        None = 0,
        Call = 1 << LuaHookEvent.Call,      // LUA_MASKCALL
        Return = 1 << LuaHookEvent.Return,  // LUA_MASKRET
        Line = 1 << LuaHookEvent.Line,      // LUA_MASKLINE
        Count = 1 << LuaHookEvent.Count,    // LUA_MASKCOUNT
    }

    public static class LuaConst
    {
        // common
        public const int MultiRet = -1;
        public const int RegistryIndex = -(int.MaxValue / 2 + 1000);
        public const int MinStack = 20;
        
        // predefined values in the registry
        public const int Globals = 2;
        public const int MainThread = 3;
        public const int Last = 3;
        
        // ref
        public const int NoRef = -2;
        public const int RefNil = -1;

        // version
        public const int Major = 5;
        public const int Minor = 5;
        public const int Release = 1;

        // global table
        public const string GlobalName = "_G";
        
        // key, in the registry, for table of loaded modules
        public const string LoadedTable = "_LOADED";
        
        // key, in the registry, for table of preloaded loaders
        public const string PreloadTable = "_PRELOAD";
        
        public const int VersionNum = Major * 100 + Minor;
        public const int VersionReleaseNum = VersionNum * 100 + Release;
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct LuaLReg
    {
        public static LuaLReg Null = new LuaLReg(null, null);

        public readonly string name;
        public readonly LuaCSFunction func;

        public LuaLReg(string name, LuaCSFunction func)
        {
            this.name = name;
            this.func = func;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LuaDebug
    {
        public LuaHookEvent evt;

        public IntPtr name;
        public IntPtr nameWhat;
        public IntPtr what;
        public IntPtr source;
        public nuint sourceLen;

        public int currentLine;
        public int lineDefined;
        public int lastLineDefined;

        public byte numberUpValues;
        public byte numberParams;
        public byte isVarArg;
        public byte extraArgs;
        public byte isTailCall;

        public int firstTransfer;
        public int numberTransfer;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)] // LUA_IDSIZE=60
        public byte[] shortSrc;

        public IntPtr callInfo; // 不用它
    }

    public static class LuaAPI
    {
#if (UNITY_IPHONE || UNITY_TVOS || UNITY_WEBGL || UNITY_SWITCH) && !UNITY_EDITOR
        private const string LuaDLL = "__Internal";
#else
        private const string LuaDLL = "lua";
#endif

        private const int StackAllocLimit = 256;
        private const byte CStringEnd = (byte)'\0';

        private static unsafe string GetString(IntPtr strPtr, nuint length)
        {
            if (strPtr == IntPtr.Zero)
            {
                return null;
            }
            
            if (length == 0)
            {
                return string.Empty;
            }
            
            if (length <= StackAllocLimit)
            {
                Span<byte> buffer = stackalloc byte[(int)length];
                fixed (byte* dst = buffer)
                {
                    Buffer.MemoryCopy((void*)strPtr, dst, length, length);
                }
                return Encoding.UTF8.GetString(buffer);
            }
            else
            {
                var buffer = new byte[(int)length];
                Marshal.Copy(strPtr, buffer, 0, (int)length);
                return Encoding.UTF8.GetString(buffer);
            }
        }
        
#pragma warning disable IDE1006

        #region lua.h

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int lua_upvalueindex(int i) => LuaConst.RegistryIndex - i;

        #region state manipulation

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_newstate(LuaAlloc f, IntPtr ud, uint seed);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_close(IntPtr L);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_newthread(IntPtr L);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_closethread(IntPtr L, IntPtr from);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaCSFunction lua_atpanic(IntPtr L, LuaCSFunction panicf);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern double lua_version(IntPtr L);

        #endregion
        
        #region basic stack manipulation

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_absindex(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_gettop(IntPtr L);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_settop(IntPtr L, int idx);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushvalue(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rotate(IntPtr L, int idx, int n);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_copy(IntPtr L, int fromidx, int toidx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_checkstack(IntPtr L, int n);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_xmove(IntPtr from, IntPtr to, int n);

        #endregion
        
        #region access functions (stack -> C)

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_isnumber(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_isstring(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_iscfunction(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_isinteger(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_isuserdata(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_type(IntPtr L, int idx);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_typename")]
        private static extern IntPtr lua_typename_raw(IntPtr L, LuaType tp);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string lua_typename(IntPtr L, LuaType tp)
        {
            var ptr = lua_typename_raw(L, tp);
            return Marshal.PtrToStringUTF8(ptr);
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern double lua_tonumberx(IntPtr L, int idx, out bool isnum);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern long lua_tointegerx(IntPtr L, int idx, out bool isnum);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_toboolean(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_tolstring(IntPtr L, int idx, out nuint len);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong lua_rawlen(IntPtr L, int idx);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaCSFunction lua_tocfunction(IntPtr L, int idx);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_touserdata(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_tothread(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_topointer(IntPtr L, int idx);

        #endregion
        
        #region Comparison and arithmetic functions

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_arith(IntPtr L, LuaOp op);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_rawequal(IntPtr L, int idx1, int idx2);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_compare(IntPtr L, int idx1, int idx2, LuaCompareOp op);

        #endregion
        
        #region push functions (C -> stack)

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushnil(IntPtr L);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushnumber(IntPtr L, double n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushinteger(IntPtr L, long n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr lua_pushlstring(IntPtr L, IntPtr str, nuint size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IntPtr lua_pushlstring(IntPtr L, ReadOnlySpan<byte> data)
        {
            fixed (byte* ptr = data)
            {
                return lua_pushlstring(L, (IntPtr)ptr, (nuint)data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr lua_pushlstring(IntPtr L, string str)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(str.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];
            var written = Encoding.UTF8.GetBytes(str, buffer);
            return lua_pushlstring(L, buffer[..written]);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr lua_pushstring(IntPtr L, IntPtr s);

        private delegate IntPtr AAA(IntPtr L, IntPtr ptr);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IntPtr lua_pushstring(IntPtr L, string str)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(str.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(str, buffer);
            buffer[written] = CStringEnd;

            fixed (byte* ptr = buffer)
            {
                return lua_pushstring(L, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushcclosure(IntPtr L, LuaCSFunction fn, int n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushboolean(IntPtr L, bool b);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushlightuserdata(IntPtr L, IntPtr p);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_pushthread(IntPtr L);

        #endregion
        
        #region get functions (Lua -> stack)

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaType lua_getglobal(IntPtr L, IntPtr name);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe LuaType lua_getglobal(IntPtr L, string name)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(name.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(name, buffer);
            buffer[written] = CStringEnd;

            fixed (byte* ptr = buffer)
            {
                return lua_getglobal(L, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_gettable(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaType lua_getfield(IntPtr L, int idx, IntPtr key);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe LuaType lua_getfield(IntPtr L, int idx, string key)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(key.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(key, buffer);
            buffer[written] = CStringEnd;

            fixed (byte* ptr = buffer)
            {
                return lua_getfield(L, idx, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_geti(IntPtr L, int idx, long n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_rawget(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_rawgeti(IntPtr L, int idx, long n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_rawgetp(IntPtr L, int idx, IntPtr p);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_createtable(IntPtr L, int narr, int nrec);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_newuserdatauv(IntPtr L, nuint size, int nuvalue);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_getmetatable(IntPtr L, int objindex);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_getiuservalue(IntPtr L, int idx, int n);

        #endregion
        
        #region set functions (stack -> Lua)

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void lua_setglobal(IntPtr L, IntPtr name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void lua_setglobal(IntPtr L, string name)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(name.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(name, buffer);
            buffer[written] = CStringEnd;

            fixed (byte* ptr = buffer)
            {
                lua_setglobal(L, (IntPtr)ptr);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_settable(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void lua_setfield(IntPtr L, int idx, IntPtr key);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void lua_setfield(IntPtr L, int idx, string key)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(key.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(key, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                lua_setfield(L, idx, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_seti(IntPtr L, int idx, long n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawset(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawseti(IntPtr L, int idx, long n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawsetp(IntPtr L, int idx, IntPtr p);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_setmetatable(IntPtr L, int objindex);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_setiuservalue(IntPtr L, int idx, int n);

        #endregion
        
        #region  'load' and 'call' functions (load and run Lua code)

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_callk(IntPtr L, int nargs, int nresults, IntPtr ctx, LuaKFunction k);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_call(IntPtr L, int nargs = 0, int nresults = LuaConst.MultiRet) => lua_callk(L, nargs, nresults, IntPtr.Zero, null);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_pcallk(IntPtr L, int nargs, int nresults, int errfunc, IntPtr ctx, LuaKFunction k);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus lua_pcall(IntPtr state, int nargs = 0, int nresults = LuaConst.MultiRet, int errfunc = 0) => lua_pcallk(state, nargs, nresults, errfunc, IntPtr.Zero, null);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaStatus lua_load(IntPtr state, LuaReader reader, IntPtr data, IntPtr chunkname, IntPtr mode);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe LuaStatus lua_load(IntPtr L, LuaReader reader, IntPtr data, string chunkname, string mode)
        {
            var maxChunkNameBytes = Encoding.UTF8.GetMaxByteCount(chunkname.Length);
            var chunkNameBuffer = maxChunkNameBytes <= StackAllocLimit ? stackalloc byte[maxChunkNameBytes + 1] : new byte[maxChunkNameBytes + 1];

            var written = Encoding.UTF8.GetBytes(chunkname, chunkNameBuffer);
            chunkNameBuffer[written] = CStringEnd;

            if (string.IsNullOrEmpty(mode))
            {
                fixed (byte* chunkNamePtr = chunkNameBuffer)
                {
                    return lua_load(L, reader, data, (IntPtr)chunkNamePtr, IntPtr.Zero);
                }
            }
            
            var modeBytes = Encoding.UTF8.GetMaxByteCount(mode.Length);
            var modeBuffer = modeBytes <= StackAllocLimit ? stackalloc byte[modeBytes + 1] : new byte[modeBytes + 1];

            written = Encoding.UTF8.GetBytes(mode, modeBuffer);
            modeBuffer[written] = CStringEnd;
            
            fixed (byte* chunkNamePtr = chunkNameBuffer, modePtr = modeBuffer)
            {
                return lua_load(L, reader, data, (IntPtr)chunkNamePtr, (IntPtr)modePtr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_dump(IntPtr state, LuaWriter writer, IntPtr data, nuint strip);
        
        #endregion

        #region coroutine functions

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_yieldk(IntPtr L, int nresults, IntPtr ctx, LuaKFunction k);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus lua_yield(IntPtr L, int nresults = 0) => lua_yieldk(L, nresults, IntPtr.Zero, null);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_resume(IntPtr L, IntPtr from, int narg, out int nresults);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_status(IntPtr L);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_isyieldable(IntPtr L);

        #endregion

        #region Warning-related functions

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_setwarnf(IntPtr L, LuaWarnFunction f, IntPtr ud);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void lua_warning(IntPtr state, IntPtr msg, int tocont);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void lua_warning(IntPtr L, string msg, bool tocont = false)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(msg.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(msg, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                lua_warning(L, (IntPtr)ptr, tocont ? 1 : 0);
            }
        }
        
        #endregion

        #region garbage-collection options

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int lua_gc(IntPtr state, LuaGC what, LuaGCParam param, int value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int lua_gc(IntPtr L, LuaGC what)
        {
            if (what == LuaGC.Param)
            {
                lua_warning(L, "lua_gc: LUA_GCPARAM is not allowed here, use lua_gcparam instead");
                return 0;
            }
            return lua_gc(L, what, LuaGCParam.PMinorMul, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int lua_gcparam(IntPtr L, LuaGCParam param, int value)
        {
            return lua_gc(L, LuaGC.Param, param, value);
        }
        
        #endregion

        #region miscellaneous functions

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_error(IntPtr L);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_next(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_concat(IntPtr L, int n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_len(IntPtr L, int idx);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaAlloc lua_getallocf(IntPtr L, out IntPtr ud);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_setallocf(IntPtr L, LuaAlloc f, IntPtr ud);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_toclose(IntPtr L, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_closeslot(IntPtr L, int idx);

        #endregion

        #region some useful macros

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr lua_getextraspace(IntPtr L)
        {
            var ptrval = L.ToInt64();
            ptrval -= IntPtr.Size;
            return (IntPtr)ptrval;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double lua_tonumber(IntPtr L, int idx) => lua_tonumberx(L, idx, out _);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long lua_tointeger(IntPtr L, int idx) => lua_tointegerx(L, idx, out _);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_pop(IntPtr state, int n) => lua_settop(state, -n - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_newtable(IntPtr L, int narr = 0, int nrec = 0) => lua_createtable(L, narr, nrec);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_register(IntPtr state, string name, LuaCSFunction func)
        {
            lua_pushcfunction(state, func);
            lua_setglobal(state, name);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_pushcfunction(IntPtr L, LuaCSFunction func) => lua_pushcclosure(L, func, 0);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_isfunction(IntPtr L, int idx) => lua_type(L, idx) == LuaType.Function;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_istable(IntPtr L, int idx) => lua_type(L, idx) == LuaType.Table;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_islightuserdata(IntPtr L, int idx) => lua_type(L, idx) == LuaType.LightUserdata;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_isnil(IntPtr L, int idx) => lua_type(L, idx) == LuaType.Nil;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_isboolean(IntPtr L, int idx) => lua_type(L, idx) == LuaType.Boolean;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_isthread(IntPtr L, int idx) => lua_type(L, idx) == LuaType.Thread;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_isnone(IntPtr L, int idx) => lua_type(L, idx) == LuaType.None;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_isnoneornil(IntPtr L, int idx) => lua_type(L, idx) is LuaType.None or LuaType.Nil;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_pushglobaltable(IntPtr L) => lua_rawgeti(L, LuaConst.RegistryIndex, LuaConst.Globals);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string lua_tostring(IntPtr L, int idx)
        {
            var str = lua_tolstring(L, idx, out var len);
            return GetString(str, len);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_insert(IntPtr state, int idx) => lua_rotate(state, idx, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_remove(IntPtr state, int idx)
        {
            lua_rotate(state, idx, -1);
            lua_pop(state, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_replace(IntPtr L, int idx)
        {
            lua_copy(L, -1, idx);
            lua_pop(L, 1);
        }

        #endregion

        #region compatibility macros

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr lua_newuserdata(IntPtr L, nuint size) => lua_newuserdatauv(L, size, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaType lua_getuservalue(IntPtr state, int idx) => lua_getiuservalue(state, idx, 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_setuservalue(IntPtr L, int idx) => lua_setiuservalue(L, idx, 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus lua_resetthread(IntPtr L) => lua_closethread(L, IntPtr.Zero);

        #endregion

        #region Debug API

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_getstack(IntPtr L, int level, ref LuaDebug debug);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaStatus lua_getinfo(IntPtr L, IntPtr what, ref LuaDebug debug);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe LuaStatus lua_getinfo(IntPtr L, string what, ref LuaDebug debug)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(what.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(what, buffer);
            buffer[written] = CStringEnd;

            fixed (byte* whatPtr = buffer)
            {
                return lua_getinfo(L, (IntPtr)whatPtr, ref debug);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getlocal")]
        private static extern IntPtr lua_getlocal_raw(IntPtr L, ref LuaDebug debug, int n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string lua_getlocal(IntPtr L, ref LuaDebug debug, int n)
        {
            var ptr = lua_getlocal_raw(L, ref debug, n);
            return Marshal.PtrToStringUTF8(ptr);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setlocal")]
        private static extern IntPtr lua_setlocal_raw(IntPtr L, ref LuaDebug debug, int n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string lua_setlocal(IntPtr L, ref LuaDebug debug, int n)
        {
            var ptr = lua_setlocal_raw(L, ref debug, n);
            return Marshal.PtrToStringUTF8(ptr);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getupvalue")]
        private static extern IntPtr lua_getupvalue_raw(IntPtr L, ref LuaDebug debug, int n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string lua_getupvalue(IntPtr L, ref LuaDebug debug, int n)
        {
            var ptr = lua_getupvalue_raw(L, ref debug, n);
            return Marshal.PtrToStringUTF8(ptr);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setupvalue")]
        public static extern IntPtr lua_setupvalue_raw(IntPtr L, ref LuaDebug debug, int n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string lua_setupvalue(IntPtr L, ref LuaDebug debug, int n)
        {
            var ptr = lua_setupvalue_raw(L, ref debug, n);
            return Marshal.PtrToStringUTF8(ptr);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_upvalueid(IntPtr L, int fidx, int n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_upvaluejoin(IntPtr L, int fidx1, int n1, int fidx2, int n2);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_sethook(IntPtr L, LuaHook func, LuaHookMask mask, int cout);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaHook lua_gethook(IntPtr L);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaHookMask lua_gethookmask(IntPtr L);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_gethookcount(IntPtr L);

        #endregion

        #endregion lua.h

        #region lualib.h

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_base(IntPtr L);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_package(IntPtr L);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_coroutine(IntPtr L);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_debug(IntPtr L);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_io(IntPtr L);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_math(IntPtr L);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_os(IntPtr L);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_string(IntPtr L);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_table(IntPtr L);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_utf8(IntPtr L);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_openselectedlibs(IntPtr L, int load, int preload);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int luaL_openlibs(IntPtr L) => luaL_openselectedlibs(L, ~0, 0);

        #endregion lualib.h

        #region lauxlib.h

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_checkversion_(IntPtr state, double ver, nuint sz);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_checkversion(IntPtr L)
        {
            nuint sz = sizeof(long) * 16 + sizeof(double);
            luaL_checkversion_(L, LuaConst.VersionNum, sz);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaType luaL_getmetafield(IntPtr L, int obj, IntPtr e);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe LuaType luaL_getmetafield(IntPtr L, int obj, string e)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(e.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(e, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_getmetafield(L, obj, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool luaL_callmeta(IntPtr L, int obj, IntPtr e);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool luaL_callmeta(IntPtr L, int obj, string e)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(e.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(e, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_callmeta(L, obj, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr luaL_tolstring(IntPtr state, int idx, out nuint len);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string luaL_tolstring(IntPtr state, int idx)
        {
            var str = luaL_tolstring(state, idx, out var len);
            return GetString(str, len);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int luaL_argerror(IntPtr state, int arg, IntPtr extramsg);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int luaL_argerror(IntPtr L, int arg, string extramsg)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(extramsg.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(extramsg, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_argerror(L, arg, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int luaL_typeerror(IntPtr state, int arg, IntPtr tname);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int luaL_typeerror(IntPtr L, int arg, string tname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(tname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(tname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_typeerror(L, arg, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr luaL_checklstring(IntPtr L, int arg, out UIntPtr len);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string luaL_checklstring(IntPtr L, int arg)
        {
            var str = luaL_checklstring(L, arg, out var len);
            return GetString(str, len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string luaL_checkstring(IntPtr L, int arg)
        {
            return luaL_checklstring(L, arg);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr luaL_optlstring(IntPtr L, int arg, string def, out nuint len);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string luaL_optlstring(IntPtr L, int arg, string def)
        {
            var str = luaL_optlstring(L, arg, def, out var len);
            return GetString(str, len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string luaL_optstring(IntPtr L, int arg, string def)
        {
            return luaL_optlstring(L, arg, def);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern double luaL_checknumber(IntPtr L, int arg);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern double luaL_optnumber(IntPtr L, int arg, double def);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern long luaL_checkinteger(IntPtr L, int arg);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern long luaL_optinteger(IntPtr L, int arg, long def);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_checkstack(IntPtr L, int sz, string msg);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_checktype(IntPtr L, int arg, LuaType t);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_checkany(IntPtr L, int arg);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool luaL_newmetatable(IntPtr state, IntPtr tname);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool luaL_newmetatable(IntPtr L, string tname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(tname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(tname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_newmetatable(L, (IntPtr)ptr);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void luaL_setmetatable(IntPtr state, IntPtr tname);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void luaL_setmetatable(IntPtr L, string tname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(tname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(tname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                luaL_setmetatable(L, (IntPtr)ptr);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr luaL_testudata(IntPtr state, int ud, IntPtr tname);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IntPtr luaL_testudata(IntPtr L, int ud, string tname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(tname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(tname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_testudata(L, ud, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr luaL_checkudata(IntPtr state, int ud, IntPtr tname);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IntPtr luaL_checkudata(IntPtr L, int ud, string tname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(tname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(tname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_checkudata(L, ud, (IntPtr)ptr);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_where(IntPtr L, int lvl);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int luaL_error(IntPtr L, IntPtr msg);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int luaL_error(IntPtr L, string tname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(tname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(tname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_error(L, (IntPtr)ptr);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_ref(IntPtr L, int t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int luaL_ref(IntPtr L) => luaL_ref(L, LuaConst.RegistryIndex);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_unref(IntPtr L, int t, int @ref);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_unref(IntPtr L, int @ref) => luaL_unref(L, LuaConst.RegistryIndex, @ref);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_getref(IntPtr state, int t, int @ref) => lua_rawgeti(state, t, @ref);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_getref(IntPtr state, int @ref) => lua_rawgeti(state, LuaConst.RegistryIndex, @ref);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaStatus luaL_loadfilex(IntPtr L, IntPtr filename, IntPtr mode);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe LuaStatus luaL_loadfilex(IntPtr L, string filename, string mode)
        {
            var maxNameBytes = Encoding.UTF8.GetMaxByteCount(filename.Length);
            var nameBuffer = maxNameBytes <= StackAllocLimit ? stackalloc byte[maxNameBytes + 1] : new byte[maxNameBytes + 1];

            var written = Encoding.UTF8.GetBytes(filename, nameBuffer);
            nameBuffer[written] = CStringEnd;

            if (string.IsNullOrEmpty(mode))
            {
                fixed (byte* namePtr = nameBuffer)
                {
                    return luaL_loadfilex(L, (IntPtr)namePtr, IntPtr.Zero);
                }
            }
            
            var maxModeBytes = Encoding.UTF8.GetMaxByteCount(mode.Length);
            var modeBuffer = maxModeBytes <= StackAllocLimit ? stackalloc byte[maxModeBytes + 1] : new byte[maxModeBytes + 1];
            
            written = Encoding.UTF8.GetBytes(mode, modeBuffer);
            modeBuffer[written] = CStringEnd;

            fixed (byte* namePtr = nameBuffer, modePtr = modeBuffer)
            {
                return luaL_loadfilex(L, (IntPtr)namePtr, (IntPtr)modePtr);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus luaL_loadfile(IntPtr state, string filename)
        {
            return luaL_loadfilex(state, filename, null);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaStatus luaL_loadbufferx(IntPtr state, IntPtr buff, nuint size, IntPtr name, IntPtr mode);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe LuaStatus luaL_loadbufferx(IntPtr L, ReadOnlySpan<byte> buff, string name, string mode)
        {
            var maxNameBytes = Encoding.UTF8.GetMaxByteCount(name.Length);
            var nameBuffer = maxNameBytes <= StackAllocLimit ? stackalloc byte[maxNameBytes + 1] : new byte[maxNameBytes + 1];

            var written = Encoding.UTF8.GetBytes(name, nameBuffer);
            nameBuffer[written] = CStringEnd;

            if (string.IsNullOrEmpty(mode))
            {
                fixed (byte* buffPtr = buff, namePtr = nameBuffer)
                {
                    return luaL_loadbufferx(L, (IntPtr)buffPtr, (nuint)buff.Length, (IntPtr)namePtr, IntPtr.Zero);
                }
            }
            
            var maxModeBytes = Encoding.UTF8.GetMaxByteCount(mode.Length);
            var modeBuffer = maxModeBytes <= StackAllocLimit ? stackalloc byte[maxModeBytes + 1] : new byte[maxModeBytes + 1];
            
            written = Encoding.UTF8.GetBytes(mode, modeBuffer);
            modeBuffer[written] = 0;

            fixed (byte* buffPtr = buff, namePtr = nameBuffer, modePtr = modeBuffer)
            {
                return luaL_loadbufferx(L, (IntPtr)buffPtr, (nuint)buff.Length, (IntPtr)namePtr, (IntPtr)modePtr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus luaL_loadbuffer(IntPtr L, ReadOnlySpan<byte> buff, string name)
        {
            return luaL_loadbufferx(L, buff, name, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus luaL_loadbuffer(IntPtr L, string buff, string name)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(buff.Length);
            var byteBuffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];
            var written = Encoding.UTF8.GetBytes(buff, byteBuffer);
            return luaL_loadbuffer(L, byteBuffer[..written], name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus luaL_loadstring(IntPtr L, string s)
        {
            return luaL_loadbuffer(L, s, s);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr luaL_newstate();
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint luaL_makeseed(IntPtr L);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern long luaL_len(IntPtr state, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_setfuncs(IntPtr state, LuaLReg[] l, int nup);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool luaL_getsubtable(IntPtr state, int idx, IntPtr fname);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool luaL_getsubtable(IntPtr L, int idx, string fname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(fname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(fname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_getsubtable(L, idx, (IntPtr)ptr);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void luaL_traceback(IntPtr L, IntPtr L1, IntPtr msg, int level);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void luaL_traceback(IntPtr L, IntPtr L1, string msg, int level)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(msg.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(msg, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                luaL_traceback(L, L1, (IntPtr)ptr, level);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void luaL_requiref(IntPtr state, IntPtr modname, LuaCSFunction openf, int glb);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void luaL_requiref(IntPtr L, string modname, LuaCSFunction openf, int glb)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(modname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(modname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                luaL_requiref(L, (IntPtr)ptr, openf, glb);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_newlibtable(IntPtr L, LuaLReg[] l)
        {
            lua_createtable(L, 0, l.Length - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_newlib(IntPtr L, LuaLReg[] l)
        {
            luaL_checkversion(L);
            luaL_newlibtable(L, l);
            luaL_setfuncs(L, l, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_argcheck(IntPtr L, bool cond, int arg, string extramsg)
        {
            if (!cond)
            {
                luaL_argerror(L, arg, extramsg);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_argexpected(IntPtr L, bool cond, int arg, string tname)
        {
            if (!cond)
            {
                luaL_typeerror(L, arg, tname);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string luaL_typename(IntPtr L, int idx)
        {
            return lua_typename(L, lua_type(L, idx));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus luaL_dofile(IntPtr L, string filename)
        {
            var status = luaL_loadfile(L, filename);
            if (status == LuaStatus.Ok)
            {
                status = lua_pcall(L, 0, LuaConst.MultiRet, 0);
            }
            return status;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus luaL_dostring(IntPtr L, string str)
        {
            var status = luaL_loadstring(L, str);
            if (status == LuaStatus.Ok) 
            {
                status = lua_pcall(L, 0, LuaConst.MultiRet, 0);
            }
            return status;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaType luaL_getmetatable(IntPtr L, string name)
        {
            return lua_getfield(L, LuaConst.RegistryIndex, name);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_pushfail(IntPtr L)
        {
            lua_pushboolean(L, false);
        }
        
        #endregion lauxlib.h

#pragma warning restore IDE1006
    }
}