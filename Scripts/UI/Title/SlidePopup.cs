using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UtilUI;

/// <summary>
/// 슬라이드 애니메이션 팝업 베이스 클래스.
/// LoadPopup, ExtraPopup, ConfigPoup에서 상속받아 사용.
/// </summary>
public class SlidePopup : PopupBase
{
    protected CanvasGroup   _dim;                         // 어두운 뒷배경 (Image + Button)
    protected Button        _btn_close;                      // 닫기 버튼
    protected TMP_Text      _txt_timer;
    protected float _slideDur = .5f;                    // 슬라이드 애니메이션 시간
    protected float _fadeDur = .3f;                     // 페이드 애니메이션 시간

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

        // close버튼 또는 dim 클릭 시 팝업 닫기 (팝업 애니메이션 중에는 클릭 무시)
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
    /// 팝업 열기 애니메이션
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
    /// 팝업 닫기 애니메이션
    /// </summary>
    protected override IEnumerator AnimateClose()
    {
        SetInteract(false);

        _dim.DOFade(0, _fadeDur);

        var closePos = _panel.anchoredPosition + Vector2.right * (Screen.width + 400f);
        yield return _panel.DOAnchorPos(closePos, _slideDur).SetEase(Ease.InCubic).WaitForCompletion();
    }

    /// <summary>
    /// 팝업 인터랙션 토글
    /// </summary>
    protected override void SetInteract(bool on)
    {
        base.SetInteract(on);

        if (_dim != null)
           _dim.blocksRaycasts = on;
    }
    #endregion
}
