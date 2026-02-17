#if UNITY_EDITOR || DEVELOPMENT_BUILD

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DebugTools : MonoBehaviour
{
    private PlayerControls _playerControls;

    [SerializeField] private float _speedUpFactor = 5f;
    [SerializeField] private int _defaultFps = -1;
    [SerializeField] private int _loweredFps = 30;

    private bool _isLowerFps;

    private void Awake()
    {
        _playerControls = new PlayerControls();

        _playerControls.Debug.ReloadScene.performed += OnReload;
        _playerControls.Debug.SpeedUp.started += OnSpeedUpStarted;
        _playerControls.Debug.SpeedUp.canceled += OnSpeedUpCanceled;
        _playerControls.Debug.LowerFPS.performed += OnLowerFps;

        _playerControls.Enable();
    }

    private void OnDestroy()
    {
        if (_playerControls == null)
            return;

        _playerControls.Debug.ReloadScene.performed -= OnReload;
        _playerControls.Debug.SpeedUp.started -= OnSpeedUpStarted;
        _playerControls.Debug.SpeedUp.canceled -= OnSpeedUpCanceled;
        _playerControls.Debug.LowerFPS.performed -= OnLowerFps;

        _playerControls.Disable();
        _playerControls.Dispose();
    }

    private bool isSceneReloaded = false;

    private void OnReload(InputAction.CallbackContext ctx)
    {
        if (isSceneReloaded)
            return;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        isSceneReloaded = true;
    }

    private void OnSpeedUpStarted(InputAction.CallbackContext ctx)
    {
        Time.timeScale = _speedUpFactor;
    }

    private void OnSpeedUpCanceled(InputAction.CallbackContext ctx)
    {
        Time.timeScale = 1f;
    }

    private void OnLowerFps(InputAction.CallbackContext ctx)
    {
        _isLowerFps = !_isLowerFps;
        Application.targetFrameRate = _isLowerFps ? _loweredFps : _defaultFps;
    }
}
#endif