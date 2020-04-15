//2015.05.10, czs, create in namu, 路径服务
//2019.01.06, czs, edit in hmx, 增加时间分辨率，修复IGS超快星历访问

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// 文件路径服务。通过一些条件提供路径服务。
    /// </summary>
    public class TimeBasedFilePathBuilder : AbstractService<FileOption, Time>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pathModel">路径模板</param>
        public TimeBasedFilePathBuilder(string pathModel) : this(new List<string> { pathModel }) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pathModels">路径模板，安装默认顺序</param>
        public TimeBasedFilePathBuilder(List<string> pathModels) { this.PathModels = (pathModels); }
           
        /// <summary>
        /// 路径模型
        /// </summary>
        List<string> PathModels { get; set; }
        /// <summary>
        /// 根据时间获取路径
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public override FileOption Get(Time time)
        { 
            ELMarkerReplaceService service = new ELMarkerReplaceService(ELMarkerReplaceService.GetTimeKeyWordDictionary(time));
            var list = service.Get(PathModels); 

            return new FileOption(list);
        } 
    }
}