using System;
using System.Collections;
using UnityEngine;

namespace Animations
{
    public class MoveTo : MonoBehaviour
    {
        public AnimationCurve curve;
        public float duration;
        public Transform[] points;
        public Vector3 axis = Vector3.up;
    }
}