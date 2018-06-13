using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace System
{
    public static class MathExternsion
    {
        #region const members

        private const float EPSILON = 1e-005f;  //最小常量

        #endregion

        #region public static functions

        /// <summary>
        /// is equal to zero
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        public static bool IsEqualZero(float p_data)
        {
            if (Mathf.Abs(p_data) <= EPSILON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// is equal to zero
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        public static bool IsEqualZero(Vector2 p_data)
        {
            if (IsEqualZero(p_data.x) && IsEqualZero(p_data.y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// is equal to zero
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        public static bool IsEqualZero(Vector3 p_data)
        {
            if (IsEqualZero(p_data.x) && IsEqualZero(p_data.y) && IsEqualZero(p_data.z))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
