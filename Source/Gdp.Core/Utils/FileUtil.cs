using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading; 
using System.Security.Cryptography;
using System.Text.RegularExpressions; 
//

namespace Gdp.Utils
{
    /// <summary>
    /// 文件和文件夹相关的实用工具
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// GetSHA1
        /// </summary>
        /// <param name="filePath"></param>
        public static string GetSHA1(string filePath)
        {
            try
            {
                FileStream file = new FileStream(filePath, FileMode.Open);
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] retval = sha1.ComputeHash(file);
                file.Close();

                StringBuilder sc = new StringBuilder();
                for (int i = 0; i < retval.Length; i++)
                {
                    sc.Append(retval[i].ToString("x2"));
                }
                //    Console.WriteLine("文件SHA1：{0}", sc);
                return sc.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// MD5 
        /// </summary>
        /// <param name="filePath"></param>
        public static string GetMD5(string filePath)
        {
            try
            {
                FileStream file = new FileStream(filePath, FileMode.Open);
                return GetMD5(file);
            }
            catch (Exception ex)
            {
                try
                {
                    var newPath = PathUtil.GetUniquePath(filePath);
                    File.Copy(filePath, newPath);
                    var result = GetMD5(newPath);
                    Thread.Sleep(10);//休息一下
                    File.Delete(newPath);
                    return result;
                }
                catch (Exception ex2)
                { }
                //   Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// MD5 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetMD5(Stream file)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retval = md5.ComputeHash(file);
            file.Close();

            StringBuilder sc = new StringBuilder();
            for (int i = 0; i < retval.Length; i++)
            {
                sc.Append(retval[i].ToString("x2"));
            }
            // Console.WriteLine("文件MD5：{0}", sc);
            return sc.ToString();
        }

        /// <summary>
        /// 获取输出文件路径。如果输入为目录，则将其与文件名组合。
        /// 如果输出目录不存在，则创建一个。
        /// </summary>
        /// <param name="fileOrDirectory"></param>
        /// <param name="fileNameJustInCase"></param>
        /// <returns></returns>
        public static string GetOutputFilePath(string fileOrDirectory, string fileNameJustInCase = "Output.txt")
        {
            var outputPath = fileOrDirectory;
            var directory = fileOrDirectory;

            if (IsDirectory(fileOrDirectory))//是目录
            {
                outputPath = Path.Combine(directory, Path.GetFileName(fileNameJustInCase));
            }
            else //是文件
            {
                directory = Path.GetDirectoryName(outputPath);
            }
            //确保创建文件夹
            if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

            return outputPath;
        }

        /// <summary>
        /// 获取文件，或目录下的文件，或包含子目录下所有的文件。
        /// 若路径不存在，则返回空的。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="extension">匹配后缀名，多个以分号分开</param>
        /// <param name="loopSubDirectories"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string path, string extension = "*.*", bool loopSubDirectories = false)
        {
            List<string> files = new List<string>();
            if (File.Exists(path)) { files.Add(path); }//is file
            else if (Directory.Exists(path)) // 是目录
            {
                SearchOption option = loopSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                var extensions = extension.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> filePathes = new List<string>();
                foreach (var item in extensions)
                {
                    filePathes.AddRange(Directory.GetFiles(path, item, option));
                }

                files.AddRange(filePathes);
            }
            return files;
        }
        /// <summary>
        /// 删除大小为 0 的文件。
        /// </summary>
        /// <param name="pathes"></param>
        /// <returns></returns>
        public static List<string> RemoveZeroFiles(IEnumerable<string> pathes)
        {
            List<string> results = new List<string>();
            foreach (var path in pathes)
            {
                if (File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    if(info.Length < 1)
                    {
                        TryDeleteFile(path);
                        continue;
                    }
                }
                results.Add(path);
            }
            return results;
        }

        /// <summary>
        /// 从完整的文件路径提取“纯”文件名（不包含后缀）
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtention(string filePath)
        {
            string[] parts = filePath.Split(new char[] { '\\' });
            string fileName = parts[parts.Length - 1];

            int lastIndex = fileName.LastIndexOf('.');
            string pureName = fileName.Substring(0, lastIndex);

            return pureName;
        }

        /// <summary>
        /// 尝试清空目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool TryClearDirectory(string dir)
        {
            try
            {
                 CheckOrCreateDirectory(dir);
                if (Directory.Exists(dir))
                {
                    var files = Directory.GetFiles(dir);
                    foreach (var item in files)
                    { 
                        if (File.Exists(item)) { File.Delete(item); }
                    }
                    var directories = Directory.GetDirectories(dir);
                    foreach (var item in directories)
                    {
                        if (Directory.Exists(item)) { Directory.Delete(item, true); }
                    }
                }

                return true;
            }
            catch (Exception ex) { Console.WriteLine("删除文件出错！" + ex.Message); return false; }
        }

        /// <summary>
        /// 检查目录是否存在，不存在则创建.
        /// 存在或创建成功，返回true，输入不合法或创建失败返回false。
        /// </summary>
        /// <param name="directory">目录</param>
        public static bool CheckOrCreateDirectory(string directory)
        {
            if (String.IsNullOrWhiteSpace(directory)) { return false; }
            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                    return true;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("创建失败！" + directory + " " + ex.Message);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 检查文件是否存在，不存在则创建
        /// </summary>
        /// <param name="absDirectory">目录</param>
        public static void CheckOrCreateFile(string filePath, string content)
        {
            CheckOrCreateDirectory(Path.GetDirectoryName(filePath));
            if (!Directory.Exists(filePath))
            {
                try
                {
                    File.WriteAllText(filePath, content, Encoding.UTF8); ;
                }
                catch (Exception ex)
                {
                   // MessageBox.Show("创建失败！" + filePath + " " + ex.Message);
                }
            }
        }

        #region 写入文件

        /// <summary>
        ///  创建一个新文件，在其中写入指定的字符串，然后关闭文件。如果目标文件已存在，则覆盖该文件。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        public static void WriteText(string path, string text)
        {
            File.WriteAllText(path, text);
        }
        #endregion


 

        /// <summary>
        /// 等待，知道文件已经可以使用
        /// </summary>
        /// <param name="fileName"></param>
        public static void WaitUtilFileReady(string fileName)
        {
            while (!IsReady(fileName))
            {
                Thread.Sleep(300);
            }

        }
        /// <summary>
        /// 文件是否可用，如果不存在或被别的程序使用，则不可用。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsFileReady(string fileName)
        {
            bool ready = false;
            if (File.Exists(fileName))
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                    ready = true;
                }
                catch { ready = false; }
                finally
                {
                    if (fs != null) { fs.Close(); }
                }
                return ready;//by yl  true表示正在使用,false没有使用
            }
            else return false;//文件不存在,肯定不可用
        }
        /// <summary>
        /// 文件是否可用，如果不存在或被别的程序使用，则不可用。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsReady(string fileName)
        {
            bool ready = false;
            if (File.Exists(fileName))
            {
                try
                {
                    File.Move(fileName, fileName);
                    ready = true;
                }
                catch { ready = false; }
                return ready;//by yl  true表示正在使用,false没有使用
            }
            else return false;//文件不存在,肯定不可用
        }


        #region 文件打开
        public static void TryOpenLog(string logFile = "sys.log")
        {
            try { FileUtil.OpenFile(Path.Combine( Setting.ApplicationDirectory, logFile)); }
            catch (Exception ex) {
           //     MessageBox.Show(ex.Message);
            }
        }



        [DllImport("shell32.dll")]
        private static extern int ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        /// <summary>
        /// 指定路径是文件或文件夹,通过存在性。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsLocalFileOrDirectory(string path)
        {
            if (File.Exists(path) || Directory.Exists(path)) { return true; }
            return false;
        }
        /// <summary>
        /// 在Windows系统中打开指定路径的文件或文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void OpenFileOrDirectory(string path)
        {
            if (File.Exists(path))
            {
                OpenFile(path); return;
            }
            else if (Directory.Exists(path))
            {
                OpenDirectory(path); return;
            }
         //   throw new Exception("输入的路径不存在！");
        }
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void OpenFile(string filePath)
        {
            FileInfo fInfo = new FileInfo(filePath);
            string fileName = fInfo.Name;
            string fileDir = fInfo.DirectoryName;
            OpenFile(fileDir, fileName);
        }
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="fileDir"></param>
        /// <param name="fileName"></param>
        public static void OpenFile(string fileDir, string fileName)
        {
            ShellExecute(IntPtr.Zero, "Open", fileName, "", fileDir, 1);
        }
        /// <summary>
        /// 打开文件的当前目录
        /// </summary>
        /// <param name="filePath"></param>
        public static void OpenFileDirectory(string filePath)
        {
            FileInfo fInfo = new FileInfo(filePath);
            string fileDir = fInfo.DirectoryName;
            OpenDirectory(fileDir);
        }
        /// <summary>
        /// 打开文件件
        /// </summary>
        /// <param name="dir"></param>
        public static void OpenDirectory(string dir)
        {
            if (File.Exists(dir))
            {
                dir = Path.GetDirectoryName(dir);
            }
            ShellExecute(IntPtr.Zero, "Open", "", "", dir, 1);
        }
        #endregion


        /// <summary>
        /// 获取程序集所在文件夹路径
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyFolderPath()
        {
            const string _PREFIX = @"file:///";
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

            codeBase = codeBase.Substring(_PREFIX.Length, codeBase.Length - _PREFIX.Length).Replace("/", "\\");
            return System.IO.Path.GetDirectoryName(codeBase) + @"\";

        }

        /// <summary>
        ///如果是文件且存在，则返回false。
        ///如果目录存在，则返回true。
        ///后续通过字符串判断是否是目录，并不检查目录的存在性。依据如果以斜杠/或\结尾，则为目录。如果最后一个斜杠后面没有后缀名，则为目录。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public bool IsDirectory(string path)
        {
            path = path.Trim();
            if (Directory.Exists(path)) { return true; }
            if (File.Exists(path)) { return false; }

            if (path.EndsWith("/") || path.EndsWith("\\"))
            {
                return true;
            }
            if (Path.GetExtension(path).Length > 10) return true;

            if (!Path.HasExtension(path)) return true;

            return false;
        }

        /// <summary>
        /// 复制文件或者目录。
        /// </summary>
        /// <param name="fromFileOrDirectory"></param>
        /// <param name="toFileOrDirectory"></param>
        public static bool MoveFileOrDirectory(string fromFileOrDirectory, string toFileOrDirectory, bool overwrite = true)
        {
            if (File.Exists(fromFileOrDirectory))//源是文件
            {
                var dest = toFileOrDirectory;
                bool isDirecory = IsDirectory(toFileOrDirectory);

                if (isDirecory)
                {
                    dest = Path.Combine(toFileOrDirectory, Path.GetFileName(fromFileOrDirectory));
                }

                MoveFile(fromFileOrDirectory, dest, overwrite);
            }
            else //源是目录，目标必须是目录
            {
                var files = Directory.GetFiles(fromFileOrDirectory);
                foreach (var item in files)
                {
                    var dest = Path.Combine(toFileOrDirectory, Path.GetFileName(item));

                    MoveFile(fromFileOrDirectory, dest, overwrite);
                }
            }
            return true;
        }
        /// <summary>
        /// 复制或移动文件
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <param name="isCopyOrMove"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static bool CopyOrMoveFile(string sourcePath, string destPath,bool isCopyOrMove, bool overwrite = true)
        {
            if (isCopyOrMove) { return  CopyFile(sourcePath, destPath, overwrite);}
            else { return MoveFile(sourcePath, destPath, overwrite); }
        }


        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <param name="overwrite"></param>
        public static bool MoveFile(string sourcePath, string destPath, bool overwrite = true)
        {
            if (!File.Exists(sourcePath)) return false;

            var destDirectory = Path.GetDirectoryName(destPath);
            CheckOrCreateDirectory(destDirectory);

            if (File.Exists(destPath))
            {
                if (overwrite) { File.Delete(destPath); }
                else return false;
            }

            File.Move(sourcePath, destPath);
            return true;
        }

        /// <summary>
        /// 复制文件或者目录。
        /// </summary>
        /// <param name="fromFileOrDirectory"></param>
        /// <param name="toFileOrDirectory"></param>
        /// <param name="overwrite"></param>
        public static bool CopyFileOrDirectory(string fromFileOrDirectory, string toFileOrDirectory, bool overwrite = true)
        {
            if (File.Exists(fromFileOrDirectory))//源是文件
            {
                bool isDirecory = IsDirectory(toFileOrDirectory);
                var dest = toFileOrDirectory;
                if (isDirecory)
                {
                    dest = Path.Combine(toFileOrDirectory, Path.GetFileName(fromFileOrDirectory));
                }
                return CopyFile(fromFileOrDirectory, dest, overwrite);
            }
            else //源是目录，目标必须是目录
            {
                var files = Directory.GetFiles(fromFileOrDirectory);
                foreach (var item in files)
                {
                    CheckOrCreateDirectory(toFileOrDirectory);
                    CopyFileOrDirectory(item, Path.Combine(toFileOrDirectory, Path.GetFileName(item)), overwrite);
                }
            }
            return true;
        }
        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="fromFile"></param>
        /// <param name="dest"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static bool CopyFile(string fromFile, string dest, bool overwrite)
        {
            if ((File.Exists(dest) && overwrite) || !File.Exists(dest))
            {
                CheckOrCreateDirectory(Path.GetDirectoryName(dest));
                File.Copy(fromFile, dest, overwrite);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// 删除文件或目录
        /// </summary>
        /// <param name="fileOrDirectory"></param>
        public static void TryDeleteFileOrDirectory(string fileOrDirectory)
        {
            try
            {
                if (Directory.Exists(fileOrDirectory)) { Directory.Delete(fileOrDirectory, true); }
                if (File.Exists(fileOrDirectory)) { File.Delete(fileOrDirectory); }
            }
            catch (Exception ex)
            {
                new Gdp.IO.Log(typeof(FileUtil)).Error(fileOrDirectory + "删除失败！ " + ex.Message);
            }
        }
        /// <summary>
        /// 删除文件或目录
        /// </summary>
        /// <param name="filePath"></param>
        public static void TryDeleteFile(string filePath)
        {
            try
            { 
                if (File.Exists(filePath)) { File.Delete(filePath); }
            }
            catch (Exception ex)
            {
                new Gdp.IO.Log(typeof(FileUtil)).Error(filePath + "删除失败！ " + ex.Message);
            }
        }

        /// <summary>
        /// 获取路径的目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDirectory(string path)
        {
            var isDirectory = IsDirectory(path);

            if (isDirectory)
            {
                return path;
            }
            return Path.GetDirectoryName(path);
        }
        /// <summary>
        /// 文件是否有效，包括是否存在和大小是否为0.
        /// </summary>
        /// <param name="navPath"></param>
        /// <returns></returns>
        public static bool IsValid(string navPath)
        {
            return !String.IsNullOrWhiteSpace(navPath) && (File.Exists(navPath) && new FileInfo(navPath).Length > 0);

        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Exists(string path) { return File.Exists(path); }
    }
}
