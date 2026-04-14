using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteLua
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int LuaCSFunction(IntPtr l);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int LuaKFunction(IntPtr l, LuaStatus status, IntPtr ctx);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr LuaReader(IntPtr l, IntPtr ud, out nuint size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int LuaWriter(IntPtr l, IntPtr p, nuint size, IntPtr ud);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr LuaAlloc(IntPtr ud, IntPtr ptr, nuint osize, nuint nsize);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LuaWarnFunction(IntPtr ud, IntPtr msg, int tocont);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LuaHook(IntPtr l, IntPtr debug);

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
        Add     = 0,    // LUA_OPADD
        Sub     = 1,    // LUA_OPSUB
        Mul     = 2,    // LUA_OPMUL
        Mod     = 3,    // LUA_OPMOD
        Div     = 4,    // LUA_OPDIV
        Idiv    = 5,    // LUA_OPIDIV
        Pow     = 6,    // LUA_OPPOW

        Band    = 7,    // LUA_OPBAND
        Bor     = 8,    // LUA_OPBOR
        Bxor    = 9,    // LUA_OPBXOR

        Shl     = 10,   // LUA_OPSHL
        Shr     = 11,   // LUA_OPSHR

        Unm     = 12,   // LUA_OPUNM
        Bnot    = 13,   // LUA_OPBNOT
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
        public static extern void lua_close(IntPtr l);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_newthread(IntPtr l);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_closethread(IntPtr l, IntPtr from);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaCSFunction lua_atpanic(IntPtr l, LuaCSFunction panicf);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern double lua_version(IntPtr l);

        #endregion
        
        #region basic stack manipulation

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_absindex(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_gettop(IntPtr l);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_settop(IntPtr l, int idx);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushvalue(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rotate(IntPtr l, int idx, int n);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_copy(IntPtr l, int fromidx, int toidx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_checkstack(IntPtr l, int n);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_xmove(IntPtr from, IntPtr to, int n);

        #endregion
        
        #region access functions (stack -> C)

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_isnumber(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_isstring(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_iscfunction(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_isinteger(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_isuserdata(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_type(IntPtr l, int idx);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_typename")]
        private static extern IntPtr lua_typename_raw(IntPtr l, LuaType tp);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string lua_typename(IntPtr l, LuaType tp)
        {
            var ptr = lua_typename_raw(l, tp);
            return Marshal.PtrToStringUTF8(ptr);
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern double lua_tonumberx(IntPtr l, int idx, out bool isnum);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern long lua_tointegerx(IntPtr l, int idx, out bool isnum);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_toboolean(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_tolstring(IntPtr l, int idx, out nuint len);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong lua_rawlen(IntPtr l, int idx);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaCSFunction lua_tocfunction(IntPtr l, int idx);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_touserdata(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_tothread(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_topointer(IntPtr l, int idx);

        #endregion
        
        #region Comparison and arithmetic functions

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_arith(IntPtr l, LuaOp op);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_rawequal(IntPtr l, int idx1, int idx2);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_compare(IntPtr l, int idx1, int idx2, LuaCompareOp op);

        #endregion
        
        #region push functions (C -> stack)

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushnil(IntPtr l);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushnumber(IntPtr l, double n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushinteger(IntPtr l, long n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr lua_pushlstring(IntPtr l, IntPtr str, nuint size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IntPtr lua_pushlstring(IntPtr l, ReadOnlySpan<byte> data)
        {
            fixed (byte* ptr = data)
            {
                return lua_pushlstring(l, (IntPtr)ptr, (nuint)data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr lua_pushlstring(IntPtr l, string str)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(str.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];
            var written = Encoding.UTF8.GetBytes(str, buffer);
            return lua_pushlstring(l, buffer[..written]);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr lua_pushstring(IntPtr l, IntPtr s);

        private delegate IntPtr AAA(IntPtr l, IntPtr ptr);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IntPtr lua_pushstring(IntPtr l, string str)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(str.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(str, buffer);
            buffer[written] = CStringEnd;

            fixed (byte* ptr = buffer)
            {
                return lua_pushstring(l, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushcclosure(IntPtr l, LuaCSFunction fn, int n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushboolean(IntPtr l, bool b);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushlightuserdata(IntPtr l, IntPtr p);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_pushthread(IntPtr l);

        #endregion
        
        #region get functions (Lua -> stack)

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaType lua_getglobal(IntPtr l, IntPtr name);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe LuaType lua_getglobal(IntPtr l, string name)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(name.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(name, buffer);
            buffer[written] = CStringEnd;

            fixed (byte* ptr = buffer)
            {
                return lua_getglobal(l, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_gettable(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaType lua_getfield(IntPtr l, int idx, IntPtr key);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe LuaType lua_getfield(IntPtr l, int idx, string key)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(key.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(key, buffer);
            buffer[written] = CStringEnd;

            fixed (byte* ptr = buffer)
            {
                return lua_getfield(l, idx, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_geti(IntPtr l, int idx, long n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_rawget(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_rawgeti(IntPtr l, int idx, long n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_rawgetp(IntPtr l, int idx, IntPtr p);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_createtable(IntPtr l, int narr, int nrec);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_newuserdatauv(IntPtr l, nuint size, int nuvalue);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_getmetatable(IntPtr l, int objindex);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaType lua_getiuservalue(IntPtr l, int idx, int n);

        #endregion
        
        #region set functions (stack -> Lua)

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void lua_setglobal(IntPtr l, IntPtr name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void lua_setglobal(IntPtr l, string name)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(name.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(name, buffer);
            buffer[written] = CStringEnd;

            fixed (byte* ptr = buffer)
            {
                lua_setglobal(l, (IntPtr)ptr);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_settable(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void lua_setfield(IntPtr l, int idx, IntPtr key);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void lua_setfield(IntPtr l, int idx, string key)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(key.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(key, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                lua_setfield(l, idx, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_seti(IntPtr l, int idx, long n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawset(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawseti(IntPtr l, int idx, long n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawsetp(IntPtr l, int idx, IntPtr p);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_setmetatable(IntPtr l, int objindex);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_setiuservalue(IntPtr l, int idx, int n);

        #endregion
        
        #region  'load' and 'call' functions (load and run Lua code)

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_callk(IntPtr l, int nargs, int nresults, IntPtr ctx, LuaKFunction k);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_call(IntPtr l, int nargs = 0, int nresults = LuaConst.MultiRet) => lua_callk(l, nargs, nresults, IntPtr.Zero, null);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_pcallk(IntPtr l, int nargs, int nresults, int errfunc, IntPtr ctx, LuaKFunction k);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus lua_pcall(IntPtr state, int nargs = 0, int nresults = LuaConst.MultiRet, int errfunc = 0) => lua_pcallk(state, nargs, nresults, errfunc, IntPtr.Zero, null);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaStatus lua_load(IntPtr state, LuaReader reader, IntPtr data, IntPtr chunkname, IntPtr mode);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe LuaStatus lua_load(IntPtr l, LuaReader reader, IntPtr data, string chunkname, string mode)
        {
            var maxChunkNameBytes = Encoding.UTF8.GetMaxByteCount(chunkname.Length);
            var chunkNameBuffer = maxChunkNameBytes <= StackAllocLimit ? stackalloc byte[maxChunkNameBytes + 1] : new byte[maxChunkNameBytes + 1];

            var written = Encoding.UTF8.GetBytes(chunkname, chunkNameBuffer);
            chunkNameBuffer[written] = CStringEnd;

            if (string.IsNullOrEmpty(mode))
            {
                fixed (byte* chunkNamePtr = chunkNameBuffer)
                {
                    return lua_load(l, reader, data, (IntPtr)chunkNamePtr, IntPtr.Zero);
                }
            }
            
            var modeBytes = Encoding.UTF8.GetMaxByteCount(mode.Length);
            var modeBuffer = modeBytes <= StackAllocLimit ? stackalloc byte[modeBytes + 1] : new byte[modeBytes + 1];

            written = Encoding.UTF8.GetBytes(mode, modeBuffer);
            modeBuffer[written] = CStringEnd;
            
            fixed (byte* chunkNamePtr = chunkNameBuffer, modePtr = modeBuffer)
            {
                return lua_load(l, reader, data, (IntPtr)chunkNamePtr, (IntPtr)modePtr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_dump(IntPtr state, LuaWriter writer, IntPtr data, nuint strip);
        
        #endregion

        #region coroutine functions

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_yieldk(IntPtr l, int nresults, IntPtr ctx, LuaKFunction k);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus lua_yield(IntPtr l, int nresults = 0) => lua_yieldk(l, nresults, IntPtr.Zero, null);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_resume(IntPtr l, IntPtr from, int narg, out int nresults);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_status(IntPtr l);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_isyieldable(IntPtr l);

        #endregion

        #region Warning-related functions

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_setwarnf(IntPtr l, LuaWarnFunction f, IntPtr ud);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void lua_warning(IntPtr state, IntPtr msg, int tocont);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void lua_warning(IntPtr l, string msg, bool tocont = false)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(msg.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(msg, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                lua_warning(l, (IntPtr)ptr, tocont ? 1 : 0);
            }
        }
        
        #endregion

        #region garbage-collection options

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_gc")]
        private static extern int lua_gc_raw(IntPtr state, LuaGC what, nuint arg0, int arg1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_gc_stop(IntPtr l)
        {
            lua_gc_raw(l, LuaGC.Stop, 0, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_gc_restart(IntPtr l)
        {
            lua_gc_raw(l, LuaGC.Restart, 0, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_gc_collect(IntPtr l)
        {
            lua_gc_raw(l, LuaGC.Collect, 0, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int lua_gc_count(IntPtr l)
        {
            return lua_gc_raw(l, LuaGC.Count, 0, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int lua_gc_countb(IntPtr l)
        {
            return lua_gc_raw(l, LuaGC.CountB, 0, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_gc_step(IntPtr l, nuint size)
        {
            return lua_gc_raw(l, LuaGC.Step, size, 0) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_gc_isrunning(IntPtr l)
        {
            return lua_gc_raw(l, LuaGC.IsRunning, 0, 0) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int lua_gc_inc(IntPtr l)
        {
            return lua_gc_raw(l, LuaGC.Inc, 0, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int lua_gc_gen(IntPtr l)
        {
            return lua_gc_raw(l, LuaGC.Gen, 0, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int lua_gc_getparam(IntPtr l, LuaGCParam param)
        {
            return lua_gc_raw(l, LuaGC.Param, (nuint)param, -1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int lua_gc_setparam(IntPtr l, LuaGCParam param, int value)
        {
            return lua_gc_raw(l, LuaGC.Param, (nuint)param, value);
        }
        
        #endregion

        #region miscellaneous functions

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_error(IntPtr l);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_next(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_concat(IntPtr l, int n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_len(IntPtr l, int idx);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaAlloc lua_getallocf(IntPtr l, out IntPtr ud);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_setallocf(IntPtr l, LuaAlloc f, IntPtr ud);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_toclose(IntPtr l, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_closeslot(IntPtr l, int idx);

        #endregion

        #region some useful macros

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr lua_getextraspace(IntPtr l)
        {
            var ptrval = l.ToInt64();
            ptrval -= IntPtr.Size;
            return (IntPtr)ptrval;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double lua_tonumber(IntPtr l, int idx) => lua_tonumberx(l, idx, out _);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long lua_tointeger(IntPtr l, int idx) => lua_tointegerx(l, idx, out _);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_pop(IntPtr state, int n) => lua_settop(state, -n - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_newtable(IntPtr l, int narr = 0, int nrec = 0) => lua_createtable(l, narr, nrec);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_register(IntPtr state, string name, LuaCSFunction func)
        {
            lua_pushcfunction(state, func);
            lua_setglobal(state, name);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_pushcfunction(IntPtr l, LuaCSFunction func) => lua_pushcclosure(l, func, 0);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_isfunction(IntPtr l, int idx) => lua_type(l, idx) == LuaType.Function;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_istable(IntPtr l, int idx) => lua_type(l, idx) == LuaType.Table;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_islightuserdata(IntPtr l, int idx) => lua_type(l, idx) == LuaType.LightUserdata;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_isnil(IntPtr l, int idx) => lua_type(l, idx) == LuaType.Nil;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_isboolean(IntPtr l, int idx) => lua_type(l, idx) == LuaType.Boolean;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_isthread(IntPtr l, int idx) => lua_type(l, idx) == LuaType.Thread;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_isnone(IntPtr l, int idx) => lua_type(l, idx) == LuaType.None;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_isnoneornil(IntPtr l, int idx) => lua_type(l, idx) is LuaType.None or LuaType.Nil;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_pushglobaltable(IntPtr l) => lua_rawgeti(l, LuaConst.RegistryIndex, LuaConst.Globals);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string lua_tostring(IntPtr l, int idx)
        {
            var str = lua_tolstring(l, idx, out var len);
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
        public static void lua_replace(IntPtr l, int idx)
        {
            lua_copy(l, -1, idx);
            lua_pop(l, 1);
        }

        #endregion

        #region compatibility macros

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr lua_newuserdata(IntPtr l, nuint size) => lua_newuserdatauv(l, size, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaType lua_getuservalue(IntPtr state, int idx) => lua_getiuservalue(state, idx, 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_setuservalue(IntPtr l, int idx) => lua_setiuservalue(l, idx, 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus lua_resetthread(IntPtr l) => lua_closethread(l, IntPtr.Zero);

        #endregion

        #region Debug API

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaStatus lua_getstack(IntPtr l, int level, ref LuaDebug debug);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaStatus lua_getinfo(IntPtr l, IntPtr what, ref LuaDebug debug);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe LuaStatus lua_getinfo(IntPtr l, string what, ref LuaDebug debug)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(what.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(what, buffer);
            buffer[written] = CStringEnd;

            fixed (byte* whatPtr = buffer)
            {
                return lua_getinfo(l, (IntPtr)whatPtr, ref debug);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getlocal")]
        private static extern IntPtr lua_getlocal_raw(IntPtr l, ref LuaDebug debug, int n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string lua_getlocal(IntPtr l, ref LuaDebug debug, int n)
        {
            var ptr = lua_getlocal_raw(l, ref debug, n);
            return Marshal.PtrToStringUTF8(ptr);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setlocal")]
        private static extern IntPtr lua_setlocal_raw(IntPtr l, ref LuaDebug debug, int n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string lua_setlocal(IntPtr l, ref LuaDebug debug, int n)
        {
            var ptr = lua_setlocal_raw(l, ref debug, n);
            return Marshal.PtrToStringUTF8(ptr);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_getupvalue(IntPtr l, int funcindex, int n);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_setupvalue(IntPtr l, int funcindex, int n);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_upvalueid(IntPtr l, int fidx, int n);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_upvaluejoin(IntPtr l, int fidx1, int n1, int fidx2, int n2);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_sethook(IntPtr l, LuaHook func, LuaHookMask mask, int cout);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaHook lua_gethook(IntPtr l);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaHookMask lua_gethookmask(IntPtr l);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_gethookcount(IntPtr l);

        #endregion

        #endregion lua.h

        #region lualib.h

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_base(IntPtr l);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_package(IntPtr l);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_coroutine(IntPtr l);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_debug(IntPtr l);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_io(IntPtr l);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_math(IntPtr l);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_os(IntPtr l);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_string(IntPtr l);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_table(IntPtr l);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_utf8(IntPtr l);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_openselectedlibs(IntPtr l, int load, int preload);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int luaL_openlibs(IntPtr l) => luaL_openselectedlibs(l, ~0, 0);

        #endregion lualib.h

        #region lauxlib.h

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_checkversion_(IntPtr state, double ver, nuint sz);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_checkversion(IntPtr l)
        {
            nuint sz = sizeof(long) * 16 + sizeof(double);
            luaL_checkversion_(l, LuaConst.VersionNum, sz);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaType luaL_getmetafield(IntPtr l, int obj, IntPtr e);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe LuaType luaL_getmetafield(IntPtr l, int obj, string e)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(e.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(e, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_getmetafield(l, obj, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool luaL_callmeta(IntPtr l, int obj, IntPtr e);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool luaL_callmeta(IntPtr l, int obj, string e)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(e.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(e, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_callmeta(l, obj, (IntPtr)ptr);
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
        public static unsafe int luaL_argerror(IntPtr l, int arg, string extramsg)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(extramsg.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(extramsg, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_argerror(l, arg, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int luaL_typeerror(IntPtr state, int arg, IntPtr tname);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int luaL_typeerror(IntPtr l, int arg, string tname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(tname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(tname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_typeerror(l, arg, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr luaL_checklstring(IntPtr l, int arg, out UIntPtr len);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string luaL_checklstring(IntPtr l, int arg)
        {
            var str = luaL_checklstring(l, arg, out var len);
            return GetString(str, len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string luaL_checkstring(IntPtr l, int arg)
        {
            return luaL_checklstring(l, arg);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr luaL_optlstring(IntPtr l, int arg, string def, out nuint len);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string luaL_optlstring(IntPtr l, int arg, string def)
        {
            var str = luaL_optlstring(l, arg, def, out var len);
            return GetString(str, len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string luaL_optstring(IntPtr l, int arg, string def)
        {
            return luaL_optlstring(l, arg, def);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern double luaL_checknumber(IntPtr l, int arg);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern double luaL_optnumber(IntPtr l, int arg, double def);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern long luaL_checkinteger(IntPtr l, int arg);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern long luaL_optinteger(IntPtr l, int arg, long def);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_checkstack(IntPtr l, int sz, string msg);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_checktype(IntPtr l, int arg, LuaType t);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_checkany(IntPtr l, int arg);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool luaL_newmetatable(IntPtr state, IntPtr tname);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool luaL_newmetatable(IntPtr l, string tname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(tname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(tname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_newmetatable(l, (IntPtr)ptr);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void luaL_setmetatable(IntPtr state, IntPtr tname);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void luaL_setmetatable(IntPtr l, string tname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(tname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(tname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                luaL_setmetatable(l, (IntPtr)ptr);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr luaL_testudata(IntPtr state, int ud, IntPtr tname);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IntPtr luaL_testudata(IntPtr l, int ud, string tname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(tname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(tname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_testudata(l, ud, (IntPtr)ptr);
            }
        }
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr luaL_checkudata(IntPtr state, int ud, IntPtr tname);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IntPtr luaL_checkudata(IntPtr l, int ud, string tname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(tname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(tname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_checkudata(l, ud, (IntPtr)ptr);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_where(IntPtr l, int lvl);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int luaL_error(IntPtr l, IntPtr msg);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int luaL_error(IntPtr l, string tname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(tname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(tname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_error(l, (IntPtr)ptr);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_ref(IntPtr l, int t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int luaL_ref(IntPtr l) => luaL_ref(l, LuaConst.RegistryIndex);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_unref(IntPtr l, int t, int @ref);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_unref(IntPtr l, int @ref) => luaL_unref(l, LuaConst.RegistryIndex, @ref);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_getref(IntPtr state, int t, int @ref) => lua_rawgeti(state, t, @ref);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_getref(IntPtr state, int @ref) => lua_rawgeti(state, LuaConst.RegistryIndex, @ref);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern LuaStatus luaL_loadfilex(IntPtr l, IntPtr filename, IntPtr mode);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe LuaStatus luaL_loadfilex(IntPtr l, string filename, string mode)
        {
            var maxNameBytes = Encoding.UTF8.GetMaxByteCount(filename.Length);
            var nameBuffer = maxNameBytes <= StackAllocLimit ? stackalloc byte[maxNameBytes + 1] : new byte[maxNameBytes + 1];

            var written = Encoding.UTF8.GetBytes(filename, nameBuffer);
            nameBuffer[written] = CStringEnd;

            if (string.IsNullOrEmpty(mode))
            {
                fixed (byte* namePtr = nameBuffer)
                {
                    return luaL_loadfilex(l, (IntPtr)namePtr, IntPtr.Zero);
                }
            }
            
            var maxModeBytes = Encoding.UTF8.GetMaxByteCount(mode.Length);
            var modeBuffer = maxModeBytes <= StackAllocLimit ? stackalloc byte[maxModeBytes + 1] : new byte[maxModeBytes + 1];
            
            written = Encoding.UTF8.GetBytes(mode, modeBuffer);
            modeBuffer[written] = CStringEnd;

            fixed (byte* namePtr = nameBuffer, modePtr = modeBuffer)
            {
                return luaL_loadfilex(l, (IntPtr)namePtr, (IntPtr)modePtr);
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
        public static unsafe LuaStatus luaL_loadbufferx(IntPtr l, ReadOnlySpan<byte> buff, string name, string mode)
        {
            var maxNameBytes = Encoding.UTF8.GetMaxByteCount(name.Length);
            var nameBuffer = maxNameBytes <= StackAllocLimit ? stackalloc byte[maxNameBytes + 1] : new byte[maxNameBytes + 1];

            var written = Encoding.UTF8.GetBytes(name, nameBuffer);
            nameBuffer[written] = CStringEnd;

            if (string.IsNullOrEmpty(mode))
            {
                fixed (byte* buffPtr = buff, namePtr = nameBuffer)
                {
                    return luaL_loadbufferx(l, (IntPtr)buffPtr, (nuint)buff.Length, (IntPtr)namePtr, IntPtr.Zero);
                }
            }
            
            var maxModeBytes = Encoding.UTF8.GetMaxByteCount(mode.Length);
            var modeBuffer = maxModeBytes <= StackAllocLimit ? stackalloc byte[maxModeBytes + 1] : new byte[maxModeBytes + 1];
            
            written = Encoding.UTF8.GetBytes(mode, modeBuffer);
            modeBuffer[written] = 0;

            fixed (byte* buffPtr = buff, namePtr = nameBuffer, modePtr = modeBuffer)
            {
                return luaL_loadbufferx(l, (IntPtr)buffPtr, (nuint)buff.Length, (IntPtr)namePtr, (IntPtr)modePtr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus luaL_loadbuffer(IntPtr l, ReadOnlySpan<byte> buff, string name)
        {
            return luaL_loadbufferx(l, buff, name, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus luaL_loadbuffer(IntPtr l, string buff, string name)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(buff.Length);
            var byteBuffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];
            var written = Encoding.UTF8.GetBytes(buff, byteBuffer);
            return luaL_loadbuffer(l, byteBuffer[..written], name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus luaL_loadstring(IntPtr l, string s)
        {
            return luaL_loadbuffer(l, s, s);
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr luaL_newstate();
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint luaL_makeseed(IntPtr l);
        
        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern long luaL_len(IntPtr state, int idx);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_setfuncs(IntPtr state, LuaLReg[] l, int nup);

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool luaL_getsubtable(IntPtr state, int idx, IntPtr fname);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool luaL_getsubtable(IntPtr l, int idx, string fname)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(fname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(fname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                return luaL_getsubtable(l, idx, (IntPtr)ptr);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void luaL_traceback(IntPtr l, IntPtr L1, IntPtr msg, int level);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void luaL_traceback(IntPtr l, IntPtr L1, string msg, int level)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(msg.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(msg, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                luaL_traceback(l, L1, (IntPtr)ptr, level);
            }
        }

        [DllImport(LuaDLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void luaL_requiref(IntPtr state, IntPtr modname, LuaCSFunction openf, int glb);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void luaL_requiref(IntPtr l, string modname, LuaCSFunction openf, int glb)
        {
            var maxBytes = Encoding.UTF8.GetMaxByteCount(modname.Length);
            var buffer = maxBytes <= StackAllocLimit ? stackalloc byte[maxBytes + 1] : new byte[maxBytes + 1];

            var written = Encoding.UTF8.GetBytes(modname, buffer);
            buffer[written] = CStringEnd;
            
            fixed (byte* ptr = buffer)
            {
                luaL_requiref(l, (IntPtr)ptr, openf, glb);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_newlibtable(IntPtr l, LuaLReg[] regs)
        {
            lua_createtable(l, 0, regs.Length - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_newlib(IntPtr l, LuaLReg[] regs)
        {
            luaL_checkversion(l);
            luaL_newlibtable(l, regs);
            luaL_setfuncs(l, regs, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_argcheck(IntPtr l, bool cond, int arg, string extramsg)
        {
            if (!cond)
            {
                luaL_argerror(l, arg, extramsg);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_argexpected(IntPtr l, bool cond, int arg, string tname)
        {
            if (!cond)
            {
                luaL_typeerror(l, arg, tname);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string luaL_typename(IntPtr l, int idx)
        {
            return lua_typename(l, lua_type(l, idx));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus luaL_dofile(IntPtr l, string filename)
        {
            var status = luaL_loadfile(l, filename);
            if (status == LuaStatus.Ok)
            {
                status = lua_pcall(l, 0, LuaConst.MultiRet, 0);
            }
            return status;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaStatus luaL_dostring(IntPtr l, string str)
        {
            var status = luaL_loadstring(l, str);
            if (status == LuaStatus.Ok) 
            {
                status = lua_pcall(l, 0, LuaConst.MultiRet, 0);
            }
            return status;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LuaType luaL_getmetatable(IntPtr l, string name)
        {
            return lua_getfield(l, LuaConst.RegistryIndex, name);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaL_pushfail(IntPtr l)
        {
            lua_pushboolean(l, false);
        }
        
        #endregion lauxlib.h

#pragma warning restore IDE1006
    }
}
