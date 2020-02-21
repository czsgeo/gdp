using System;
using System.Collections.Generic;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// 载波频率的类型。A,B,C...
    /// 为了方便处理多GNSS系统数据，统一采用A B C 表示系统频率，代表各系统的 L1，L2，L3/L5 的载波频率。
    /// 频率类型与 GNSS 系统无关。
    /// 而不采用 GPSL1，GPSL2 这种方式。
    /// 实际上，可以采用 1， 2，3 这种编码方式，但是具有不可控的缺点,并且C#不支持数字变量。
    /// </summary>
    public enum FrequenceType
    {
        Unknown = 0,
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        E = 5,
        F = 6,
        G = 7,
        H = 8,
        I = 9,
        J = 10, 
        K = 11,
        L = 12,
        M = 13,
        N = 14,
        O = 15,
        P = 16,
        Q = 17,
        R = 18,
        S = 19,
        T = 20,
        U = 21,
        V = 22,
        W = 23,
        X = 24,
        Y = 25,
        Z = 26
    }
}