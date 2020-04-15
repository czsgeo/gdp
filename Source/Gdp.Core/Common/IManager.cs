//2015.05.09, czs, create in namu, 管理器模式总接口

using System; 
using Gdp.IO;

namespace Gdp
{
    /// <summary>
    /// 管理器总接口。管理器负责对一个对象反复折腾。
    /// </summary>
    public interface IManager
    { 
    }


    /// <summary>
    /// 管理器负责对一个对象反复折腾。管理器。管理器模式，将对一个类的所有对象的管理封装到一个单独的管理器类中。
    /// 这使得管理职责的变化独立于类本身，并且管理器还可以为不同的类进行重用。
    /// </summary>
    /// <typeparam name="TMaster">待处理类型</typeparam>
    public interface IManager<TMaster> : IManager
    {
       
    }

    /// <summary>
    /// 管理器负责对一个对象反复折腾。管理器。管理器模式，将对一个类的所有对象的管理封装到一个单独的管理器类中。
    /// 这使得管理职责的变化独立于类本身，并且管理器还可以为不同的类进行重用。
    /// </summary>
    /// <typeparam name="TMaster">待处理类型</typeparam>
    public abstract class Manager<TMaster> : IManager<TMaster>
    {
      /// <summary>
        /// 日志记录。错误信息记录在日志里面。
        /// </summary>
        protected ILog log = Log.GetLog(typeof(Manager<TMaster>));

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 执行信息
        /// </summary>
        public string Message { get; set; } 
    }
     
}
