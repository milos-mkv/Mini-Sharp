# MiniSharp
*A lightweight embeddable scripting language for C#*


<p align="center">
  <img src="logo.png" alt="MiniSharp Logo" width="400" height="400">
</p>

> ⚠️ **Warning: Early Development Stage**
>
> MiniSharp is currently in an **early development state**.  
> The language, parser, and runtime are still evolving and may contain **bugs, incomplete features, or breaking changes**.  
> Use it for **experimentation, learning, or prototyping** — not yet in production environments.


MiniSharp is a **C#-like scripting language** designed to be embedded directly into .NET applications.  
It focuses on **simplicity**, **runtime flexibility**, and **deep reflection access** — scripts can call, modify, and construct **any public C# object** safely at runtime.

---

## ✨ Features

- **C#-like syntax** – familiar operators, conditionals, loops, and functions
- **Anonymous functions and lambdas** – `()->{ ... }`
- **Dynamic object access** – `obj.Field`, `obj.Method()`, `arr[0]`
- **Reflection bridge** – call and modify any public C# type
- **First-class functions** – pass and store callables in arrays or variables
- **Simple embedding API** – execute scripts from your C# app in a few lines
- **Optional typed interop** – you can expose your own `Std` or host objects
- **Flat execution model** – ideal for embedding in game engines, editors, and tools

---

## 🧱 Basic Syntax

### Variables
```js
let name = "Milos";
let age = 27;
let sum = 0;
```

### Arithmetic and expressions
```js
let a = 10;
let b = 20;
let c = (a + b) * 2;
do Std.Print(c);    // 60
```
### Control flow
```js
let x = 5;

if (x < 10) {
    do Std.Print("small");
} else {
    do Std.Print("large");
}

let i = 0;
while (i < 5) {
    do Std.Print("loop", i);
    i = i + 1;
    if (i == 3) {
        break;   
    }
}

// Start, End, Increment
for (let j = 0, 100, 1) {
    do Std.Print(j);
}
```

## ⚙️ Functions
### Declared functions
```js
function add(a, b) {
    return a + b;
}

do Std.Print(add(10, 20));   // 30
```
### Anonymous functions (lambdas)
```js
let adder = (x, y) -> {
    return x + y;
};

do Std.Print(adder(5, 7));   // 12
```

## 🧩 Arrays and objects
### Arrays
```js
let arr = [10, 20, 30];
do Std.Print(arr[0]);        // 10

arr[1] = 50;
do Std.Print(arr[1]);        // 50

let mularr = [
    [1, 2, 3],
    [4, 5, 6]
];

do Std.Print(mularr[1, 2]); // 6
```
### Arrays with anonymous functions
```js
let actions = [
    () -> { do Std.Print("Hello"); },
    () -> { do Std.Print("World"); }
];

do actions[0]();  // Hello
do actions[1]();  // World
```

## 🧠 Reflection access (C# interop)
MiniSharp lets you access any C# object, field, or method via reflection:
```js
do Std.Print(System.Math.Abs(-42));
do Std.Print(System.String.Join("-", ["a", "b", "c"]));

let sb = new System.Text.StringBuilder();
do sb.Append("MiniSharp ");
do sb.Append("rocks!");
do Std.Print(sb.ToString());
```
You can even access fields, properties, and constants:
```js
do Std.Print(System.DateTime.Now);
do Std.Print(System.Int32.MaxValue);
```

## 🔁 Function chaining and member access
```js
let sb = new System.Text.StringBuilder();
do sb.Append("Hello").Append(" ").Append("MiniSharp");
    do Std.Print(sb.ToString());
```

## 🧠 Embedding MiniSharp in C#
```csharp
using MiniSharp;

MiniSharpInterpreter msi = new MiniSharpInterpreter();
msi.executeFile("./main.ms");
```

## 🚀 Performance

MiniSharp is a tree-walking interpreter, similar to early Lua or Python.
It favors readability and flexibility over raw speed.

Benchmark (100,000-iteration loop):

| Language  | Runtime |
| --------  | ------- |
| Lua 5.4   | ~0.21s  |
| MiniSharp | ~0.87s  |

## 📄 License
MIT License © 2025 Milos Milicevic