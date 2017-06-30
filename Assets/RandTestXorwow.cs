using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Xorwow;

public class RandTestXorwow : RandTest<RandTestXorwow.RandData>
{
    #region type define
    public struct RandData
    {
        public XorwowService.XorwowState seed;
        public float current;
    }
    #endregion

    protected override RandData GetFirstData()
    {
        return new RandData() { seed = XorwowService.XorwowState.Generate() };
    }
}