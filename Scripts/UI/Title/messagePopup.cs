using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UtilUI;

public class MessagePopup : PopupBase
{
    public enum MessagePopupType
    {
        YesNo,
        Ok
    }

    public CanvasGroup      dim;
    public RectTransform    panel;
    public Button           btn_yes;
    public Button           btn_no;
    public Button           btn_ok;

    private float           _fadeDur = .1f;     // Fade �ִϸ��̼� �ð�
    private Action          _onYes;             // Yes ��ư Ŭ�� �� ������ �׼�
    private Action          _onNo;              // No ��ư Ŭ�� �� ������ �׼�
    private Action          _onOk;              // Ok ��ư Ŭ�� �� ������ �׼�

    public override void Init()
    {
        base.Init();

        Transform root = transform;
        panel = _panel;

        dim     = "dim".FindIn<CanvasGroup>(root);
        btn_yes = "btn_yes".FindIn<Button>(panel);
        btn_no  = "btn_no".FindIn<Button>(panel);
        btn_ok  = "btn_ok".FindIn<Button>(panel);

        SetClick(btn_yes, () => { _onYes?.Invoke(); Close(); });
        SetClick(btn_no, () =>  { _onNo?.Invoke(); Close(); });
        SetClick(btn_ok, () =>  { _onOk?.Invoke(); Close(); });

        AddHoverSwap(btn_yes.gameObject);
        AddHoverSwap(btn_no.gameObject);
        AddHoverSwap(btn_ok.gameObject);
    }

    /// <summary>
    /// ��ư ���� ���� (Yes/No �Ǵ� OK �Ǵ� ...)
    /// </summary>
    public void Setup(MessagePopupType type, Action onYes = null, Action onNo = null, Action onOk = null, bool useDim = true)
    {
        _onYes  = onYes;
        _onNo   = onNo;
        _onOk   = onOk;
        _useDim = useDim;

        // ��ư Ȱ��ȭ
        btn_yes.gameObject.SetActive(type == MessagePopupType.YesNo);
        btn_no.gameObject.SetActive(type == MessagePopupType.YesNo);
        btn_ok.gameObject.SetActive(type == MessagePopupType.Ok);
    }

    protected override IEnumerator AnimateOpen()
    {
        SetInteract(false);

        if (_useDim && dim != null)
        {
            dim.alpha = 0;
            dim.gameObject.SetActive(true);
            yield return dim.DOFade(0.8f, _fadeDur).WaitForCompletion();
        }

        SetInteract(true);
    }

    protected override IEnumerator AnimateClose()
    {
        SetInteract(false);

        if (_useDim && dim != null)
        {
            yield return dim.DOFade(0, _fadeDur).WaitForCompletion();
        }
    }

    protected override void SetInteract(bool on)
    {
        base.SetInteract(on);

        if(dim != null && _useDim)
            dim.blocksRaycasts = on;
    }

}
