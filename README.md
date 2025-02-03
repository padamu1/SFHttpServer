## ✨ SFHttpLibrary - Socket-based HTTP Communication Library

This library is designed to **support HTTP communication over sockets**.

---

## 🖥️ Supported Environments
- **Web Browsers**
- **Unity with SFHttpClient** ([GitHub Repository](https://github.com/padamu1/SFNetworking))

---

## 📝 How to Use

### ✅ Build the Project
Make sure to build the project according to your target operating system.

### ⚙️ Create an `HttpApplication` Instance
Create an instance of `HttpApplication` using the following code:

```csharp
HttpApplication application = new HttpApplication(new SFCSharpServerLib.Common.Data.SFServerInfo()
{
    Url = "*",
    Port = 8000,
});
```

### ➕ Define HTTP Methods
Define the methods you want to handle:

```csharp
application.AddMethod(SFHttpServer.Data.HTTP_METHOD.GET, "/", GetRoot);
application.AddMethod(SFHttpServer.Data.HTTP_METHOD.GET, "/index", GetIndex);
```

### 🔄 Supported HTTP Methods
- **GET** ✅
- **POST** ✅
- **PUT / DELETE** ❌ *(Currently not supported)*

### ⏳ Run the Application
Once all definitions are complete, start the application using:

```csharp
application.RunAsync();
```

---

This library provides a simple way to integrate HTTP communication into your application with minimal setup. 🚀

