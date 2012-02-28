/*
 * 功能：向底层Internal里传递参数实现此接口
 * author:丁乐
 * time:2011/12/23
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Thinkment.Data
{
    /// <summary>
    /// 扩展参数接口
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public interface IParametExtension<T> : ICloneable where T : class
    {

    }
}
