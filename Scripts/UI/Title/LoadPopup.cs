using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UtilUI;

/// <summary>
/// Title 씬에서 Load 버튼 클릭 시 나타나는 세이브로드 팝업.
/// </summary>
public sealed class LoadPopup : SlidePopup
{
    /* 0. Global */
    public CanvasGroup      dim;
    public RectTransform    panel;
    public Image            img_title;
    public Button           btn_close;
    public TMP_Text         txt_timer;

    /* 1. Slot Handlers */
    public Image            img_boxPage;
    public Button           btn_next;
    public Button           btn_prev;
    private TMP_Text        txt_boxPage_text;
    public GridLayoutGroup  slotGrid;

    #region Private Variables  

    private Transform   _titleRoot;             // 메시지 팝업이 열릴 위치
    private GameObject  _saveSlotPrefab;        // 저장 슬롯 프리팹
    private const int   _slotPerPage = 6;       // 페이지당 슬롯 수
    private int         _pageCurrent;           // 현재 페이지 인덱스
    private int         _pageMax;               // 최대 페이지 수

    #endregion

    /* 테스트용 임시 데이터 */
    private readonly List<TempSaveData> _temp = new(); // 임시 저장 데이터 리스트

    #region Init
    public override void Init()
    {
        base.Init();

        CacheViews();
        InitButtons();

        CreateTempSaveData();   // 테스트용 임시 데이터 생성
        Refresh();              // 슬롯 목록 갱신
    }

    /// <summary>
    /// UI 컴포넌트 캐싱
    /// </summary>
    private void CacheViews()
    {
        Transform root = transform;
        panel       = _panel;
        dim         = _dim;
        btn_close   = _btn_close;
        txt_timer   = _txt_timer;

        img_title       = "img_title".FindIn<Image>(panel);
        img_boxPage     = "img_boxPage".FindIn<Image>(panel);
        txt_boxPage_text = "img_boxPage/text".FindIn<TMP_Text>(panel);
        btn_prev        = "btn_prev".FindIn<Button>(panel);
        btn_next        = "btn_next".FindIn<Button>(panel);
        slotGrid        = "slotArea/slotGrid".FindIn<GridLayoutGroup>(panel);

        _saveSlotPrefab     = ResManager.GetTypeResource<GameObject>("Prefabs/prefab_saveSlot");

        _titleRoot      = GameObject.Find("TitleRoot")?.transform;
        if (_titleRoot == null)
        {
            Debug.LogError("[LoadPopup] TitleRoot not found.");
        }
    }

    private void InitButtons()
    {
        // 버튼 이벤트 바인딩
        SetClick(btn_prev, MovePage, -1);
        SetClick(btn_next, MovePage, +1);

        AddHoverSwap(btn_prev.gameObject);
        AddHoverSwap(btn_next.gameObject);
    }
    #endregion

    #region Slot Handlers

    // TODO: TmpSaveSlot => Option.SaveSlotData로 교체.
    public void CreateTempSaveData()
    {
        Sprite thumb = Resources.Load<Sprite>("backgroundImage/title_campusDay_01");
        if (thumb == null)
        {
            Debug.Log("[LoadPopup] thumb's Sprite does not found");
            return;
        }
        for (int i = 0; i < 13; i++)
            _temp.Add(new TempSaveData(true, thumb, $"SAVE {i}", DateTime.Now));

        _pageMax = Mathf.CeilToInt(_temp.Count / (float)_slotPerPage);
    }

    /// <summary>
    /// 페이지 이동
    /// </summary>
    private void MovePage(int delta)
    {
        _pageCurrent = Mathf.Clamp(_pageCurrent + delta, 0, _pageMax - 1);
        Refresh();
    }

    /// <summary>
    /// 슬롯 목록 갱신 및 페이지 UI 업데이트
    /// </summary>
    private void Refresh()
    {
        // 기존 슬롯 제거
        foreach (Transform child in slotGrid.transform)
            Destroy(child.gameObject);

        // 현재 페이지에 표시할 saveSlot들을 생성
        int start = _pageCurrent * _slotPerPage;
        for (int i = 0; i < _slotPerPage; i++)
        {
            int idx = start + i;
            TempSaveData data = idx < _temp.Count ? _temp[idx] : TempSaveData.Empty;
            
            var go = Instantiate(_saveSlotPrefab, slotGrid.transform);
            var slot = go.AddComponent<SaveSlotView>();
            slot.Init(idx, data, OnSlotClicked);
        }

        txt_boxPage_text.text = $"{_pageCurrent + 1}";
        btn_prev.interactable = _pageCurrent > 0;
        btn_next.interactable = _pageCurrent < _pageMax - 1;
    }

    /// <summary>
    /// 슬롯 클릭 핸들러
    /// </summary>
    private void OnSlotClicked(int idx)
    {
        SpawnMessagePopup
            (
            _titleRoot,
            MessagePopup.MessagePopupType.YesNo,
            onYes: () => Debug.Log($"Play saveSlot: {idx}"),
            onNo: () => Debug.Log("Canceled")
        );
    }

    #endregion

}

#region TemSaveData
/// <summary>
/// 테스트용 임시 SaveData
/// TODO: TmpSaveSlot => Option.SaveSlotData로 교체.
/// </summary>
[Serializable]
public struct TempSaveData
{
    public bool HasData;     // 데이터 존재 여부
    public Sprite ThumbSprite; // 썸네일 이미지
    public string Title;     // 저장 타이틀
    public DateTime Time;    // 저장 시각

    public TempSaveData(bool hasData, Sprite thumb, string title, DateTime time)
    {
        HasData = hasData;
        ThumbSprite = thumb;
        Title = title;
        Time = time;
    }

    public static readonly TempSaveData Empty = new TempSaveData(false, null, string.Empty, DateTime.MinValue);
}

#endregion
