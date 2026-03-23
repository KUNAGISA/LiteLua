# LiteLua

LiteLua 是一个轻量的 **Lua C API (.NET) 绑定库**，用于在 C# 中直接调用 Lua 原生接口。

> ⚠️ 注意：LiteLua **不是 Lua 解释器，也不是完整脚本框架**
> 它只提供底层 Binding，用于构建你自己的 Lua 运行时

---

## ✨ 特性

* ✔ 直接绑定 Lua C API（接近原生语义）
* ✔ 使用 `Span` / `stackalloc`，减少 GC
* ✔ UTF8 全链路处理
* ✔ 适合构建自定义 Lua 框架（而不是黑盒）

---

## 📦 适用场景

* 在 C# / Unity 中嵌入 Lua
* 自定义脚本系统（非 xLua / SLua 黑盒）
* 高性能 Lua 绑定（可控 GC / 内存）
* 学习 Lua C API 或实现自己的绑定层

---

## 📁 命名空间

```csharp
LiteLua
```

核心入口：

```csharp
LiteLua.LuaAPI
```

---

## 🚀 快速开始

### 1. 创建 Lua 状态

```csharp
var L = LuaAPI.luaL_newstate();
LuaAPI.luaL_openlibs(L);
```

---

### 2. 执行 Lua 代码

```csharp
LuaAPI.luaL_dostring(L, "print('Hello from Lua')");
```

---

### 3. 注册 C# 函数到 Lua

```csharp
static int Add(IntPtr L)
{
    var a = LuaAPI.lua_tonumber(L, 1);
    var b = LuaAPI.lua_tonumber(L, 2);

    LuaAPI.lua_pushnumber(L, a + b);
    return 1;
}

LuaAPI.lua_pushcfunction(L, Add);
LuaAPI.lua_setglobal(L, "add");
```

Lua 调用：

```lua
print(add(1, 2)) -- 3
```

---

### 4. 读取 Lua 全局变量

```csharp
LuaAPI.lua_getglobal(L, "x");
var value = LuaAPI.lua_tonumber(L, -1);
LuaAPI.lua_pop(L, 1);
```

---

### 5. 操作 Table

```csharp
LuaAPI.lua_newtable(L);

LuaAPI.lua_pushstring(L, "key");
LuaAPI.lua_pushnumber(L, 123);
LuaAPI.lua_settable(L, -3);

LuaAPI.lua_setglobal(L, "t");
```

---

### 6. userdata 示例（基础）

```csharp
var ptr = LuaAPI.lua_newuserdata(L, (nuint)sizeof(int));
*(int*)ptr = 42;

LuaAPI.lua_setglobal(L, "data");
```

---

## 🔁 Coroutine 示例

```csharp
var thread = LuaAPI.lua_newthread(L);

LuaAPI.lua_getglobal(thread, "func");
LuaAPI.lua_resume(thread, IntPtr.Zero, 0, out int nresults);
```

---

## 🧠 设计理念

LiteLua 遵循：

* **尽量贴近 Lua C API**
* **不隐藏行为**
* **不引入额外抽象**
* **由使用者构建上层系统**

---

## 🔥 为什么不用 xLua / SLua？

LiteLua 的定位是：

* ✔ 更低层
* ✔ 更可控
* ✔ 更适合自定义架构

如果你需要：

* 自动绑定
* 零配置使用

👉 建议使用 xLua / SLua

---

## 🛠 依赖

* Lua 5.5（或兼容版本）
* 需要提供 `lua.dll / liblua.so / liblua.a`

---

## 📌 注意事项

* 本库不负责 Lua 生命周期管理（由你控制）
* 所有字符串默认 UTF8
* 使用 P/Invoke，请确保 Lua 动态库正确加载

---

## 📄 License

MIT

---

## ❤️ 致谢

* Lua 官方实现
* 现有 Lua binding 项目（xLua / sLua 等）

---