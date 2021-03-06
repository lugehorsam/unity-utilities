﻿namespace Utilities
{
    using UnityEngine;

    public static class AxisExt
    {
        public static Vector3 ToVector3(this Axis thisAxis)
        {
            switch (thisAxis)
            {
                case Axis.X:
                    return Vector3.right;
                case Axis.Y:
                    return Vector3.up;
                case Axis.Z:
                    return Vector3.forward;
            }

            return Vector3.zero;
        }
    }
}
