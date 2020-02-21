//2014.10.18, czs, create in beijing, 服务的内容为产品AbstractService<TProduct>
//2014.10.18，czs, create in beijing, 任何结果皆为产品AbstractParamedService<TProduct, TMaterial> : 
//2014.10.23, czs, edit in flight 成都->沈阳 CA4185， 重构，增加批量生产，增加原材料，产品不再标识，任何类皆是产品，皆是原材料。
//2016.04.24, czs, edit in hongqing, 对 AbstractProcessService进行了扩展。
//2018.06.08, czs, edti in hmx, 增加 init, Complete 的事件和方法
//2018.08.04, czs, edit in hmx, 存储上一结果和原料

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{ 
    /// <summary>
    /// 提供产品的供应商。规定了生产流程。
    /// 包含原料检核，生产和产品检核。
    /// </summary> 
    public abstract class AbstractProcessService<TProduct, TMaterial> : 
        AbstractService<TProduct, TMaterial>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AbstractProcessService(): base() {
            CurrentIndex = -1;
            IsPostCheckEnabled = true;
        }

        #region 基础属性和事件
        /// <summary>
        /// 当前编号。计算次数。
        /// </summary>
        public int CurrentIndex { get; set; } 
        /// <summary>
        /// 成功计算完成一次，激发一次。
        /// </summary>
        public virtual event ProducedEventHandler<TProduct, TMaterial> Produced;
        /// <summary>
        /// 计算前触发，可以在此做一些初始化工作。
        /// </summary>
        public virtual event ProducedEventHandler<TMaterial> Producing;
        /// <summary>
        /// 生产事件响应。
        /// </summary>
        /// <param name="PositionResult"></param>
        /// <param name="EpochInformation"></param>
        protected void OnProduced(TProduct PositionResult, TMaterial EpochInformation)
        {
            if (Produced != null) Produced(PositionResult, EpochInformation);
        }
        /// <summary>
        /// 原料缓存
        /// </summary>
        public IWindowData<TMaterial> MaterialBuffers { get; set; }
        /// <summary>
        /// 算后检验
        /// </summary>
        public bool IsPostCheckEnabled { get; set; }
        /// <summary>
        /// 上一非 NULL 产品
        /// </summary>
        public TProduct PrevProduct { get; private set; }

        /// <summary>
        /// 上一非 NULL 原料
        /// </summary>
        public TMaterial PrevMaterial { get; private set; }
        /// <summary>
        /// 当前非 NULL 产品
        /// </summary>
        public TProduct CurrentProduct { get; private set; }
        /// <summary>
        /// 当前非 NULL 原料
        /// </summary>
        public TMaterial CurrentMaterial { get; private set; }
        /// <summary>
        /// 设置当前原料，若不重复则更新
        /// </summary>
        /// <param name="material"></param>
        public AbstractProcessService<TProduct, TMaterial> SetMaterial(TMaterial material)
        {
            if (material == null) { return this; }

            if (null == CurrentMaterial || !CurrentMaterial.Equals(material))
            {
                this.PrevMaterial = this.CurrentMaterial;
                this.CurrentMaterial = material;
            }
            return this;
        }
        /// <summary>
        /// 设置当前产品，若不重复则更新
        /// </summary>
        /// <param name="product"></param>
        public AbstractProcessService<TProduct, TMaterial> SetProduct(TProduct product)
        {
            if (product == null) { return this; }

            if (null == CurrentProduct || !CurrentProduct.Equals(product))
            {
                this.PrevProduct = this.CurrentProduct;
                this.CurrentProduct = product;
            }
            return this;
        }

        #endregion

        #region 每次生产都要执行。
        /// <summary>
        /// 获取最终的产品，如果失败则返回默认对象，通常为null。每次生产都要执行。
        /// </summary>
        /// <returns></returns>
        public override TProduct Get(TMaterial material)
        {
            CurrentIndex++;

            if (material == null)
                return default(TProduct);

            this.SetMaterial(material);

            OnProducing(material);

            TProduct product = Produce(material);

            if (product == null) return product;

            if (IsPostCheckEnabled && !CheckProduct(product))
            {
                return default(TProduct);
            }
            this.SetProduct(product);
            OnProduced(this.CurrentProduct, this.CurrentMaterial);
            return product;
        }
        /// <summary>
        /// 生产前触发
        /// </summary>
        /// <param name="material"></param>
        protected virtual void OnProducing(TMaterial material)
        {
            if (Producing != null) Producing(material);
        }
        /// <summary>
        /// 算后的产品检核，通过或不通过。每次生产都要执行。
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckProduct(TProduct t);

        /// <summary>
        /// 具体的生产过程。每次生产都要执行。
        /// </summary>
        /// <returns></returns>
        public abstract TProduct Produce(TMaterial material);
        #endregion
    }

    /// <summary>
    /// 提供产品的供应商。规定了生产流程。
    /// </summary> 
    public abstract class AbstractService<TProduct, TCondition> : AbstractService<TProduct>, IService<TProduct, TCondition>
    {
        /// <summary>
        /// 默认构造函数。
        /// </summary>
        public AbstractService() { Init(); }

        /// <summary>
        /// 获取最终的产品，如果失败则返回默认对象，通常为null。每次生产都要执行。
        /// </summary>
        /// <returns></returns>
        public abstract TProduct Get(TCondition material);


    }

    /// <summary>
    /// 服务，一切皆服务。
    /// </summary>
    public abstract class AbstractService<TProduct> : IService<TProduct>
    {
        /// <summary>
        /// 刚刚初始化时激活
        /// </summary>
        public event Action Initting;
        /// <summary>
        /// 完成初始化后激活
        /// </summary>
        public event Action Inited;
        /// <summary>
        /// 调用完成后激活
        /// </summary>
        public event Action Completed;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 构造函数之后调用。用于完成一些初始工作。
        /// </summary>
        public virtual void Init() { OnInitting(); OnInited();   }
        /// <summary>
        /// 刚刚初始化时激活
        /// </summary>
        protected virtual void OnInitting() { Initting?.Invoke(); }
        /// <summary>
        /// 完成初始化后激活
        /// </summary>
        protected virtual void OnInited() { Inited?.Invoke(); }
        /// <summary>
        /// 结束时调用，需要手动调用。
        /// </summary>
        protected virtual void OnCompleted() { Completed?.Invoke(); }
        /// <summary>
        ///结束时调用，需要手动调用。
        /// </summary>
        public virtual void Complete() {
            OnCompleted();
        }
    }
}

