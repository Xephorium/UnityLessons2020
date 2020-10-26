using System.Collections;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public class CameraZoom {
    #region Variables

    #region Private Serialized
    [Space, Header("Zoom Settings")]
    [Range(20f, 60f)][SerializeField] private float zoomFOV = 20f;
    [SerializeField] private AnimationCurve zoomCurve = new AnimationCurve();
    [SerializeField] private float zoomTransitionDuration = 0f;

    [Space, Header("Run Settings")]
    [Range(60f, 150f)][SerializeField] private float runFOV = 120f;
    [SerializeField] private AnimationCurve runCurve = new AnimationCurve();
    [SerializeField] private float runTransitionDuration = 0f;
    [SerializeField] private float runReturnTransitionDuration = 0f;
    #endregion

    #region Private Non Serialized
    private float m_initFOV;
    private LookInputState lookInputState;

    #region Flags
    private bool m_running;
    private bool m_zooming;
    #endregion

    #region Components
    private Camera m_cam;
    #endregion

    #region Reference/Cache
    private IEnumerator m_ChangeFOVRoutine;
    private IEnumerator m_ChangeRunFOVRoutine;
    #endregion
    #endregion
    #endregion

    #region Custom Methods
    public void Init(Camera _cam, LookInputState _data) {
        lookInputState = _data;

        m_cam = _cam;
        m_initFOV = m_cam.fieldOfView;
    }

    public void ChangeFOV(MonoBehaviour _mono) {
        if(m_running) {
            lookInputState.isZooming = !lookInputState.isZooming;
            m_zooming = lookInputState.isZooming;
            return;
        }

        if(m_ChangeRunFOVRoutine != null)
            _mono.StopCoroutine(m_ChangeRunFOVRoutine);

        if(m_ChangeFOVRoutine != null)
            _mono.StopCoroutine(m_ChangeFOVRoutine);

        m_ChangeFOVRoutine = ChangeFOVRoutine();
        _mono.StartCoroutine(m_ChangeFOVRoutine);
    }

    IEnumerator ChangeFOVRoutine() {
        float _percent = 0f;
        float _smoothPercent = 0f;

        float _speed = 1f / zoomTransitionDuration;

        float _currentFOV = m_cam.fieldOfView;
        float _targetFOV = lookInputState.isZooming ? m_initFOV : zoomFOV;

        lookInputState.isZooming = !lookInputState.isZooming;
        m_zooming = lookInputState.isZooming;

        while(_percent < 1f) {
            _percent += Time.deltaTime * _speed;
            _smoothPercent = zoomCurve.Evaluate(_percent);
            m_cam.fieldOfView = Mathf.Lerp(_currentFOV, _targetFOV, _smoothPercent);
            yield return null;
        }
    }

    public void ChangeRunFOV(bool _returning, MonoBehaviour _mono) {
        if(m_ChangeFOVRoutine != null)
            _mono.StopCoroutine(m_ChangeFOVRoutine);

        if(m_ChangeRunFOVRoutine != null)
            _mono.StopCoroutine(m_ChangeRunFOVRoutine);

        m_ChangeRunFOVRoutine = ChangeRunFOVRoutine(_returning);
        _mono.StartCoroutine(m_ChangeRunFOVRoutine);
    }

    IEnumerator ChangeRunFOVRoutine(bool _returning) {
        float _percent = 0f;
        float _smoothPercent = 0f;

        float _duration = _returning ? runReturnTransitionDuration : runTransitionDuration;
        float _speed = 1f / _duration;

        float _currentFOV = m_cam.fieldOfView;
        float _targetFOV = _returning ? m_initFOV : runFOV;

        m_running = !_returning;

        while(_percent < 1f) {
            _percent += Time.deltaTime * _speed;
            _smoothPercent = runCurve.Evaluate(_percent);
            m_cam.fieldOfView = Mathf.Lerp(_currentFOV, _targetFOV, _smoothPercent);
            yield return null;
        }
    }
    #endregion
}
