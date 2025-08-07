using System.Collections;
using UnityEngine;

public abstract class PopupBase : MonoBehaviour    
{
    protected bool          _opened;            // �˾��� ���ȴ��� ����
    protected Coroutine     _routine;           // �˾� ����/���� �ڷ�ƾ
    protected bool          _popupAnimating;    // �˾� ����/���� �ִϸ��̼� ������ ���� (PopupBase������ ����)
    protected CanvasGroup   _panelGroup;        // �˾� �г� CanvasGroup (���ͷ��� �����)
    protected RectTransform _panel;             // �˾� �г� RectTransform (* ��� �˾��� �ݵ�� ���� *)
    protected bool _useDim = true;              // Dim ��� ����

    /// <summary>
    /// �˾� �ʱ�ȭ (�ʼ�)
    /// </summary>
    public virtual void Init() 
    {
        Transform root = transform;
        _panel = "panel".FindIn<RectTransform>(root);

        _panelGroup = _panel.GetComponent<CanvasGroup>();
        _panelGroup.interactable = false;
    }

    /// <summary>
    /// �˾� ����
    /// </summary>
    public virtual void Open() 
    {
        // �˾��� �����ְų� �ִϸ��̼� ���̸� ����
        if (_opened || _popupAnimating) return;

        _opened = true;
        _popupAnimating = true;
        gameObject.SetActive(true);

        if (_routine != null)
            StopCoroutine(_routine);
        _routine = StartCoroutine(OpenRoutine());
    }

    /// <summary>
    /// �˾� �ݱ�
    /// </summary>
    public virtual void Close() 
    {
        // �˾��� �����ְų� �ִϸ��̼� ���̸� ����
        if (!_opened || _popupAnimating) return;

        _popupAnimating = true;

        if (_routine != null)
            StopCoroutine(_routine);
        _routine = StartCoroutine(CloseRoutine());
    }

    /// <summary>
    /// �˾��� ���� �� ȣ��
    /// </summary>
    protected virtual void OnOpened() 
    {
        _popupAnimating = false;
    }

    /// <summary>
    /// �˾��� ���� �� ȣ��
    /// </summary>
    protected virtual void OnClosed() 
    {
        _popupAnimating = false;
    }

    /// <summary>
    /// �˾��� ���� �� ����Ǵ� �ִϸ��̼� �ڷ�ƾ
    /// </summary>
    protected abstract IEnumerator AnimateOpen();

    /// <summary>
    /// �˾��� ���� �� ����Ǵ� �ִϸ��̼� �ڷ�ƾ
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
    /// ESC Ű�� �˾� �ݱ�
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
    /// �˾��� ���ͷ����� ����
    /// </summary>
    protected virtual void SetInteract(bool on)
    {
        if (_panelGroup != null)
        {
            _panelGroup.interactable = on;
        }

        // dim �� �ٸ� ����� ���ͷ��� ��� �ʿ��� �� �ڽĿ��� ����
    }

    #endregion
}
