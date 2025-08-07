using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UtilUI;

public sealed class TitleView : MonoBehaviour
{
    public Image img_logo;
    public Image img_bottomGradient;
    public Image img_bottomGraphic;
    public Button btn_start;
    public Button btn_continue;
    public Button btn_load;
    public Button btn_extra;
    public Button btn_config;
    public Button btn_exit;
    public TMP_Text txt_copyright;

    #region Private Variables

    private Transform _titleRoot;
    private Sprite _bottomGraphicNone;
    private Sprite _bottomGraphicStart;
    private Sprite _bottomGraphicContinue;
    private Sprite _bottomGraphicLoad;
    private Sprite _bottomGraphicExtra;
    private Sprite _bottomGraphicConfig;
    private Sprite _bottomGraphicExit;

    #endregion


    #region Init
    public void Init()
    {
        CacheViews();
        // ApplyDesignTitle();
        InitButtons();

        img_bottomGraphic.sprite = _bottomGraphicNone;
        Refresh();
    }

    /// <summary>
    /// UI 컴포넌트 캐싱
    /// </summary>
    private void CacheViews()
    {
        Transform root = transform;

        img_logo = "img_logo".FindIn<Image>(root);
        img_bottomGradient = "img_bottomGradient".FindIn<Image>(root);
        img_bottomGraphic = "img_bottomGraphic".FindIn<Image>(root);
        btn_start = "btn_start".FindIn<Button>(root);
        btn_continue = "btn_continue".FindIn<Button>(root);
        btn_load = "btn_load".FindIn<Button>(root);
        btn_extra = "btn_extra".FindIn<Button>(root);
        btn_config = "btn_config".FindIn<Button>(root);
        btn_exit = "btn_exit".FindIn<Button>(root);
        txt_copyright = "txt_copyright".FindIn<TMP_Text>(root);

        /* Private Variables */
        _titleRoot = GameObject.Find("TitleRoot")?.transform;
        if (_titleRoot == null)
        {
            Debug.LogError("[TitleUI] TitleRoot not found");
            return;
        }

        _bottomGraphicNone = ResManager.GetTypeResource<Sprite>("commonUI/prefab_title/titleBottom_graphic_none");
        _bottomGraphicStart = ResManager.GetTypeResource<Sprite>("commonUI/prefab_title/titleBottom_graphic_start");
        _bottomGraphicContinue = ResManager.GetTypeResource<Sprite>("commonUI/prefab_title/titleBottom_graphic_continue");
        _bottomGraphicLoad = ResManager.GetTypeResource<Sprite>("commonUI/prefab_title/titleBottom_graphic_load");
        _bottomGraphicExtra = ResManager.GetTypeResource<Sprite>("commonUI/prefab_title/titleBottom_graphic_extra");
        _bottomGraphicConfig = ResManager.GetTypeResource<Sprite>("commonUI/prefab_title/titleBottom_graphic_config");
        _bottomGraphicExit = ResManager.GetTypeResource<Sprite>("commonUI/prefab_title/titleBottom_graphic_exit");
    }

    /// <summary>
    /// 버튼 초기화 및 이벤트 연결
    /// </summary>
    private void InitButtons()
    {
        // 클릭 이벤트 바인딩
        SetClick(btn_start, OnStart);
        SetClick(btn_continue, OnContinue);
        SetClick(btn_load, OnLoad);
        SetClick(btn_extra, OnExtra);
        SetClick(btn_config, OnConfig);
        SetClick(btn_exit, OnExit);

        // (normal / hover) 토글
        AddHoverSwap(btn_start.gameObject);
        AddHoverSwap(btn_continue.gameObject);
        AddHoverSwap(btn_load.gameObject);
        AddHoverSwap(btn_extra.gameObject);
        AddHoverSwap(btn_config.gameObject);
        AddHoverSwap(btn_exit.gameObject);

        // 버튼별 호버시 표시할 bottomGraphic 등록
        AddHoverCallbacks(btn_start.gameObject, onEnter: () => img_bottomGraphic.sprite = _bottomGraphicStart, onExit: () => img_bottomGraphic.sprite = _bottomGraphicNone);
        AddHoverCallbacks(btn_continue.gameObject, onEnter: () => img_bottomGraphic.sprite = _bottomGraphicContinue, onExit: () => img_bottomGraphic.sprite = _bottomGraphicNone);
        AddHoverCallbacks(btn_load.gameObject, onEnter: () => img_bottomGraphic.sprite = _bottomGraphicLoad, onExit: () => img_bottomGraphic.sprite = _bottomGraphicNone);
        AddHoverCallbacks(btn_extra.gameObject, onEnter: () => img_bottomGraphic.sprite = _bottomGraphicExtra, onExit: () => img_bottomGraphic.sprite = _bottomGraphicNone);
        AddHoverCallbacks(btn_config.gameObject, onEnter: () => img_bottomGraphic.sprite = _bottomGraphicConfig, onExit: () => img_bottomGraphic.sprite = _bottomGraphicNone);
        AddHoverCallbacks(btn_exit.gameObject, onEnter: () => img_bottomGraphic.sprite = _bottomGraphicExit, onExit: () => img_bottomGraphic.sprite = _bottomGraphicNone);
    }
    #endregion
    #region Button Handlers

