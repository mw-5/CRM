﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Model
{
    public enum ColTypes
    {
        Text,
        Numeric,
        Date,
        Boolean
    }
    /// <summary>
    /// use of struct instead of class to make properties immutable.
    /// </summary>
    public struct ColDef
    {
        public String Name;
        public ColTypes Type;
    }
}
