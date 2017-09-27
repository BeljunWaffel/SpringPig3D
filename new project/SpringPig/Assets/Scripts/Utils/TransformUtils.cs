﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class TransformUtils
    {
        /// <summary>
        /// Takes object coordinates in the grid, and returns a vector for the object's local position.
        /// Used for determining where to place an object in the UI when passed in grid dimensions.
        /// </summary>
        /// <param name="coordinates">x,y,z grid coordinates</param>
        /// <param name="objectDimensions">width, height, length of object</param>
        /// <returns></returns>
        public static Vector3 GetLocalPositionFromGridCoordinates(Vector3 coordinates, Vector3 objectDimensions)
        {
            return new Vector3(objectDimensions.x / 2 + coordinates.x,
                                      coordinates.y + objectDimensions.y / 2,
                                      -.5f * objectDimensions.z - coordinates.z);
        }
    }
}