    private void OnStart()
    {
        Debug.Log("Start 클릭");

        // 새 게임 시작 상태 설정
        GameManager.Instance.StartNewGame();

        // 로딩씬으로 전환
        SceneManager.LoadSceneAsync("GameScene");
    }

    private void OnContinue() { Debug.Log("Continue 클릭"); }

    private void OnLoad()
    {
        // * Option 참조 주석 처리
        //string language = Option.CurrentLanguage();

        string language = "kr";
        SpawnPopup<LoadPopup>($"Prefabs/prefab_load_{language}", _titleRoot);
    }

    private void OnExtra()
    {
        // * Option 참조 주석 처리
        //string language = Option.CurrentLanguage();

        string language = "kr";
        SpawnPopup<ExtraPopup>($"Prefabs/prefab_extra_{language}", _titleRoot);

    }

    private void OnConfig()
    {
        // * Option 참조 주석 처리
        //string language = Option.CurrentLanguage();

        string language = "kr";
        SpawnPopup<ConfigPopup>($"Prefabs/prefab_config_{language}", _titleRoot);
    }

    private void OnExit()
    {
#if UNITY_EDITOR
        // 에디터에서는 플레이 모드를 종료
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    #endregion

    #region Internal Methods

    /// <summary>
    /// UI 재설정 (언어가 바뀌는 상황등에 적용)
    /// </summary>
    private void Refresh()
    {
        ApplyValidHoverButtonSprite();
    }

    /// <summary>
    /// 버튼 호버 시 언어에 따른 이미지를 적용
    /// </summary>
    private void ApplyValidHoverButtonSprite()
    {
        // * Option 참조 주석 처리
        //string language = Option.CurrentLanguage();

        string language = "kr";

        // go 찾기
        var hoverStart = "hover".FindIn<Image>(btn_start.transform);
        var hoverContinue = "hover".FindIn<Image>(btn_continue.transform);
        var hoverLoad = "hover".FindIn<Image>(btn_load.transform);
        var hoverExtra = "hover".FindIn<Image>(btn_extra.transform);
        var hoverConfig = "hover".FindIn<Image>(btn_config.transform);
        var HoverExit = "hover".FindIn<Image>(btn_exit.transform);

        // path 찾기
        string hoverStartPath = $"commonUI/prefab_title/titleBtn_start_{language}";
        string hoverContinuePath = $"commonUI/prefab_title/titleBtn_continue_{language}";
        string hoverLoadPath = $"commonUI/prefab_title/titleBtn_load_{language}";
        string hoverExtraPath = $"commonUI/prefab_title/titleBtn_extra_{language}";
        string hoverConfigPath = $"commonUI/prefab_title/titleBtn_config_{language}";
        string hoverExitPath = $"commonUI/prefab_title/titleBtn_exit_{language}";

        // 적용
        SetImage(hoverStart, hoverStartPath);
        SetImage(hoverContinue, hoverContinuePath);
        SetImage(hoverLoad, hoverLoadPath);
        SetImage(hoverExtra, hoverExtraPath);
        SetImage(hoverConfig, hoverConfigPath);
        SetImage(HoverExit, hoverExitPath);
    }

    /// <summary>
    /// 알맞은 DesignTitle 정보를 UI에 적용
    /// </summary>
    public void ApplyDesignTitle(int id)
    {
        var table = TableManager.Instance.GetComponentInChildren<DesignTitle>();
        if (table == null || table._data.Count == 0)
        {
            Debug.LogError("DesignTitle 테이블 로드 실패");
            return;
        }

        var row = table._data[id - 1];

        // 배경 설정
        SetBackground($"{row.texture._folder}{row.texture._texture}");

        // 로고 설정
        SetImage(img_logo, $"{row.sprite._folder}{row.sprite._subFolder}/{row.sprite._image}");

        // copyright 설정 (특정 언어가 달라진다면...)
        txt_copyright.text = "Copyright 2025 Team LUCID all right reserved";
    }

    #endregion
}