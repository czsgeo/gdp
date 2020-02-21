//2015.10.21, czs, create in hongqing, 路径工具

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
 

namespace Gdp.Utils
{
    /// <summary>
    /// 路径工具
    /// </summary>
    public static class PathUtil
    {
        /// <summary>
        /// 返回本地一个可用的文件名称，如果不存在，则直接返回；如果已经有了，则依次编号下去。
        /// </summary>
        /// <param name="fileNameOrPath"></param>
        /// <returns></returns>
        public static string GetAvailableName(string fileNameOrPath)
        {
            if (!File.Exists(fileNameOrPath))
            { return fileNameOrPath; }

            var fileName = Path.GetFileNameWithoutExtension(fileNameOrPath);
            var fileExtension = Path.GetExtension(fileNameOrPath);
            var directory = Path.GetDirectoryName(fileNameOrPath);

            if (fileName.Contains("(") && fileName.Contains(")"))
            {
                var numString = RegexUtil.GetLastBraketContent(fileName);
                if (numString != null)
                {
                    var num = IntUtil.TryParse(numString) + 1;
                    fileName = fileName.Remove(fileName.IndexOf("(")) + "(" + num + ")";
                }

            }
            else
            {
                fileName += "(1)";
            }

            var newPath = Path.Combine(directory, fileName + fileExtension);

            return GetAvailableName(newPath);
        }

        /// <summary>
        /// 获取第一个匹配的路径
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        static public string GetMatchedPath(List<string> inputs, string extension)
        {
            List<string> pathes = GetMatchedPathes(inputs, extension);
            if (pathes.Count != 0)
                return pathes[0];

            return null;
        }
        /// <summary>
        /// 获取匹配的路径
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        static public List<string> GetMatchedPathes(List<string> inputs, string extension)
        {
            List<string> mathed = new List<string>();
            foreach (var item in inputs)
            {
                if (PathUtil.IsFileExtensionMatched(item, extension))
                {
                    mathed.Add(item);
                }
            }
            return mathed;
        }

        /// <summary>
        /// 获取一个全局唯一的名称。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetUniquePath(string path)
        {
           var extension = Path.GetExtension(path);

           var guid = Guid.NewGuid().ToString().Replace("-", "");
           var newName = Path.GetFileNameWithoutExtension(path) + guid + extension;

           var directory = Path.GetDirectoryName(path);
           var newPath = Path.Combine(directory, newName);
           return newPath;
        }
        /// <summary>
        /// 匹配获取包含输入路径的一个目录，再返回子目录，如 C:\Test\ 目录和C:\Test\Sub\Test.txt,将返回Sub
        /// </summary>
        /// <param name="directories"></param>
        /// <param name="inputPath"></param>
        /// <returns></returns>
        public static string GetSubDirectory(IEnumerable<string> directories, string inputPath)
        {
            string topDirectory = Gdp.Utils.PathUtil.GetTopDirectory(directories, inputPath);
            if(String.IsNullOrWhiteSpace( topDirectory)) { return String.Empty; }
            return Gdp.Utils.PathUtil.GetSubDirectory(topDirectory, inputPath);
        }

        /// <summary>
        /// 匹配获取包含输入路径的一个目录，再返回子目录，如 C:\Test\目录和C:\Test\Sub\Test.txt,将返回Sub
        /// </summary> 
        /// <param name="inputPath"></param>
        /// <param name="subCount">几级目录</param>
        /// <returns></returns>
        public static string GetSubDirectory(string inputPath, int subCount = 1)
        {
            string subPath = inputPath;
            string result = "";
            for (int i = 0; i < subCount; i++)
            {
                subPath = Path.GetDirectoryName(subPath);
                var sub = System.IO.Path.GetFileName(subPath);

                result = Path.Combine(sub, result);
            }
            return result;
        }
        /// <summary>
        /// 获取包含输入路径的一个目录
        /// </summary>
        /// <param name="directories"></param>
        /// <param name="inputPath"></param>
        /// <returns></returns>
        public static string GetTopDirectory(IEnumerable<string> directories, string inputPath)
        {
            string topDirectory = null;
            foreach (var item in directories)
            {
                if (inputPath.Contains(item))
                {
                    topDirectory = item;
                    break;
                }
            }
            return topDirectory;
        }
        /// <summary>
        /// 返回子目录，如 C:\Test\目录和C:\Test\Sub\Test.txt,将返回Sub
        /// </summary>
        /// <param name="topDirectory"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetSubDirectory(string topDirectory, string filePath)
        {
            if(topDirectory == null) { return ""; }

            string fileName = Path.GetFileName(filePath);
            var sub = filePath.Replace(topDirectory, "").Replace(fileName, "");
            return sub.TrimStart('\\').TrimEnd('\\').TrimStart('/').TrimEnd('/');
        }


