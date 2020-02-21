// 2012.05, czs,   created
// 2013.03.28, czs, revision.
// 2013.04.11, czs, edit, 核心的应该是法方程 N X = U, N:Normal, U:RightHandSide
// 2013.05.07, czs, edit, 取消无 A 的构造函数，因其无法进行精度评定。增加常量构造函数。简化程序。
// 2013.06.01, czs, edit, 为利于区别，修改了权逆阵名称；修改N为参数权逆阵InverseWeightOfObs，增加参数 权阵 属性。
// 2014.09.04, czs, edit, 基本稳定了吧
//2017.07.20, czs, edit in hongqing, 采用通用Matrix实现。
//2018.03.24, czs, edit in hmx, 按照新架构重构, ParamAdjustment 更名为 ParamAdjuster

using System;
using System.Collections.Generic;
using System.Text;

using Gdp.Utils;

//平差：在有多余观测的基础上，根据一组含有误差的观测值，
//依一定的数学模型，按某种平差准则，求出未知量的最优估值，并进行精度评定。
namespace Gdp
{
    //最小二乘法方程之参数平差,或间接平差。
    //参考文献：
    //[1] 隋立芬，宋力杰.误差理论与测量平差基础[D].解放军出版社，2005:72-83
    //[2] 金日首，戴华阳.误差理论与测量平差基础[D].测绘出版社，2011:75-80
    //
    //参数平差：选函数独立的必需观测量为参数，组成误差方程。

    //    函数模型： L = A X + delta
    //    随机模型： E(delta) = 0; 
    // delta 为观测噪声
    //    误差方程： v  = A x - l , 其中 l = L - A X0, 即 l = 观测值 - 先验值的函数值。

    //      法方程:        A' P A x - A' P l = 0
    //法方程系数阵： N  =  A' P A
    //      改正数:  x  =  inv(A' P A) A' P l  = inv(N) A' P l
    //    参数估值： X  =  X0 + x； 即 X平差估值 = X初始值 + x ；
    //单位权中误差： s0 =  (v' P v) / (n_obs - n_unk)
    //参数协方差阵： Q  =  s0 inv(N)

    //参数平差步骤：
    //1. 根据平差问题选取t个独立量作为参数；
    //2. 将每一个观测值的平差值分别表达成参数的函数，对于非线性函数线性化，列出误差方程V = coeffOfParams({X_0} + \delta \hat X) + Consts - ObsValues；
    //3. 组成法方程 {coeffOfParams^T}PA\hat X + {coeffOfParams^T}Pl = 0 (这一步由本程序做，法方程系数阵N)；
    //4. 解算法方程，求出参数 Param=(coeffOfParams^T PA)^(-1) coeffOfParams^T Weight ObsMinusApriori  ；
    //5. 计算参数的平差值，求出观测量的平差值 \hat{X}={{X}_{0}}+\delta \hat{X}；
    //6、精度估计 InverseWeight.

    //1.由A、P、L计算ATPA、ATPL
    //2.求ATPA的逆矩阵
    //3.计算参数平差值
    //4.由3求残差V
    //5.计算单单位权中误差u。 


    ///约定：以 Vector 结尾的为一维数组。如参数，改正数等。
    /// <summary>
    /// 最小二乘法方程之参数平差,或间接平差。
    ///    函数模型： L = A X + delta
    ///    随机模型： E(delta) = 0; 
    ///    误差方程： v  = A x - l , 其中 l = L - A X0, 即 l = 观测值 - 先验值的函数值。
    /// </summary>
    public class ParamAdjuster //: MatrixAdjuster
    {
        #region 构造函数         
        /// <summary>
        /// 构造函数
        /// </summary>
        public ParamAdjuster() { }
        #endregion

        #region 核心计算方法
        /// <summary>
        /// 数据计算
        /// </summary>
        /// <param name="input">观测矩阵</param>
        /// <returns></returns>
        public  Vector Run(Matrix A, Matrix L, Matrix PL=null, Matrix X0=null, Matrix D =null)
        {

            //观测值权阵设置,对已知量赋值  
            Matrix QL = PL.Inverse;
            Matrix AT = A.Transpose();   
            int obsCount = A.RowCount ;
            int paramCount = A.ColCount;
            int freedom = obsCount - paramCount;
            //观测值更新
            Matrix l=L-(A*X0 + D); //如果null，则是本身
            
            //法方程
            Matrix N =  AT * PL * A;
            Matrix U = AT * PL * l;

            Matrix InverN = N.Inverse;

            //平差结果
            Matrix x = InverN * U;
            Matrix Qx = InverN ;
            //精度评定
            Matrix V = A * x - l;
            Matrix Qv = QL - A * Qx * AT;
            Matrix X = X0 + x; 

            double vtpv = (V.Transpose()* PL * V)[0,0];
            double s0 = vtpv / (freedom == 0 ? 0.1 : freedom);//单位权方差  
            StdDev = s0;

            var result = x.GetCol(0);
            return result;
        }

        public double StdDev { get; set; }

        #endregion

    }
}
