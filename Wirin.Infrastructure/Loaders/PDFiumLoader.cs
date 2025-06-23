using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Wirin.Infrastructure.Loaders
{
    public static class PDFiumLoader
    {
        private static bool _initialized = false;
        private static readonly object _lock = new object();

        public static void Initialize()
        {
            if (_initialized)
                return;

            lock (_lock)
            {
                if (_initialized)
                    return;

                try
                {
                    Console.WriteLine("=== Iniciando carga de PDFium ===");
                    
                    // Determinar la arquitectura del sistema
                    string architecture = Environment.Is64BitProcess ? "x64" : "x86";
                    Console.WriteLine($"Arquitectura detectada: {architecture}");
                    Console.WriteLine($"SO detectado: {RuntimeInformation.OSDescription}");
                    
                    // Obtener la ruta base de la aplicación
                    string basePath = AppContext.BaseDirectory;
                    Console.WriteLine($"Ruta base de la aplicación: {basePath}");
                    
                    // Construir la ruta al archivo nativo según el SO
                    string libraryPath;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        libraryPath = Path.Combine(basePath, "NativeBinaries", architecture, "pdfium.dll");
                        Console.WriteLine("Plataforma Windows detectada");
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        libraryPath = Path.Combine(basePath, "NativeBinaries", architecture, "libpdfium.so");
                        Console.WriteLine("Plataforma Linux detectada");
                    }
                    else
                    {
                        throw new PlatformNotSupportedException("Plataforma no soportada");
                    }
                    
                    Console.WriteLine($"Ruta de librería calculada: {libraryPath}");
                    
                    // Verificar si el archivo existe
                    if (!File.Exists(libraryPath))
                    {
                        Console.WriteLine($"Archivo no encontrado en ruta principal: {libraryPath}");
                        
                        // Intentar buscar en ubicaciones alternativas
                        string altPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
                            ? Path.Combine(Directory.GetCurrentDirectory(), "NativeBinaries", architecture, "pdfium.dll")
                            : Path.Combine(Directory.GetCurrentDirectory(), "NativeBinaries", architecture, "libpdfium.so");
                        
                        Console.WriteLine($"Intentando ruta alternativa: {altPath}");
                        
                        if (File.Exists(altPath))
                        {
                            libraryPath = altPath;
                            Console.WriteLine("Archivo encontrado en ruta alternativa");
                        }
                        else
                        {
                            Console.WriteLine("Archivo no encontrado en ninguna ubicación");
                            throw new FileNotFoundException($"No se pudo encontrar el archivo PDFium en {libraryPath} ni en {altPath}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Archivo PDFium encontrado correctamente");
                    }
                    
                    Console.WriteLine($"Cargando PDFium desde: {libraryPath}");
                    
                    // Cargar la biblioteca nativa
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        Console.WriteLine("Cargando librería usando LoadLibrary (Windows)");
                        LoadLibraryWindows(libraryPath);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Console.WriteLine("Cargando librería usando dlopen (Linux)");
                        LoadLibraryLinux(libraryPath);
                    }
                    
                    _initialized = true;
                    Console.WriteLine("=== PDFium inicializado correctamente ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al inicializar PDFium: {ex.Message}");
                    throw;
                }
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("libdl.so.2", CharSet = CharSet.Ansi)]
        private static extern IntPtr dlopen(string filename, int flags);

        [DllImport("libdl.so.2", CharSet = CharSet.Ansi)]
        private static extern IntPtr dlerror();

        private const int RTLD_NOW = 2;

        private static void LoadLibraryWindows(string dllPath)
        {
            IntPtr result = LoadLibrary(dllPath);
            if (result == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Exception($"Error al cargar la biblioteca nativa: {dllPath}. Código de error: {errorCode}");
            }
        }

        private static void LoadLibraryLinux(string libraryPath)
        {
            IntPtr result = dlopen(libraryPath, RTLD_NOW);
            if (result == IntPtr.Zero)
            {
                IntPtr errorPtr = dlerror();
                string error = errorPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(errorPtr) : "Error desconocido";
                throw new Exception($"Error al cargar la biblioteca nativa: {libraryPath}. Error: {error}");
            }
            Console.WriteLine($"Biblioteca cargada exitosamente: {libraryPath}");
        }
    }
}