//2016.11.08, czs, create in hongqing, 增加 FTP 增删改查

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Gdp.Utils
{
    /// <summary>
    /// 网络工具
    /// </summary>
    public static class NetUtil
    {
        static Gdp.IO.ILog log = Gdp.IO.Log.GetLog(typeof(NetUtil));
        
        #region  FTP 操作
        /// <summary>
        ///  ftp的上传功能
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="ftpPath"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        static public void FtpUpload(string localFilePath, string ftpPath, string userName = "Anonymous", string password = "User@")  
       {
           log.Info("即将上传 " + localFilePath + " 到 " + ftpPath);

           ParseFtpPath(ref ftpPath, ref userName, ref password);

           FileInfo fileInf = new FileInfo(localFilePath);  
           FtpWebRequest reqFTP;  
           // 根据uri创建FtpWebRequest对象   
           reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath + fileInf.Name));  
           // ftp用户名和密码  
           reqFTP.Credentials = new NetworkCredential(userName, password);  
  
           reqFTP.UsePassive = false;  
           // 默认为true，连接不会被关闭  
           // 在一个命令之后被执行  
           reqFTP.KeepAlive = false;  
           // 指定执行什么命令  
           reqFTP.Method = WebRequestMethods.Ftp.UploadFile;  
           // 指定数据传输类型  
           reqFTP.UseBinary = true;  
           // 上传文件时通知服务器文件的大小  
           reqFTP.ContentLength = fileInf.Length;  
           // 缓冲大小设置为2kb  
           int buffLength = 2048;  
           byte[] buff = new byte[buffLength];  
           int contentLen;  
           // 打开一个文件流 (System.IO.FileStream) 去读上传的文件  
           FileStream fs = fileInf.OpenRead();  
           try  
           {  
               // 把上传的文件写入流  
               Stream strm = reqFTP.GetRequestStream();  
               // 每次读文件流的2kb  
               contentLen = fs.Read(buff, 0, buffLength);  
               // 流内容没有结束  
               while (contentLen != 0)  
               {  
                   // 把内容从file stream 写入 upload stream  
                   strm.Write(buff, 0, contentLen);  
                   contentLen = fs.Read(buff, 0, buffLength);  
               }  
               // 关闭两个流  
               strm.Close();  
               fs.Close();  
           }  
           catch (Exception ex)  
           {
               log.Error("FTP 上传出错。" + ex.Message + "\r\n" + localFilePath);
           }  
       }  
   
        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="ftpPath"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        static public void Delete(string ftpPath, string userName = "Anonymous", string password = "User@")  
        {
           ParseFtpPath(ref ftpPath, ref userName, ref password);

           FtpWebRequest reqFTP;  
           try  
           {   
               reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath));  
               reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;  
               reqFTP.UseBinary = true;  
               reqFTP.Credentials = new NetworkCredential(userName, password);  
               reqFTP.UsePassive = false;  
               FtpWebResponse listResponse = (FtpWebResponse)reqFTP.GetResponse();  
               string sStatus = listResponse.StatusDescription;  
           }  
           catch (Exception ex)  
           {  
               throw ex;  
           }  
       }  

        /// <summary>
        /// 获取FTP文件地址。
        /// </summary>
        /// <param name="ftpFolderOrFilePath"></param>
        /// <param name="extension"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static List<string> GetFtpFileUrls(string ftpFolderOrFilePath, string extension, string userName, string password)
        {
            List<string> fileUrlPathes = new List<string>();
            //Uri uri = new Uri(ftpFolderOrFilePath);
            if (PathUtil.IsFile(ftpFolderOrFilePath))
            {
                fileUrlPathes.Add(ftpFolderOrFilePath);
            }
            else
            {
                fileUrlPathes.AddRange(NetUtil.GetFtpFileList(ftpFolderOrFilePath, extension, userName, password));
            }
            return fileUrlPathes;
        }
        /// <summary>
        /// 获取指定目标下的文件路径。
        /// </summary>
        /// <param name="ftpFolderPath">目录路径，含IP地址和端口，请以"/"结尾</param>
        /// <param name="extension">，可以以分号分隔多个匹配类型</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        static public List<string> GetFtpFileList(string ftpFolderPath, string extension = "*.*", string userName = "anonymous", string password = "")
        {
            ParseFtpPath(ref ftpFolderPath, ref userName, ref password);

            Uri uri = new Uri(ftpFolderPath);
             
            FtpClient client = new FtpClient(uri, userName, password);
            client.RemotePath =uri.AbsolutePath;//.GetLeftPart(UriPartial.Path); 
            var results = client.GetFileList(extension);

            List<string> pathes = new List<string>();
            foreach (var item in results)
            {
                pathes.Add(Path.Combine(ftpFolderPath, item));
            }

            return pathes;
        }

        /// <summary>
        /// 下载，返回本地路径。这是最智能化的处理函数。如果没有指定用户名和密码，则自动采用 Anonymous 用户名。
        /// </summary>
        /// <param name="ftpFolderOrFilePath">路径，含IP地址和端口，若是目录，请以"/"结尾</param>
        /// <param name="extension">若是目录，则设置，可以以分号分隔多个匹配类型</param>
        /// <param name="localFolder"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static List<string> DownloadFtpDirecotryOrFile(string ftpFolderOrFilePath, string extension = "*.*", string localFolder = @"C:\GnsserTemp\", string userName = "anonymous", string password = "User@", bool IsOverwrite = false, bool throwException = false)
        {
            ParseFtpPath(ref ftpFolderOrFilePath, ref userName, ref password);

            List<string> fileUrlPathes = new List<string>();
            //Uri uri = new Uri(ftpFolderOrFilePath);
            if (PathUtil.IsFile(ftpFolderOrFilePath))
            {
                fileUrlPathes.Add(ftpFolderOrFilePath);
            }
            else
            {
                fileUrlPathes.AddRange(GetFtpFileList(ftpFolderOrFilePath, extension, userName, password));
            }


            List<string> localFilePathes = new List<string>();
            foreach (var url in fileUrlPathes)
            {
                var localPath = Path.Combine(localFolder, Path.GetFileName(url));
                if (FtpDownload(url, localPath, IsOverwrite, userName, password, throwException))
                {
                    localFilePathes.Add(localPath);

                }
            }

            return localFilePathes;
        }
        /// <summary>
        /// 解析FTP路径，提取用户名和密码
        /// </summary>
        /// <param name="ftpPath"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        private static void ParseFtpPath(ref string ftpPath, ref string userName, ref string password)
        {
            //可能自带了用户名和密码，而未指定
            if (ftpPath.Contains("@"))
            {
                var startIndex = ftpPath.IndexOf("//");
                var endIndex = ftpPath.IndexOf("@");
                var strs = ftpPath.Substring(startIndex + 1, endIndex - startIndex - 1);
                var strarray = strs.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                userName = strarray[0];
                password = strarray[1];

                ftpPath = "ftp://" + ftpPath.Substring(endIndex + 1);
            }
        }

        /// <summary>
        /// FTP 下载,单文件对单文件的下载。
        /// </summary>
        /// <param name="urlpath">Url 地址</param>
        /// <param name="savePath">保存地址</param>
        /// <param name="userName">ftp用户名</param>
        /// <param name="password">ftp密码</param>
        public static bool FtpDownload(string urlpath, string savePath,  bool IsOverwrite = false, string userName = "anonymous", string password = "User@", bool throwException = false)
        {
            log.Info("正在尝试下载 " + urlpath + " 到 " + savePath);

            ParseFtpPath(ref urlpath, ref userName, ref password);

            if (String.IsNullOrEmpty(userName)) { userName = "anonymous"; }
            if (String.IsNullOrEmpty(password)) { password = "User@"; }

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(urlpath);
            if (userName != "Anonymous") { request.Credentials = new NetworkCredential(userName, password); }
            //增加下面两个属性即可
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            //request.ProtocolVersion = HttpVersion.Version10;
            //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            request.UseBinary = true;
          //  request.UsePassive = false;
            request.UsePassive = true;
            request.Timeout = 20 * 1000;
            try
            {
                //检查目录的存在性，并创建 
                Gdp.Utils.FileUtil.CheckOrCreateDirectory(Path.GetDirectoryName(savePath));  

                //检查文件是否存在，如果不覆盖，文件存在，且大小为10byte以上，则不下载。
                if (!IsOverwrite && File.Exists(savePath))
                {
                    var fileInfo = new FileInfo(savePath);
                    if (fileInfo.Length > 10L)
                    {
                        log.Info("已经存在 " + savePath + "，且设置为不覆盖，不必下载 ");
                        return true;
                    }
                }
                

                FtpWebResponse respose = (FtpWebResponse)request.GetResponse();
                
            using (Stream ftpStream = respose.GetResponseStream())
                {
                using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
                    {
                        int size = 1024;
                        byte[] buffer = new byte[size];
                        int count = 0;
                        while ((count = ftpStream.Read(buffer, 0, size)) > 0)
                        {
                            fileStream.Write(buffer, 0, count);
                            fileStream.Flush();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("下载失败! " + urlpath + " " + ex.Message);

                if (throwException) { throw ex; }
               
                return false;
            }
            log.Info("成功下载 " + urlpath + " 到 " + savePath);
            return true;
        }
        #endregion

        #region 其它网络方法
        /// <summary>
        /// 采用WebClient直接下载
        /// </summary>
        /// <param name="urlpath"></param>
        /// <param name="savePath"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static bool Download(string urlpath, string savePath, bool throwException = false)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadFile(urlpath, savePath);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                if (throwException) { throw ex; }

                return false;
            }
            return true;
        }
        public static bool DownloadAsync(string urlpath, string savePath, bool throwException = false)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadFileAsync( new Uri( urlpath), savePath);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                if (throwException) { throw ex; }

                return false;
            }
            return true;
        }
        /// <summary>
        /// 下载网络字符串
        /// </summary>
        /// <param name="urlpath"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static string GetNetString(string urlpath, bool throwException = false)
        {
            try
            {
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                return client.DownloadString(urlpath);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                if (throwException) { throw ex; }

            }
            return "";
        }
        /// <summary>
        /// 下载网络字符串,并解析为Double
        /// </summary>
        /// <param name="urlpath"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static double GetNetDouble(string urlpath, bool throwException = false)
        {
            try
            { 
                var val = GetNetString(urlpath, throwException);
                var va= val.Replace("\"", "");
                return double.Parse(va);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                if (throwException) { throw ex; }

            }
            return 0;
        }

        /// <summary>
        /// Http 下载
        /// </summary>
        /// <param name="urlpath">Url 地址</param>
        /// <param name="savePath">保存地址</param>
        /// <param name="throwException">是否抛出异常</param> 
        public static bool HttpDownload(string urlpath, string savePath, bool throwException = false)
        {
            HttpWebRequest request = (HttpWebRequest)FtpWebRequest.Create(urlpath); 
            try
            {
                //检查目录的存在性，并创建
                var saveDir = Path.GetDirectoryName(savePath); 
                Gdp.Utils.FileUtil.CheckOrCreateDirectory(saveDir);

                HttpWebResponse respose = (HttpWebResponse)request.GetResponse();
            using (Stream ftpStream = respose.GetResponseStream())
                {
                using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
                    {
                        int size = 512;
                        byte[] buffer = new byte[size];
                        int count = 0;
                        while ((count = ftpStream.Read(buffer, 0, size)) > 0)
                        {
                            fileStream.Write(buffer, 0, count);
                            fileStream.Flush();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (throwException) { throw ex; }

                return false;
            }
            return true;
        }

        /// <summary>
        /// get the Ip of currentItem computor.
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            string ipStr = "";
            string hostName = System.Net.Dns.GetHostName();
            System.Net.IPAddress[] addessList = System.Net.Dns.GetHostEntry(hostName).AddressList;
            for (int i = 0; i < addessList.Length; i++)
            {
                ipStr += addessList[i].ToString() + ", ";
            }
            return ipStr;
        }
        public static string GetFirstIp()
        { 
            string hostName = System.Net.Dns.GetHostName();
            System.Net.IPAddress[] addessList = System.Net.Dns.GetHostEntry(hostName).AddressList;
            return addessList[0].ToString();
        }
        public static bool IsIp(string ip)
        {
            Match m = Regex.Match(ip, "(\\d{1,3}\\.){3}\\d{1,3}");
            return m.Success;
        }

        public static IPStatus PingResult(string hostNmeOrAddress)
        {
            try
            {
                IPAddress ip = Dns.GetHostAddresses(hostNmeOrAddress)[0];
                System.Net.NetworkInformation.Ping ping = new Ping();
                PingOptions pingOptions = new PingOptions();
                pingOptions.DontFragment = true;

                string data = "aa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                //Watie fraction for a reply.
                int timeout = 1000;
                PingReply reply = ping.Send(ip, timeout, buffer, pingOptions);
                return reply.Status;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return IPStatus.NoResources;
            }
        }


        #region using dll
        [DllImport("wininet.dll")]
        public extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        /// <summary>
        /// 网络是否可用
        /// </summary>
        /// <returns></returns>
        public static bool IsConnected()
        {
            int i = 0; 
            bool state = InternetGetConnectedState(out i, 0);
            return state;
        }
        /// <summary>
        /// 是否连接了国际互联网，通过百度网站测试。
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectedToInternet(){
          bool isConnected =Gdp.Utils.NetUtil.IsConnected();
          if (isConnected)
          {
              IPStatus status = NetUtil.PingResult("www.baidu.com");
              if (status != IPStatus.Success)
              {
                  return false;
              }
              return true;
          }
          return false;
        }

        #endregion
        #endregion

    }
}
