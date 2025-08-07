using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UtilUI;

/// <summary>
/// Title 씬에서 Extra 버튼 클릭 시 나타나는 엑스트라 팝업.
/// </summary>
public sealed class ExtraPopup : SlidePopup
{
    /* 0. Global */
    public CanvasGroup      dim;
    public RectTransform    panel;
    public Image            img_title;
    public Button           btn_close;
    public TMP_Text         txt_timer;
    
    /* 1. Extra Buttons */
    public Button           btn_scene;     
    public Button           btn_cg;        
    public Button           btn_collect;


    #region Private Variables
    
    private float _hoverMoveY   = 10f;      // Hover 이동 거리
    private float _hoverDur     = 0.2f;     // Hover 애니메이션 시간

    #endregion

    #region Init

    public override void Init()
    {
        base.Init();
        CacheViews();
        InitButtons();
    }

    /// <summary>
    /// UI 컴포넌트 캐싱
    /// </summary>
    private void CacheViews()
    {
        Transform root = transform;
        panel = _panel;

        // Global
        dim = _dim;
        btn_close   = _btn_close;
        img_title   = "img_title".FindIn<Image>(panel);
        txt_timer   = _txt_timer;

        // Extra Buttons
        btn_scene   = "btn_scene".FindIn<Button>(panel);
        btn_cg      = "btn_cg".FindIn<Button>(panel);
        btn_collect = "btn_collect".FindIn<Button>(panel);
    }

    /// <summary>
    /// 버튼 초기화
    /// </summary>
    private void InitButtons()
    {      
        InitExtraButton(btn_scene, "scene");
        InitExtraButton(btn_cg, "cg");
        InitExtraButton(btn_collect, "collect");
    }

    /// <summary>
    /// 개별 버튼에 클릭 및 hover 효과 등록
    /// </summary>
    private void InitExtraButton(Button btn, string id)
    {
        SetClick(btn, () => Debug.Log($"{id} Button Clicked"));

        // 이동 애니메이션
        Vector2 offset = new Vector2(0, _hoverMoveY);
        AddHoverMove(btn.gameObject, offset, _hoverDur, isAllowed: () => !_popupAnimating);
        AddHoverSwap(btn.gameObject);
    }

    #endregion
}
