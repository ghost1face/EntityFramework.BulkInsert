using SqlServerTypes;
using System;
using System.IO;
using System.Reflection;

namespace EntityFramework.BulkInsert
{
    public static class LibraryLoader
    {
        private static readonly Lazy<bool> loaded = new Lazy<bool>(LoadLibraries);

        public static bool IsLoaded => loaded.Value;

        private static bool LoadLibraries()
        {
            var paths = new[]
            {
                Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory),
                Environment.CurrentDirectory,
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                Environment.SystemDirectory,
            };

            int totalCount = paths.Length;
            int count = 0;

            foreach (var path in paths)
            {
                totalCount++;

                try
                {
                    Utilities.LoadNativeAssemblies(path);
                    break;
                }
                catch (Exception)
                {
                    if (totalCount < count - 1)
                        continue;

                    throw new Exception($"NativeAssemblies cannot be loaded.  Attempted the following paths: {string.Join(";", paths)}");
                }
            }

            return true;
        }
    }
}
