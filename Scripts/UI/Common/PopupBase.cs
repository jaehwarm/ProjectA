using System.Collections;
using UnityEngine;

public abstract class PopupBase : MonoBehaviour    
{
    protected bool          _opened;            // 팝업이 열렸는지 여부
    protected Coroutine     _routine;           // 팝업 열림/닫힘 코루틴
    protected bool          _popupAnimating;    // 팝업 열림/닫힘 애니메이션 중인지 여부 (PopupBase에서만 제어)
    protected CanvasGroup   _panelGroup;        // 팝업 패널 CanvasGroup (인터랙션 제어용)
    protected RectTransform _panel;             // 팝업 패널 RectTransform (* 모든 팝업에 반드시 존재 *)
    protected bool _useDim = true;              // Dim 사용 여부

    /// <summary>
    /// 팝업 초기화 (필수)
    /// </summary>
    public virtual void Init() 
    {
        Transform root = transform;
        _panel = "panel".FindIn<RectTransform>(root);

        _panelGroup = _panel.GetComponent<CanvasGroup>();
        _panelGroup.interactable = false;
    }

    /// <summary>
    /// 팝업 열기
    /// </summary>
    public virtual void Open() 
    {
        // 팝업이 열려있거나 애니메이션 중이면 무시
        if (_opened || _popupAnimating) return;

        _opened = true;
        _popupAnimating = true;
        gameObject.SetActive(true);

        if (_routine != null)
            StopCoroutine(_routine);
        _routine = StartCoroutine(OpenRoutine());
    }

    /// <summary>
    /// 팝업 닫기
    /// </summary>
    public virtual void Close() 
    {
        // 팝업이 닫혀있거나 애니메이션 중이면 무시
        if (!_opened || _popupAnimating) return;

        _popupAnimating = true;

        if (_routine != null)
            StopCoroutine(_routine);
        _routine = StartCoroutine(CloseRoutine());
    }

    /// <summary>
    /// 팝업이 열린 후 호출
    /// </summary>
    protected virtual void OnOpened() 
    {
        _popupAnimating = false;
    }

    /// <summary>
    /// 팝업이 닫힌 후 호출
    /// </summary>
    protected virtual void OnClosed() 
    {
        _popupAnimating = false;
    }

    /// <summary>
    /// 팝업이 열릴 때 실행되는 애니메이션 코루틴
    /// </summary>
    protected abstract IEnumerator AnimateOpen();

    /// <summary>
    /// 팝업이 닫힐 때 실행되는 애니메이션 코루틴
    /// </summary>
    protected abstract IEnumerator AnimateClose();


    #region Open,Close Routines  
    
    private IEnumerator OpenRoutine()
    {
        yield return AnimateOpen();
        OnOpened();
        _routine = null;          
    }

    private IEnumerator CloseRoutine()
    {
        yield return AnimateClose();
        _opened = false;
        OnClosed();
        _routine = null;
        Destroy(gameObject);
    }

    #endregion

    #region ESC Close

    /// <summary>
    /// ESC 키로 팝업 닫기
    /// </summary>

    public bool _escClose = true;
    protected virtual void Update()
    {
        if (_escClose && Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    #endregion

    #region Interact

    /// <summary>
    /// 팝업의 인터랙션을 설정
    /// </summary>
    protected virtual void SetInteract(bool on)
    {
        if (_panelGroup != null)
        {
            _panelGroup.interactable = on;
        }

        // dim 등 다른 요소의 인터랙션 제어가 필요할 시 자식에서 설정
    }

    #endregion
}
