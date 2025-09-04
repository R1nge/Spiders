using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace MorpehECS
{
    
    [Serializable]
    public struct SpiderMoveComponent : IComponent
    {
        public float changeDirectionCooldown;
        public Vector2 direction;
        public float rotateSpeed;
        public Quaternion rotation;
    }
}