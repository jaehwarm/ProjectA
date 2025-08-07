using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UtilUI;

/// <summary>
/// �����̵� �ִϸ��̼� �˾� ���̽� Ŭ����.
/// LoadPopup, ExtraPopup, ConfigPoup���� ��ӹ޾� ���.
/// </summary>
public class SlidePopup : PopupBase
{
    protected CanvasGroup   _dim;                         // ��ο� �޹�� (Image + Button)
    protected Button        _btn_close;                      // �ݱ� ��ư
    protected TMP_Text      _txt_timer;
    protected float _slideDur = .5f;                    // �����̵� �ִϸ��̼� �ð�
    protected float _fadeDur = .3f;                     // ���̵� �ִϸ��̼� �ð�

    protected override void Update()
    {
        base.Update();

        if (_txt_timer != null)
            _txt_timer.text = DateTime.Now.ToString("HH:mm"); 
    }

    public override void Init()
    {
        base.Init();

        Transform root = transform;
        _dim = "dim".FindIn<CanvasGroup>(root);
        

        Transform panel = _panel.transform;
        _txt_timer = "txt_timer".FindIn<TMP_Text>(panel);
        _btn_close = "btn_close".FindIn<Button>(panel);        

        // close��ư �Ǵ� dim Ŭ�� �� �˾� �ݱ� (�˾� �ִϸ��̼� �߿��� Ŭ�� ����)
        SetClickIf(_btn_close, condition: () => !_popupAnimating, Close);
        SetClickIf(_dim.GetComponent<Button>(), condition: () => !_popupAnimating, Close);
    }

    protected override void OnOpened()
    {
        base.OnOpened();
        SetInteract(true);
    }

    protected override void OnClosed()
    {
        base.OnClosed();
    }

    #region Popup Animation
    /// <summary>
    /// �˾� ���� �ִϸ��̼�
    /// </summary>
    protected override IEnumerator AnimateOpen()
    {
        SetInteract(false);

        _dim.alpha = 0;
        _dim.gameObject.SetActive(true);
        _dim.DOFade(0.8f, _fadeDur);

        var startPos = _panel.anchoredPosition + Vector2.right * (Screen.width + 400f);
        var targetPos = _panel.anchoredPosition;
        _panel.anchoredPosition = startPos;

        yield return _panel.DOAnchorPos(targetPos, _slideDur).SetEase(Ease.OutCubic).WaitForCompletion();
    }

    /// <summary>
    /// �˾� �ݱ� �ִϸ��̼�
    /// </summary>
    protected override IEnumerator AnimateClose()
    {
        SetInteract(false);

        _dim.DOFade(0, _fadeDur);

        var closePos = _panel.anchoredPosition + Vector2.right * (Screen.width + 400f);
        yield return _panel.DOAnchorPos(closePos, _slideDur).SetEase(Ease.InCubic).WaitForCompletion();
    }

    /// <summary>
    /// �˾� ���ͷ��� ���
    /// </summary>
    protected override void SetInteract(bool on)
    {
        base.SetInteract(on);

        if (_dim != null)
           _dim.blocksRaycasts = on;
    }
    #endregion
}
