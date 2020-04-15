//2018.07.10, czs, create in hmx, 多项式拟合生成器
//2018.07.13, czs, edit in HMX, 改进中
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using Gdp.IO;

namespace Gdp
{   
    /// <summary>
    /// 多项式拟合生成器管理器
    /// </summary>
    public class PolyfitBuilderManager<TKey> : BaseDictionary<TKey, PolyfitBuilder>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="order"></param>
        public PolyfitBuilderManager(int order)
        {
            this.Order = order;
        }
        /// <summary>
        /// 拟合阶次
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override PolyfitBuilder Create(TKey key)
        {
            return new PolyfitBuilder(Order);
        }
    }


    /// <summary>
    /// 多项式拟合生成器
    /// </summary>
    public class PolyfitBuilder : AbstractBuilder<LsPolyFit>
    {
        Log log = new Log(typeof(PolyfitBuilder));

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="order">拟合阶次</param>
        /// <param name="Margin">边界</param>
        /// <param name="Overlap">重叠区</param>
        public PolyfitBuilder(int order, double Margin=4, double Overlap= 2)
        {
            this.Order = order;
            this.TheVeryFirstKey = double.MaxValue;
            this.Margin = Margin;
            this.Overlap = Overlap;
        }
        #region 属性和设置属性
        /// <summary>
        /// 统计构建次数
        /// </summary>
        public static long BuildCount = 0;

        /// <summary>
        /// 边缘缓存区域
        /// </summary>
        public double Margin { get; set; }
        /// <summary>
        /// 重叠区
        /// </summary>
        public double Overlap { get; set; }
        /// <summary>
        /// 多项式次数
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public IDictionary<double, double> Data { get; set; }
        /// <summary>
        /// 初始化后的第一个键
        /// </summary>
        public double TheVeryFirstKey { get; set; }
        /// <summary>
        /// 当前的第一个键
        /// </summary>
        public double FirstKey { get; set; }
        /// <summary>
        /// 最后一个键
        /// </summary>
        public double LastKey { get; set; }
        /// <summary>
        /// 顺序是否为升序，否则为降序
        /// </summary>
        public bool IsKeyAscOrDesc { get => FirstKey < LastKey; }

        /// <summary>
        /// 上一个拟合器
        /// </summary>
        public LsPolyFit Previous { get; set; }
        /// <summary>
        /// 下一个拟合器
        /// </summary>
        public LsPolyFit Next { get; set; }
        /// <summary>
        /// 当前拟合器
        /// </summary>
        public LsPolyFit Currrent { get; private set; }

        /// <summary>
        /// 是否已经生成过一次了
        /// </summary>
        public bool IsFitted { get; set; }
        /// <summary>
        /// 是否具有上一个
        /// </summary>
        /// <returns></returns>
        public bool HasPrevious
        {
            get => Previous != null;
        }
        /// <summary>
        /// 是否具有下一个
        /// </summary>
        /// <returns></returns>
        public bool HasNext
        {
            get => Next != null;
        }
        /// <summary>
        /// X 是否从第一个注册的X开始拟合
        /// </summary>
        public bool IsFromTheVeryFirstKey { get; set; }
        /// <summary>
        ///  X 是否从第一个注册的X开始拟合
        /// </summary>
        /// <param name="isFromTheVeryFirstKey"></param>
        /// <returns></returns>
        internal PolyfitBuilder SetIsFromTheVeryFirstKey(bool isFromTheVeryFirstKey)
        {
            this.IsFromTheVeryFirstKey = isFromTheVeryFirstKey;
            return this;
        }
        /// <summary>
        /// 清空上一次
        /// </summary>
        /// <returns></returns>
        public PolyfitBuilder IsBreaked(bool tureOrNot)
        {
            if(tureOrNot)
                this.Previous = null;
            return this;
        }
        /// <summary>
        /// 设置拟合数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public PolyfitBuilder SetData(IDictionary<double, double> data)
        {
            this.Data = data;
            return this;
        }
        /// <summary>
        /// 设置当前
        /// </summary>
        /// <param name="Currrent"></param>
        public void SetCurrent(LsPolyFit Currrent)
        {
            this.Previous = this.Currrent;
            this.Currrent = Currrent;

            //log.Info( "Current Setted: " + ToString());
        }
        /// <summary>
        /// 重置首件
        /// </summary>
        /// <returns></returns>
        public PolyfitBuilder ResetTheVeryFirstKey()
        {
            this.TheVeryFirstKey = double.MaxValue;
            return this;
        }
        #endregion

        /// <summary>
        /// 是否可以拟合，在边缘 margin 内即可
        /// </summary>
        /// <param name="key"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        public bool IsFittable(double key, double margin = 0)
        {
            if (IsKeyAscOrDesc)
            {
                return key >= FirstKey + margin && key <= LastKey - margin;
            }
            //降序
            return key >= LastKey + margin && key <= FirstKey - margin;
        }

        /// <summary>
        /// 是否在重叠区域内，0表示不，非0表示重合，-1表示
        /// </summary>
        /// <param name="key"></param>
        /// <param name="overlap">重叠区东西</param>
        /// <param name="margin">边缘</param>
        /// <returns></returns>
        public PositionOfMovingWindow GetPosition(double key, double overlap = 0, double margin = 0)
        {
            //顺序 
            if (IsKeyAscOrDesc)
            {
                return GetPosition(key, FirstKey, LastKey, overlap, margin);
            }
            //逆序
            return GetPosition(key,  LastKey,FirstKey, overlap, margin);             
        }

        /// <summary>
        /// 获取当前拟合器的拟合值根据不同区域进行判断。
        /// </summary>
        /// <param name="keyOrX"></param>
        /// <param name="overlap"></param>
        /// <param name="margin"></param>
        /// <param name="isSubsected">是否采用叠加分段式拟合方法</param>
        /// <returns></returns>
        public RmsedNumeral GetPolyfitValue(double keyOrX, bool isSubsected =true,  double overlap = 0, double margin = 0)
        {
            if (this.Currrent == null)
            {
                throw new Exception("请生成拟合器后再来！");
            }
            if (!isSubsected)
            {
                return this.Currrent.GetRmsedY(keyOrX - this.GetBaseKey());
            }

            var type = GetPosition(keyOrX, overlap, margin);

            var info = keyOrX + "[" + FirstKey + ", " + (FirstKey + margin) + ", " + (FirstKey + margin + overlap)
                + "--" + (LastKey - margin - overlap) + ", " + (LastKey - margin) + ", " + LastKey + "]";

            var differX = keyOrX - this.GetBaseKey();
            switch (type)
            {
                case PositionOfMovingWindow.FormerOutside:
                case PositionOfMovingWindow.LatterOutside:
                    log.Warn("在拟合区外计算！" + type + " "+ info);
                    return this.Currrent.GetRmsedY(differX);
                case PositionOfMovingWindow.FormerMargin:
                    log.Debug("在拟合区前边界计算！" + info);
                    return this.Currrent.GetRmsedY(differX);
                case PositionOfMovingWindow.FormerOverlap:
                    {
                        var val = this.Currrent.GetRmsedY(differX);
                        if (HasPrevious)
                        {
                            var val2 = this.Previous.GetRmsedY(differX);
                            double dis1 = Math.Abs(this.FirstKey + margin + overlap - keyOrX);
                            double dis2 = Math.Abs(overlap - dis1);

                            val.Value = Gdp.Utils.DoubleUtil.WeightedAverage(val.Value, val2.Value, dis1, dis2);
                            val.StdDev = Gdp.Utils.DoubleUtil.WeightedAverage(val.StdDev, val2.StdDev, dis1, dis2);
                        }
                        return val;
                    }
                case PositionOfMovingWindow.Inside:
                    return this.Currrent.GetRmsedY(differX);
                case PositionOfMovingWindow.LatterOverlap:
                    {
                        var val = this.Currrent.GetRmsedY(differX);
                        if (HasNext)
                        {
                            var val2 = this.Next.GetRmsedY(differX);
                            double dis1 = Math.Abs(keyOrX - (this.LastKey - margin - overlap));
                            double dis2 = Math.Abs(overlap - dis1);

                            val.Value = Gdp.Utils.DoubleUtil.WeightedAverage(val.Value, val2.Value, dis1, dis2);
                            val.StdDev = Gdp.Utils.DoubleUtil.WeightedAverage(val.StdDev, val2.StdDev, dis1, dis2);
                        }
                        log.Debug("在拟合区后重叠区计算，表示即将计算完毕！" + info);
                        return val;
                    }
                case PositionOfMovingWindow.LatterMargin:
                  //  log.Warn("在拟合区后边界计算！" + info);
                    return this.Currrent.GetRmsedY(differX);
                default:
                    break;
            }

            return this.Currrent.GetRmsedY(differX);
        } 

        /// <summary>
        /// 获取对应X为0的Key。所有的都要减去它。
        /// </summary>
        /// <returns></returns>
        public double GetBaseKey()
        {
            return IsFromTheVeryFirstKey ? TheVeryFirstKey : FirstKey;
        } 

        /// <summary>
        /// 构建最小二乘拟合器。 
        /// </summary> 
        /// <returns></returns>
        public override LsPolyFit Build()
        {
            double[] ys = Data.Values.ToArray();
            var len = ys.Length;
            double[] xs = new double[len];

            var keys = this.Data.Keys.ToArray(); ;
            this.FirstKey = (keys[0]);

            if (this.TheVeryFirstKey == double.MaxValue)
            {
                this.TheVeryFirstKey = FirstKey;
            }

            var baseKey = GetBaseKey(); 

            this.LastKey = keys[len - 1];
            for (int i = 0; i < len; i++)
            {
                xs[i] = (keys[i]) - baseKey;
            }
            var lsp = new LsPolyFit(xs, ys, Order);
            lsp.FitParameters();

            this.SetCurrent(lsp);            

            IsFitted = true;

            BuildCount++;

            log.Debug("生成拟合器，编号：" + BuildCount + ", " +  this.ToString());

            return Currrent;
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "多项式拟合生成器：[" + this.FirstKey + "-" + this.LastKey + ", "+(this.LastKey - this.FirstKey) + "], BaseStart:"+GetBaseKey()+", Current：" + this.Currrent + ", Previous: " + Previous;
        }

        /// <summary>
        /// 计算位置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="small"></param>
        /// <param name="larger"></param>
        /// <param name="overlap"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        private static PositionOfMovingWindow GetPosition(double key, double small, double larger, double overlap, double margin)
        {
            double content = larger - small - 2 * (margin + overlap);

            if(key >= small + margin + overlap && key <= larger - margin - overlap) {
                return PositionOfMovingWindow.Inside;
            }

            if (key < small) { return PositionOfMovingWindow.FormerOutside; }
            if (key > larger) { return PositionOfMovingWindow.LatterOutside; }

            //优先考虑重叠区
            if (key >= small + margin && key <= small + margin + overlap) { return PositionOfMovingWindow.FormerOverlap; }
            if (key >= larger - margin - overlap && key <= larger - margin) { return PositionOfMovingWindow.LatterOverlap; }

            if (key >= small && key < small + margin) { return PositionOfMovingWindow.FormerMargin; }
            if (key >= larger - margin && key < larger) { return PositionOfMovingWindow.LatterMargin; }

            return PositionOfMovingWindow.Inside;
        }
    }
}
