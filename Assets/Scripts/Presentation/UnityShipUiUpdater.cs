using System;

namespace AsteroidsGame.Presentation
{
    using System.Globalization;
    using AsteroidsGame.Contracts;
    using UnityEngine;
    using TMPro;

    public sealed class UnityShipUiUpdater : MonoBehaviour, IShipUiUpdater
    {
        [SerializeField] private TextMeshProUGUI _healthLabel;
        [SerializeField] private TextMeshProUGUI _coordinatesLabel;
        [SerializeField] private TextMeshProUGUI _angleLabel;
        [SerializeField] private TextMeshProUGUI _speedLabel;
        [SerializeField] private TextMeshProUGUI _laserChargesLabel;
        [SerializeField] private TextMeshProUGUI _laserCooldownLabel;

        private int _lastHealth;

        private float _lastX;
        private float _lastY;

        private float _lastAngle;
        private float _lastSpeed;

        private int _lastLaserCharges;
        private float _lastLaserCooldown;

        [SerializeField] private float _epsilon = 0.01f;

        public void UpdateUI(in ShipUiData data)
        {
            if (data.health != _lastHealth)
            {
                _lastHealth = data.health;
                _healthLabel.SetText("Health: {0}", data.health);
            }

            if (Math.Abs(data.x - _lastX) > _epsilon ||
                Math.Abs(data.y - _lastY) > _epsilon)
            {
                _lastX = data.x;
                _lastY = data.y;

                _coordinatesLabel.SetText(
                    "Coordinates: ({0:0.00}, {1:0.00})",
                    data.x,
                    data.y);
            }

            if (Math.Abs(data.angle - _lastAngle) > _epsilon)
            {
                _lastAngle = data.angle;
                _angleLabel.SetText("Angle: {0:0.0}", data.angle);
            }

            if (Math.Abs(data.speed - _lastSpeed) > _epsilon)
            {
                _lastSpeed = data.speed;
                _speedLabel.SetText("Speed: {0:0.00}", data.speed);
            }

            if (data.laserCharges != _lastLaserCharges)
            {
                _lastLaserCharges = data.laserCharges;
                _laserChargesLabel.SetText("Laser charges: {0}", data.laserCharges);
            }

            if (Math.Abs(data.laserCooldown - _lastLaserCooldown) > _epsilon)
            {
                _lastLaserCooldown = data.laserCooldown;
                _laserCooldownLabel.SetText("Laser cooldown: {0:0.00}", data.laserCooldown);
            }
        }
    }
}