using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XExten.Advance.LinqFramework;

namespace XExten.Advance.InternalFramework.FileHandle
{
    internal class FileManager
    {
        internal static string CreateFile(string path)
        {
            if (File.Exists(path) == false) File.Create(path).Dispose();
            return path;
        }

        internal static string CreateDir(string path)
        {
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            return path;
        }

        internal static void DeleteFolder(string path)
        {
            if (Directory.Exists(path))
            {
                string[] fileSystemEntries = Directory.GetFileSystemEntries(path);
                for (int i = 0; i < fileSystemEntries.Length; i++)
                {
                    string text = fileSystemEntries[i];
                    if (File.Exists(text))
                    {
                        File.Delete(text);
                    }
                    else
                    {
                        DeleteFolder(text);
                    }
                }
                Directory.Delete(path);
            }
        }

        internal static void DeleteFile(string path)
        {
            if (File.Exists(path) == true) File.Delete(path);
        }

        internal static void DeleteDirFile(string path, params string[] condition)
        {
            string[] Files = { };

            if (Directory.Exists(path)) Files = Directory.GetFiles(path);
            if (File.Exists(path)) Files = Directory.GetFiles(Path.GetDirectoryName(path));

            if (Files.Length > 0)
            {
                if (condition.Length <= 0)
                    Files.ForArrayEach<string>(t => DeleteFile(t));
                else
                    Files.Where(t => !condition.Contains(Path.GetFileName(t))).ForEnumerEach(t => DeleteFile(t));
            }
        }

        internal static string WriteFile(byte[] bytes, string path)
        {
            Stream stream = new MemoryStream(bytes);
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(fs);
            writer.Write(bytes);
            writer.Close();
            fs.Close();
            stream.Close();
            stream.Dispose();
            return path;
        }

        internal static string WriteDirFile(byte[] bytes, string path)
        {
            var dir = Path.GetDirectoryName(path);
            CreateDir(dir);
            Stream stream = new MemoryStream(bytes);
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(fs);
            writer.Write(bytes);
            writer.Close();
            fs.Close();
            stream.Close();
            stream.Dispose();
            return path;
        }

        internal static string ReadFile(string path)
        {
            if (File.Exists(path))
            {
                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                fileStream.Close();
                return Encoding.UTF8.GetString(bytes);
            }
            else
                return null;
        }
    }
}
