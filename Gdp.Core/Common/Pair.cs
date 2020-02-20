//2018.04.17, czs, create in hmx, 一对

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 数字数据对
    /// </summary>
    public class NumerialTriple : Triple<double>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        public NumerialTriple(double First, double Second, double third) : base(First, Second, third)
        {
        }
        static NumerialTriple zero = new NumerialTriple(0,0, 0);
        /// <summary>
        /// 0
        /// </summary>
        public static NumerialTriple Zero { get { return zero; } }
    }
    /// <summary>
    /// 数字数据对
    /// </summary>
    public class NumerialPair : Pair<double>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        public NumerialPair(double First, double Second) : base(First, Second)
        {
        }
        static NumerialPair zero = new NumerialPair(0, 0);
        /// <summary>
        /// 0
        /// </summary>
        public static NumerialPair Zero { get { return zero; } }
    }
    /// <summary>
    /// 数字数据对
    /// </summary>
    public class StringPair : Pair<string>
    {
        /// <summary>
        /// 构造函数
        /// </summary> 
        public StringPair() : base("", "")
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        public StringPair(string First, string Second) : base(First, Second)
        {
        }
        
        public static StringPair Empty = new StringPair("", "");
    }
    public class StringTriple : Triple<string>
    {
        /// <summary>
        /// 构造函数
        /// </summary> 
        public StringTriple() : base("", "","")
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        public StringTriple(string First, string Second, string third) : base(First, Second, third)
        {
        }

        public static StringTriple Empty = new StringTriple();
    }

    public class Triple<T> : Pair<T>
    {
        public Triple(T First, T Second, T Third) : base(First, Second)
        {
            this.Third = Third;
        }
        /// <summary>
        /// 第3个
        /// </summary>
        public T Third { get; set; }
        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(" \t");
        }
        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public  string ToString(string spliter)
        {
            return First + spliter + Second + spliter + Third;
        }
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Triple<T>))
            {
                return false;
            }
            var o = obj as Triple<T>;
            return First.Equals(o.First) && Second.Equals(o.Second) && Third.Equals(o.Third);
        }
        /// <summary>
        /// 哈希数
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() + Third.GetHashCode() * 5;
        }
    }




    /// <summary>
    /// 一对
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pair<T>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        public Pair(T First, T Second)
        {
            this.First = First;
            this.Second = Second;
        }
        /// <summary>
        /// 第一个
        /// </summary>
        public T First { get; set; }
        /// <summary>
        /// 第二个
        /// </summary>
        public T Second { get; set; }
        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(" \t");
        }
        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public   string ToString(string spliter )
        {
            return First + spliter + Second;
        }
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(!(obj is Pair<T>))
            {
                return false;
            }
            var o = obj as Pair<T>;
            return First.Equals(o.First) && Second.Equals(o.Second);
        }
        /// <summary>
        /// 哈希数
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return First.GetHashCode()+ 13 * Second .GetHashCode() ;
        }
    }
    /// <summary>
    /// 具有时间的NEU
    /// </summary>
    public class TimedNeu
    {
        public DateTime Time { get; set; }

        public double N { get; set; }
        public double E { get; set; }
        public double U { get; set; }
    }

    /// <summary>
    /// 时间对
    /// </summary>
    public class TimedPair : TimedValue<Pair<Double>>
    {
        public TimedPair() : base() { }
        public TimedPair(DateTime time, Pair<Double> val) : base(time, val) { }


        public override int GetHashCode()
        {
            return Time.GetHashCode() + 13 * Value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var o = obj as TimedPair;

            return Time.Equals(o.Time) && Value.Equals(o.Value);
        }
        public override string ToString()
        {
            string str = Time.ToString("yyyy-MM-dd HH:mm:ss");
            return str + "\t" + Value;
        }
    }

    /// <summary>
    /// 具有名称数组，接口。用于绘图等。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INamedArray<T> : IEnumerable<T>
    {
        string Name { get; set; }
        List<T> Array { get; set; }
        int Count { get; }
    }
    /// <summary>
    /// 具有名称数组,用于绘图等。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NamedArray<T> : INamedArray<T>
    {
        public NamedArray(string name) { this.Name = name; this.Array = new List<T>(); }
        public string Name { get; set; }
        public List<T> Array { get; set; }

        public void Add(T t) { this.Array.Add(t); }
        public int Count { get { return Array.Count; } }

        public IEnumerator<T> GetEnumerator()
        {
            return Array.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Array.GetEnumerator();
        }
    }

    public class NamedDoubles : NamedArray<Double>
    {
        public NamedDoubles(string name) : base(name) { }
    }
    public class NamedTimeDoubles : NamedArray<TimedDouble>
    {
        public NamedTimeDoubles(string name) : base(name) { }
    }




    /// <summary>
    /// 具有时间属性的值。
    /// </summary>
    /// <typeparam name="TVal"></typeparam>
    public class TimedValue<TVal> : IComparable<TimedValue<TVal>>
    {
        public TimedValue() { this.Time = DateTime.Now; this.Value = default(TVal); }

        public TimedValue(DateTime Time, TVal Value) { this.Time = Time; this.Value = Value; }

        public DateTime Time { get; set; }

        public TVal Value { get; set; }

        public int CompareTo(TimedValue<TVal> other)
        {
            return Time.CompareTo(other.Time);
        }
    }

    public class TimedDouble : TimedValue<Double>
    {
        public TimedDouble() : base() { }
        public TimedDouble(DateTime time, Double val) : base(time, val) { }

        public static TimedDouble operator -(TimedDouble left, TimedDouble right)
        {
            return new TimedDouble(left.Time, left.Value - right.Value);
        }
        public override int GetHashCode()
        {
            return Time.GetHashCode() + 13 * Value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var o = obj as TimedDouble;

            return Time.Equals(o.Time) && Value.Equals(o.Value);
        }
        public override string ToString()
        {
            string str = Time.ToString("yyyy-MM-dd HH:mm:ss");
            return str + "\t" + Value;
        }
    }

    public class TimedXYZ : TimedValue<XYZ>
    {
        public TimedXYZ() : base() { }
        public TimedXYZ(DateTime time, XYZ val) : base(time, val) { }
    }
}
