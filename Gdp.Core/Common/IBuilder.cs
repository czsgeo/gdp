//2014.10.14，czs, create in hailutu, 处理器，输入对象本身改变对象内部结构，属于构造函数的延续。
//2014.12.11，czs, create in jinxinliangmao shaungliao, 具参数（材料）的对象构造器。
//2015.05.14, czs, edit in namu,  增加双参数构造器，所有修改为Builder，去掉Paramed

using System;
using Gdp.IO;
namespace Gdp
{  
    /// <summary>
    /// 完美（可算）对象构造器。
    /// </summary>
    /// <typeparam name="TProduct">待构建的类型</typeparam>
    public interface IBuilder<TProduct>
    {
        /// <summary>
        /// 返回OK对象。
        /// </summary>
        /// <returns></returns>
        TProduct Build();
    }

    /// <summary>
    /// 完美（可算）对象构造器。
    /// </summary>
    /// <typeparam name="TProduct">待构建的类型</typeparam>
    public abstract class AbstractBuilder <TProduct> : IBuilder<TProduct>
    {
        /// <summary>
        /// 回OK对象。
        /// </summary>
        /// <returns></returns>
        public abstract TProduct Build();
        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }


    /// <summary>
    /// 具参数（材料）的对象构造器。
    /// 不难发现，与IService结构相似，只是名称不同而已。
    /// </summary>
    /// <typeparam name="TProduct">待构建的类型</typeparam>
    /// <typeparam name="TMaterial">材料</typeparam>
    /// <typeparam name="TSecondMaterial">第二材料</typeparam>
    public interface IBuilder<TProduct, TMaterial, TSecondMaterial>
    {
        /// <summary>
        /// 返回OK对象。
        /// </summary>
        /// <returns></returns>
        TProduct Build( TMaterial material, TSecondMaterial secondMaterial);
    }

    /// <summary>
    /// 具参数（材料）的对象构造器。
    /// 不难发现，与IService结构相似，只是名称不同而已。
    /// </summary>
    /// <typeparam name="TProduct">待构建的类型</typeparam>
    /// <typeparam name="TMaterial">材料</typeparam>
    public interface IBuilder<TProduct, TMaterial>
    {
        /// <summary>
        /// 返回OK对象。
        /// </summary>
        /// <returns></returns>
        TProduct Build( TMaterial material);
    }
    /// <summary>
    /// 具参数（材料）的对象构造器。
    /// 不难发现，与IService结构相似，只是名称不同而已。
    /// </summary>
    /// <typeparam name="TProduct">待构建的类型</typeparam>
    /// <typeparam name="TMaterial">材料</typeparam>
    public abstract class AbstractBuilder<TProduct, TMaterial> : IBuilder<TProduct, TMaterial>
    {
        /// <summary>
        /// 日志写
        /// </summary>
        protected Gdp.IO.Log log = new Log(typeof(AbstractBuilder<TProduct, TMaterial>));
        /// <summary>
        /// 回OK对象。
        /// </summary>
        /// <returns></returns>

        public abstract TProduct Build(TMaterial material);
    }
    /// <summary>
    /// 具参数（材料）的对象构造器。
    /// 不难发现，与IService结构相似，只是名称不同而已。
    /// </summary>
    /// <typeparam name="TProduct">待构建的类型</typeparam>
    /// <typeparam name="TMaterial">材料</typeparam>
    /// <typeparam name="TSecondMaterial">第二材料</typeparam>
    public abstract class AbstractBuilder<TProduct, TMaterial, TSecondMaterial> : IBuilder<TProduct, TMaterial, TSecondMaterial>
    {
        /// <summary>
        /// 回OK对象。
        /// </summary>
        /// <returns></returns>

        public abstract TProduct Build(TMaterial material, TSecondMaterial secondMaterial);
    }

}
