using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace MorpehECS
{
    [Serializable]
    public struct SpiderMoveComponent : IComponent
    {
        public Transform transform;
        public float changeDirectionCooldown;
        public float moveSpeed;
        public Vector2 direction;
        public float rotateSpeed;
    }
}