        /// <summary>
        /// 是否为同一个路径。只从字符串判断，并不判断相对路径和绝对路径。
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static  bool IsSamePath(string path1, string path2)
        {
            if(String.IsNullOrWhiteSpace(path1) || String.IsNullOrWhiteSpace(path2)){
                throw new Exception("输入地址无效！" + path1 + path2);
            }

            path1 = path1.Trim().TrimEnd('/','\\').Replace("\\","/");//目录情况
            path2 = path2.Trim().TrimEnd('/','\\').Replace("\\","/");

            if (String.Equals(path1, path2, StringComparison.CurrentCultureIgnoreCase)) { return true; }

            return false;
        }



        /// <summary>
        /// 是否匹配类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="destType">如“*O; ZIP”或“*.*O,*.ZIP”,多个类型以分号分隔</param>
        /// <returns></returns>
        public static bool IsFileExtensionMatched(string fileName, string destType)
        {
            if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(destType))
            {
                throw new ArgumentNullException("路径和参数都必须赋值！");
            }

            var sourceType = Path.GetExtension(fileName).TrimStart('.').ToUpper();//仅文件后缀

            return IsFileTypeMatched(sourceType, destType);
        }
        /// <summary>
        /// 检查是否匹配目标类型
        /// </summary>
        /// <param name="destType">目标类型,多个类型以分号分隔</param>
        /// <param name="sourceType">源路径或后缀名</param>
        /// <returns></returns>
        public static bool IsFileTypeMatched( string sourceType, string destType)
        {
            var destTypes = destType.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in destTypes)
            {
                var ismatched = IsMatchedSingleFileType(sourceType, item);
                if (ismatched) return true;
            }

            return false;
        }
        
        /// <summary>
        /// 检查是否匹配类型
        /// </summary>
        /// <param name="destType">目标类型,支持多个类型</param>
        /// <param name="sourceType">源路径或后缀名</param>
        /// <returns></returns>
        public static bool IsFileTypeMatched(string sourceType, params string [] destTypes)
        {
            foreach (var item in destTypes)
            {
                var ismatched = IsMatchedSingleFileType(sourceType, item);
                if (ismatched) return true;
            }

            return false;

        }
        /// <summary>
        /// 匹配唯一个后缀名
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destType"></param>
        /// <returns></returns>
        private static bool IsMatchedSingleFileType(string sourceType,   string destType)
        {
            if (destType.StartsWith("*.")) { destType = destType.Substring(2); }
            var pattern = destType.ToUpper() + "$";
            if (destType.StartsWith("*"))//具有通配符开始
            {
                pattern = "." + destType;
            }

            Match m = Regex.Match(sourceType, pattern);//.为通配符，
            return m.Success;
        }
         

        /// <summary>
        /// 获取绝对路径，如果是，则直接返回。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="absDirectory"></param>
        /// <returns></returns>
        public static string GetAbsPath(string path, string absDirectory)
        {
            if (String.IsNullOrWhiteSpace(path)) return absDirectory;
            if (path.Contains(":")) { return path; }
            if (Path.IsPathRooted(path)) { return path; }

            return Path.Combine(absDirectory, path);
        }
        /// <summary>
        /// 获取相对路径，如果是，则直接返回。并不检查文件的存在性
        /// </summary>
        /// <param name="path"></param>
        /// <param name="absDirectory"></param>
        /// <returns></returns>
        public static string GetRelativePath(string path, string absDirectory)
        {
            if (String.IsNullOrWhiteSpace(path)) return absDirectory;
            if (!Path.IsPathRooted(path)) { return path; }
            if (path.Contains(":")) return path;

            return path.Replace(absDirectory, "").TrimStart(new char[]{'/', '\\'});
        }

        /// <summary>
        /// 获取后缀，从最后一个往前，0为默认最后一个。以点“.”为分隔符。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetExtension(string path, int index = 0)
        {
            var extension = Path.GetExtension(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            for (int i = 0; i < index; i++)
            {
                extension = Path.GetExtension(fileName);
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }
            return extension;
        }
        /// <summary>
        /// 是否是有效的路径。例如如果为空，或则包含了不允许字母如分号";",竖号"|"等则认为是非路径。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsValidPath(string path)
        {
            if (String.IsNullOrWhiteSpace(path)) return false;
            if (path.Contains(';') || path.Contains('|')) return false;

            return true;
        }

        internal static bool IsFile(string path)
        {
            if (path.EndsWith(@"\") || path.EndsWith(@"/"))
            {
                return false;
            }
            if (Path.GetExtension(path).Length > 0) 
                return true;
            return false;
        }
    }
}
