using UnityEngine;
using UnityEngine.InputSystem;
using AsteroidsGame.Contracts;

namespace AsteroidsGame.Presentation
{
    public class UnityInputReader : MonoBehaviour, IInputReader
    {
        public PlayerControls playerControls;

        private Vector2 _moveValue;
        private bool _bulletPressed = false;
        private bool _laserPressed = false;
        private InputAction.CallbackContext _lastContext;

        private void Awake()
        {
            playerControls = new PlayerControls();
            playerControls.Enable();
        }

        private void OnEnable()
        {
            playerControls.Ship.Move.performed += OnMove;
            playerControls.Ship.Move.canceled += OnMove;

            playerControls.Ship.ShootBullet.performed += OnShootBullet;
            playerControls.Ship.ShootLaser.performed += OnShootLaser;
        }

        private void OnDisable()
        {
            playerControls.Disable();

            playerControls.Ship.Move.performed -= OnMove;
            playerControls.Ship.Move.canceled -= OnMove;

            playerControls.Ship.ShootBullet.performed -= OnShootBullet;
            playerControls.Ship.ShootLaser.performed -= OnShootLaser;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            _moveValue = context.ReadValue<Vector2>();
        }

        private void OnShootBullet(InputAction.CallbackContext context)
        {
            _bulletPressed = true;
        }

        private void OnShootLaser(InputAction.CallbackContext context)
        {
            _laserPressed = true;
        }

        public InputData ReadInput()
        {
            var data = new InputData
            {
                forward = Mathf.Max(0, _moveValue.y),
                turn = _moveValue.x,
                shootLaser = _laserPressed,
                shootBullet = _bulletPressed
            };

            _laserPressed = false;
            _bulletPressed = false;

            return data;
        }
    }

    public interface IInputReader
    {
        InputData ReadInput();
    }
}