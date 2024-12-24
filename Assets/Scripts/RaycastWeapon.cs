using Demo.Scripts.Runtime.Item;
using UnityEngine;

namespace FPS
{
    public class RaycastWeapon : Weapon
    {
        [Header("Ray")]
        [SerializeField] private LayerMask _layerMask;
        [SerializeField, Min(1)] private int _shootCount = 1;
        [SerializeField, Min(0)] private float _distance = 100f;

        [Header("Spread")]
        [SerializeField] private bool _useSpread;
        [SerializeField, Min(0)] private float _spreadFactor;

        [SerializeField] private float _gizmosSphereRadius = 0.02f;

        private Vector3 _lastHitPosition;
        private Transform _startPoint;

        protected override void Shoot()
        {
            base.Shoot();

            for (int i = 0; i < _shootCount; ++i)
            {
                PerformRaycast();
            }
        }

        private Vector3 CalculateSpread()
        {
            return new Vector3
            {
                x = GetRandomSpreadFactor(),
                y = GetRandomSpreadFactor(),
                z = GetRandomSpreadFactor()
            };


            float GetRandomSpreadFactor()
            {
                return Random.Range(-_spreadFactor, _spreadFactor);
            }
        }

        private void PerformRaycast()
        {
            var direction = _useSpread
                ? _startPoint.forward + CalculateSpread()
                : _startPoint.forward;
            var ray = new Ray(_startPoint.position, direction);

            direction *= 0.1f;

            if (Physics.Raycast(ray, out var hitInfo, _distance, _layerMask, QueryTriggerInteraction.Ignore))
            {
                _lastHitPosition = hitInfo.point;
            }
        }

        private void Start()
        {
            _startPoint = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_lastHitPosition == Vector3.zero)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_lastHitPosition, _gizmosSphereRadius);
        }
#endif
    }
}