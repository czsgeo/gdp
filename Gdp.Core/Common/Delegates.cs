//2016.10.19, czs, create in hongqing, 将所有的委托集中于此

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{

    /// <summary>
    /// 实体产生了。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    public delegate void EntityProducedEventHandler<T>(T entity);
    /// <summary>
    /// 类型改变事件处理委托
    /// </summary>
    /// <param name="type"></param>
    public delegate void TypeChangedEventHandler(Type type);

    //2016.08.20, czs,create in fujian yongan, BooleanChangedEventHandler and 信息生产委托
    /// <summary>
    /// 信息生产委托
    /// </summary>
    /// <param name="info"></param>
    public delegate void InfoProducedEventHandler(string info);
    /// <summary>
    /// bool 改变
    /// </summary>
    /// <param name="trueOrFalse"></param>
    public delegate void BooleanChangedEventHandler(bool trueOrFalse);
    /// <summary>
    /// 浮点数事件
    /// </summary>
    /// <param name="val"></param>
    public delegate void NumberEventHandler(double val);
    /// <summary>
    /// 整数事件
    /// </summary>
    /// <param name="val"></param>
    public delegate void IntEventHandler(int val);
    
    /// <summary>
    /// 一个顶层的委托。指示一个整型数据变化了。
    /// </summary>
    /// <param name="newValue"></param>
    public delegate void IntValueChangedEventHandler(int newValue);
    /// <summary>
    /// 处理命令改变啦！
    /// </summary>
    /// <param name="type"></param>
    public delegate void ProcessCommandChangedEventHandler(ProcessCommandType type);
}
