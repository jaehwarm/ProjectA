using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UtilUI;

/// <summary>
/// Title ������ Load ��ư Ŭ�� �� ��Ÿ���� ���̺�ε� �˾�.
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

    private Transform   _titleRoot;             // �޽��� �˾��� ���� ��ġ
    private GameObject  _saveSlotPrefab;        // ���� ���� ������
    private const int   _slotPerPage = 6;       // �������� ���� ��
    private int         _pageCurrent;           // ���� ������ �ε���
    private int         _pageMax;               // �ִ� ������ ��

    #endregion

    /* �׽�Ʈ�� �ӽ� ������ */
    private readonly List<TempSaveData> _temp = new(); // �ӽ� ���� ������ ����Ʈ

    #region Init
    public override void Init()
    {
        base.Init();

        CacheViews();
        InitButtons();

        CreateTempSaveData();   // �׽�Ʈ�� �ӽ� ������ ����
        Refresh();              // ���� ��� ����
    }

    /// <summary>
    /// UI ������Ʈ ĳ��
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
        // ��ư �̺�Ʈ ���ε�
        SetClick(btn_prev, MovePage, -1);
        SetClick(btn_next, MovePage, +1);

        AddHoverSwap(btn_prev.gameObject);
        AddHoverSwap(btn_next.gameObject);
    }
    #endregion

    #region Slot Handlers

    // TODO: TmpSaveSlot => Option.SaveSlotData�� ��ü.
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
    /// ������ �̵�
    /// </summary>
    private void MovePage(int delta)
    {
        _pageCurrent = Mathf.Clamp(_pageCurrent + delta, 0, _pageMax - 1);
        Refresh();
    }

    /// <summary>
    /// ���� ��� ���� �� ������ UI ������Ʈ
    /// </summary>
    private void Refresh()
    {
        // ���� ���� ����
        foreach (Transform child in slotGrid.transform)
            Destroy(child.gameObject);

        // ���� �������� ǥ���� saveSlot���� ����
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
    /// ���� Ŭ�� �ڵ鷯
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
/// �׽�Ʈ�� �ӽ� SaveData
/// TODO: TmpSaveSlot => Option.SaveSlotData�� ��ü.
/// </summary>
[Serializable]
public struct TempSaveData
{
    public bool HasData;     // ������ ���� ����
    public Sprite ThumbSprite; // ����� �̹���
    public string Title;     // ���� Ÿ��Ʋ
    public DateTime Time;    // ���� �ð�

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
