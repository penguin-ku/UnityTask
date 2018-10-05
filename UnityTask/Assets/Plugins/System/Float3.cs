﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace System
{
    public class Float3
    {
        public float x { set; get; }

        public float y { set; get; }

        public float z { set; get; }


        public static implicit operator Float3(Vector3 p_value)
        {
            return new Float3()
            {
                x = p_value.x,
                y = p_value.y,
                z = p_value.z
            };
        }

        public static implicit operator Vector3(Float3 p_value)
        {
            return new Vector3(p_value.x, p_value.y, p_value.z);
        }
    }
}
