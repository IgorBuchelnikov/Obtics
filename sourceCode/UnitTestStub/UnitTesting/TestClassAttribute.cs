﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TvdP.UnitTesting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TestClassAttribute : Attribute
    {
    }
}
