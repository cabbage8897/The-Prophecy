using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheProphecy.Player
{
    public class MovementController : MonoBehaviour
    {
        
        [Header("References")]
        [SerializeField] private Joystick _joystick;
        [SerializeField] private GameObject _gunHoldingPoint;
        [SerializeField] private GameObject _canvas;
        private TrailRenderer _trailRenderer;
        private SpriteRenderer _spriteRenderer;

        [Header("Basic Movement")]
        public float speed;
        private Vector2 direction = new Vector2(1, 0);
        private Vector3 _movement;


        [Header("Dashing")]
        [SerializeField] private float _dashingVelocity;
        [SerializeField] private float _dashingTime;
        [SerializeField] private float _dashingCooldown;
        private bool _isDashOnCooldown;
        private float _lastDashTime = 0f;
        private bool _isDashing = false;
        private float _dashCooldownPercentage = 1f;

        public Vector2 Direction { get => direction; }
        public float DashCooldownPercentage { get => _dashCooldownPercentage;}

        private void Start()
        {
            _trailRenderer = GetComponent<TrailRenderer>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

        }

        private void Update()
        {
            CalculateDashCooldowndPercentage();
        }

        private void FixedUpdate()
        {
            if (!_isDashing)
            {
                Move();
                RotateCharacterWhenMove();
            }

            Dash();
        }

        private void Move()
        {
            _movement.x = _joystick.Horizontal;
            _movement.y = _joystick.Vertical;

            transform.Translate(_movement * speed * Time.deltaTime);
        }

        private void RotateCharacterWhenMove()
        {
            if (_movement.x != 0 || _movement.y != 0)
            {
                direction = _joystick.Direction;

                float directionAngle = Vector2.SignedAngle(new Vector2(1, 0), direction);
                _gunHoldingPoint.transform.rotation = Quaternion.Euler(0, 0, directionAngle);

                if (_movement.x < 0)
                {
                    _spriteRenderer.flipX = true;
                }

                else if(_movement.x > 0)
                {
                    _spriteRenderer.flipX = false;
                }
            }
            
        }

        public void OnDashButtonPressed()
        {
            if (_isDashOnCooldown || _isDashing)
            {
                return;
            }

            _lastDashTime = Time.time;
            _isDashing = true;
            _trailRenderer.emitting = true;
        }

        private void Dash()
        {
            _isDashOnCooldown = _lastDashTime == 0f ? false : Time.time - _lastDashTime < _dashingCooldown;

            if (!_isDashing)
            {
                return;
            }

            bool isDashFinished = Time.time - _lastDashTime > _dashingTime;

            if (isDashFinished)
            {
                _isDashing = false;
                _trailRenderer.emitting = false;
            }

            else
            {
                transform.position += new Vector3(direction.x, direction.y, 0) * _dashingVelocity * Time.deltaTime;
            }
        }

        public void CalculateDashCooldowndPercentage()
        {
            _dashCooldownPercentage = (Time.time - _lastDashTime) / _dashingCooldown;
        }
    }
}
