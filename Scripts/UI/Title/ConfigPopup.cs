using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UtilUI;

/// <summary>
/// Title 씬에서 Config 버튼 클릭 시 나타나는 설정 팝업.
/// </summary>
public sealed class ConfigPopup : SlidePopup
{
    /* 0. Global */
    public CanvasGroup      dim;
    public RectTransform    panel;
    public Image            img_box;
    public Image            img_title;
    public Button           btn_apply;
    public Button           btn_reset;
    public Button           btn_close;
    public TMP_Text         txt_timer;

    /* 1. Resolution Section */
    public Image            img_boxResolution;
    public Image            img_resolution;
    public Button           btn_fullScreen;
    public Button           btn_windowScreen;
    public Button           btn_left;
    public Button           btn_right;
    public Button           btn_resolutionApply;
    public TMP_Text         txt_resolutionTitle;
    public TMP_Text         txt_resolutionDesc;

    /* 2. Sound Section */
    public TMP_Text         txt_soundTitle;
    public TMP_Text         txt_bgm;
    public TMP_Text         txt_bgmDesc;
    public TMP_Text         txt_se;
    public TMP_Text         txt_seDesc;
    public Slider           sld_bgm;
    public Slider           sld_se;

    /* 3. Dialogue Section */
    public TMP_Text         txt_dialogueTitle;
    public TMP_Text         txt_typingSpeed;
    public TMP_Text         txt_typingSpeedDesc;
    public TMP_Text         txt_autoPlaySpeed;
    public TMP_Text         txt_autoPlaySpeedDesc;
    public Slider           sld_typingSpeed;
    public Slider           sld_autoPlaySpeed;

    /* 4. Voice Section */
    public TMP_Text         txt_voiceTitle;
    public TMP_Text         txt_yaeun;
    public TMP_Text         txt_daeun;
    public TMP_Text         txt_bom;
    public TMP_Text         txt_heewon;
    public TMP_Text         txt_yujin;
    public TMP_Text         txt_roa;
    public Slider           sld_yaeun;
    public Slider           sld_daeun;
    public Slider           sld_bom;
    public Slider           sld_heewon;
    public Slider           sld_yujin;
    public Slider           sld_roa;

    #region Private Variables

    private Transform   _titleRoot;                     // 메시지 팝업이 열릴 위치
    private Sprite[]    _resSprites = new Sprite[5];    // 해상도별 스프라이트 배열
    private Sprite      _modeOnSprite;                  // 버튼 켜짐 상태 스프라이트 (fullScreen / windowScreen)
    private Sprite      _modeOffSprite;                 // 버튼 꺼짐 상태 스프라이트 (fullScreen / windowScreen)
    private GameObject  _messagePopupPrefab;            // 설정 변경 확인 메시지 팝업 프리팹
    private bool        _isFullScreen;                  // 전체화면 여부
    private int         _resIndex;                      // 현재 해상도

    private static readonly Vector2Int[] _RES_LIST =    // 지원 해상도 리스트
    {
        new Vector2Int(1920,1080),
        new Vector2Int(1440,900),
        new Vector2Int(1280,1024),
        new Vector2Int(960,720),
        new Vector2Int(800,600)
    };

    private float _tmpBgmValue;             // BGM 임시 값
    private float _tmpSeValue;              // SE 임시 값
    private float _tmpTypingSpeedValue;     // 타이핑 속도 임시 값
    private float _tmpAutoPlaySpeedValue;   // 자동 재생 속도 임시 값
    private float _tmpYaeunValue;           // Yaeun 음성 임시 값
    private float _tmpDaeunValue;           // Daeun 음성 임시 값
    private float _tmpBomValue;             // Bom 음성 임시 값
    private float _tmpHeewonValue;          // Heewon 음성 임시 값
    private float _tmpYujinValue;           // Yujin 음성 임시 값
    private float _tmpRoaValue;             // Roa 음성 임시 값
    #endregion

    private AudioManager _audioManager;     // AudioManager 인스턴스
    private const float _DEFAULT_VOL = 0.4f;// 기본 볼륨 값 (0.4f)

    #region Init

    public override void Init()
    {
        base.Init();

        _audioManager = AudioManager.Instance;
        if (_audioManager == null)
        {
            Debug.Log("[ConfigPopup] AudioManager not found");
        }

        CacheViews();
        InitButtons();
        InitSliderEvents();
        LoadOption();
    }


    /// <summary>
    /// UI 컴포넌트 캐싱
    /// </summary>
    private void CacheViews()
    {
        Transform root = transform;
        panel = _panel;

        // 0. Global
        dim         = _dim;
        img_box     = "img_box".FindIn<Image>(panel);
        img_title   = "img_title".FindIn<Image>(panel);
        btn_apply   = "btn_apply".FindIn<Button>(panel);
        btn_reset   = "btn_reset".FindIn<Button>(panel);
        btn_close   = _btn_close;
        txt_timer   = _txt_timer;

        // 1. Resolution Section
        Transform resolution = "1_resolutionSection".FindIn(panel);
        img_boxResolution   = "img_boxResolution".FindIn<Image>(resolution);
        img_resolution      = "img_resolution".FindIn<Image>(resolution);
        btn_fullScreen      = "btn_fullScreen".FindIn<Button>(resolution);
        btn_windowScreen    = "btn_windowScreen".FindIn<Button>(resolution);
        btn_left            = "btn_left".FindIn<Button>(resolution);
        btn_right           = "btn_right".FindIn<Button>(resolution);
        btn_resolutionApply = "btn_resolutionApply".FindIn<Button>(resolution);
        txt_resolutionTitle = "txt_resolutionTitle".FindIn<TMP_Text>(resolution);
        txt_resolutionDesc  = "txt_resolutionDesc".FindIn<TMP_Text>(resolution);

        // 2. Sound Section
        Transform sound     = "2_soundSection".FindIn(panel);
        txt_soundTitle      = "txt_soundTitle".FindIn<TMP_Text>(sound);
        txt_bgm             = "txt_bgm".FindIn<TMP_Text>(sound);
        txt_bgmDesc         = "txt_bgmDesc".FindIn<TMP_Text>(sound);
        txt_se              = "txt_se".FindIn<TMP_Text>(sound);
        txt_seDesc          = "txt_seDesc".FindIn<TMP_Text>(sound);
        sld_bgm             = "sld_bgm".FindIn<Slider>(sound);
        sld_se              = "sld_se".FindIn<Slider>(sound);

        // 3. Dialogue Section
        Transform dialogue  = "3_dialogueSection".FindIn(panel);
        txt_dialogueTitle   = "txt_dialogueTitle".FindIn<TMP_Text>(dialogue);
        txt_typingSpeed     = "txt_typingSpeed".FindIn<TMP_Text>(dialogue);
        txt_typingSpeedDesc = "txt_typingSpeedDesc".FindIn<TMP_Text>(dialogue);
        txt_autoPlaySpeed   = "txt_autoPlaySpeed".FindIn<TMP_Text>(dialogue);
        txt_autoPlaySpeedDesc = "txt_autoPlaySpeedDesc".FindIn<TMP_Text>(dialogue);
        sld_typingSpeed     = "sld_typingSpeed".FindIn<Slider>(dialogue);
        sld_autoPlaySpeed   = "sld_autoPlaySpeed".FindIn<Slider>(dialogue);

        // 4. Voice Section
        Transform voice     = "4_voiceSection".FindIn(panel);
        txt_voiceTitle      = "txt_voiceTitle".FindIn<TMP_Text>(voice);
        txt_yaeun           = "txt_yaeun".FindIn<TMP_Text>(voice);
        txt_daeun           = "txt_daeun".FindIn<TMP_Text>(voice);
        txt_bom             = "txt_bom".FindIn<TMP_Text>(voice);
        txt_heewon          = "txt_heewon".FindIn<TMP_Text>(voice);
        txt_yujin           = "txt_yujin".FindIn<TMP_Text>(voice);
        txt_roa             = "txt_roa".FindIn<TMP_Text>(voice);
        sld_yaeun           = "sld_yaeun".FindIn<Slider>(voice);
        sld_daeun           = "sld_daeun".FindIn<Slider>(voice);
        sld_bom             = "sld_bom".FindIn<Slider>(voice);
        sld_heewon          = "sld_heewon".FindIn<Slider>(voice);
        sld_yujin           = "sld_yujin".FindIn<Slider>(voice);
        sld_roa             = "sld_roa".FindIn<Slider>(voice);

        // 5. Private Resources
        _titleRoot          = GameObject.Find("TitleRoot")?.transform;
        if (_titleRoot == null)
        {
            Debug.LogError("[ConfigPopup] TitleRoot not found.");
            return;
        }

        _messagePopupPrefab = ResManager.GetTypeResource<GameObject>("Prefabs/prefab_message_popup");

        for (int i = 0; i < _resSprites.Length; i++)
            _resSprites[i] = ResManager.GetTypeResource<Sprite>($"commonUI/prefab_config/resolution_0{i + 1}");

        _modeOnSprite = ResManager.GetTypeResource<Sprite>("commonUI/prefab_config/btn_mode_on");
        _modeOffSprite = ResManager.GetTypeResource<Sprite>("commonUI/prefab_config/btn_mode_off");
    }


    /// <summary>
    /// 버튼 초기화
    /// </summary>
    private void InitButtons()
    {
        // Close 버튼은 TitlePopup에서 이미 초기화됨

        /* global */
        SetClick(btn_apply, ApplyAll);
        SetClick(btn_reset, ResetAll);

        /* resolution */
        SetClick(btn_fullScreen, SetScreenModeUI, true);
        SetClick(btn_windowScreen, SetScreenModeUI, false);
        SetClick(btn_left, ChangeResolution, -1);
        SetClick(btn_right, ChangeResolution, +1);
        SetClick(btn_resolutionApply, ApplyResolution);


    }

    /// <summary>
    /// 슬라이더 이벤트 초기화
    /// </summary>
    private void InitSliderEvents()
    {
        sld_bgm.onValueChanged.AddListener(val => _tmpBgmValue = val);
        sld_se.onValueChanged.AddListener(val => _tmpSeValue = val);
        sld_typingSpeed.onValueChanged.AddListener(val => _tmpTypingSpeedValue = val);
        sld_autoPlaySpeed.onValueChanged.AddListener(val => _tmpAutoPlaySpeedValue = val);
        sld_yaeun.onValueChanged.AddListener(val => _tmpYaeunValue = val);
        sld_daeun.onValueChanged.AddListener(val => _tmpDaeunValue = val);
        sld_bom.onValueChanged.AddListener(val => _tmpBomValue = val);
        sld_heewon.onValueChanged.AddListener(val => _tmpHeewonValue = val);
        sld_yujin.onValueChanged.AddListener(val => _tmpYujinValue = val);
        sld_roa.onValueChanged.AddListener(val => _tmpRoaValue = val);
    }

    #endregion

    #region Option Data Handling

    /// <summary>
    /// 기존에 적장된 옵션을 불러와 UI에 적용한다.
    /// </summary>
    private void LoadOption()
    {
        // Resolution 관련 로드
        _isFullScreen = Screen.fullScreen;
        SetScreenModeUI(_isFullScreen);
        var curRes = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
        _resIndex = System.Array.FindIndex(_RES_LIST, r => r == curRes);
        if (_resIndex < 0)
            _resIndex = 0;

        RefreshResolutionImage();    // 해상도 이미지 갱신

        LoadTmpValuesFromOption();   // 옵션에서 임시 값 로드
        ApplyTmpValuesToSliders();   // 슬라이더에 임시 값 적용
        UpdateAudioManagerVolumes(); // 실제 게임에 임시 값 적용
    }

    /// <summary>
    /// 옵션에서 임시값들을 불러온다.
    /// </summary>
    private void LoadTmpValuesFromOption()
    {
        // * Option 참조 주석 처리

        //_tmpBgmValue = Option.GetOption(Option.OptionType.BGMVolume, 0.4f);
        //_tmpSeValue = Option.GetOption(Option.OptionType.SEVolume, 0.4f);
        //_tmpTypingSpeedValue = Option.GetOption(Option.OptionType.TypingSpeed, 0.4f);
        //_tmpAutoPlaySpeedValue = Option.GetOption(Option.OptionType.AutoPlaySpeed, 0.4f);
        //_tmpYaeunValue = Option.GetOption(Option.OptionType.YaEunVolume, 0.4f);
        //_tmpDaeunValue = Option.GetOption(Option.OptionType.DaEunVolume, 0.4f);
        //_tmpBomValue = Option.GetOption(Option.OptionType.BomVolume, 0.4f);
        //_tmpHeewonValue = Option.GetOption(Option.OptionType.HeeWonVolume, 0.4f);
        //_tmpYujinValue = Option.GetOption(Option.OptionType.YuJinVolume, 0.4f);
        //_tmpRoaValue = Option.GetOption(Option.OptionType.RoaVolume, 0.4f);
    }

    /// <summary>
    /// 임시값들을 옵션에 저장한다.
    /// </summary>
    private void SaveTmpValuesToOption()
    {
        // * Option 참조 주석 처리

        //Option.SetOption(Option.OptionType.BGMVolume, _tmpBgmValue);
        //Option.SetOption(Option.OptionType.SEVolume, _tmpSeValue);
        //Option.SetOption(Option.OptionType.TypingSpeed, _tmpTypingSpeedValue);
        //Option.SetOption(Option.OptionType.AutoPlaySpeed, _tmpAutoPlaySpeedValue);
        //Option.SetOption(Option.OptionType.YaEunVolume, _tmpYaeunValue);
        //Option.SetOption(Option.OptionType.DaEunVolume, _tmpDaeunValue);
        //Option.SetOption(Option.OptionType.BomVolume, _tmpBomValue);
        //Option.SetOption(Option.OptionType.HeeWonVolume, _tmpHeewonValue);
        //Option.SetOption(Option.OptionType.YuJinVolume, _tmpYujinValue);
        //Option.SetOption(Option.OptionType.RoaVolume, _tmpRoaValue);
    }

    /// <summary>
    /// 임시값들을 슬라이더에 적용한다.
    /// </summary>
    private void ApplyTmpValuesToSliders()
    {
        sld_bgm.SetValueWithoutNotify(_tmpBgmValue);
        sld_se.SetValueWithoutNotify(_tmpSeValue);
        sld_typingSpeed.SetValueWithoutNotify(_tmpTypingSpeedValue);
        sld_autoPlaySpeed.SetValueWithoutNotify(_tmpAutoPlaySpeedValue);
        sld_yaeun.SetValueWithoutNotify(_tmpYaeunValue);
        sld_daeun.SetValueWithoutNotify(_tmpDaeunValue);
        sld_bom.SetValueWithoutNotify(_tmpBomValue);
        sld_heewon.SetValueWithoutNotify(_tmpHeewonValue);
        sld_yujin.SetValueWithoutNotify(_tmpYujinValue);
        sld_roa.SetValueWithoutNotify(_tmpRoaValue);
    }

    /// <summary>
    /// 실제 게임에 현재 설정값(tmp)을 적용
    /// </summary>
    private void UpdateAudioManagerVolumes()
    {
        // 실제 볼륨 적용
        _audioManager.BgmVolume = _tmpBgmValue;
        _audioManager.seVolume = _tmpSeValue;
        // TODO: ... (다른 tmpValue들도 적용)
    }

    #endregion

    #region Apply / Reset

    /// <summary>
    /// 'btn_apply' 클릭 시 호출되는 메소드
    /// 현재 설정값을 옵션에 저장하고 실제 게임에 적용한다.
    /// </summary>
    private void ApplyAll()
    {
        
        SaveTmpValuesToOption();        // 임시 값들을 옵션에 저장
        UpdateAudioManagerVolumes();    // 실제 게임에 현재 설정값(tmp)을 적용
    }   

    /// <summary>
    /// 'btn_reset' 클릭 시 호출되는 메소드
    /// 전체 설정을 초기값으로 되돌린다.
    /// </summary>
    private void ResetAll()
    {
        // 기본값으로 초기화
        _tmpBgmValue =              _DEFAULT_VOL;
        _tmpSeValue =               _DEFAULT_VOL;
        _tmpTypingSpeedValue =      _DEFAULT_VOL;
        _tmpAutoPlaySpeedValue =    _DEFAULT_VOL;
        _tmpYaeunValue =            _DEFAULT_VOL;
        _tmpDaeunValue =            _DEFAULT_VOL; 
        _tmpBomValue =              _DEFAULT_VOL;
        _tmpHeewonValue =           _DEFAULT_VOL;
        _tmpYujinValue =            _DEFAULT_VOL;
        _tmpRoaValue =              _DEFAULT_VOL;

        ApplyTmpValuesToSliders();      // 슬라이더에 기본값 적용
        UpdateAudioManagerVolumes();    // 실제 게임에 기본값 적용
        SaveTmpValuesToOption();        // 옵션에 기본값으로 저장
    }

    #endregion

    #region Resolution Handling

    /// <summary>
    /// 해상도를 변경한다.
    /// </summary>
    private void ChangeResolution(int delta)
    {
        _resIndex = (_resIndex + delta + _RES_LIST.Length) % _RES_LIST.Length;
        RefreshResolutionImage();
    }

    /// <summary>
    /// 해상도 이미지를 갱신한다.
    /// </summary>
    private void RefreshResolutionImage() => img_resolution.sprite = _resSprites[Mathf.Clamp(_resIndex, 0, _resSprites.Length - 1)];

    /// <summary>
    /// 화면 모드를 설정한다. (fullscreen / windowScreen)
    /// </summary>
    private void SetScreenModeUI(bool full)
    {
        _isFullScreen = full;
        btn_fullScreen.image.sprite = full ? _modeOnSprite : _modeOffSprite;
        btn_windowScreen.image.sprite = full ? _modeOffSprite : _modeOnSprite;
    }

    /// <summary>
    /// 변경된 해상도를 실제로 적용한다.
    /// </summary>
    private void ApplyResolution()
    {
        var res = _RES_LIST[_resIndex];
        Screen.fullScreenMode = _isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(res.x, res.y, _isFullScreen);
        Debug.Log($"Fullscreen: {_isFullScreen}, Resolution: {res.x}, {res.y}");

        // * Option 참조 주석 처리
        //// 옵션에 저장
        //Option.SetOption(Option.OptionType.IsFullScreen, _isFullScreen);
        //Option.SetOption(Option.OptionType.ResolutionIndex, _resIndex);
    }
    #endregion

    #region Popup Lifecycle / Close

    /// <summary>
    /// 닫기 전에 저장되지 않은 변경 사항이 있는지 확인하고, 있다면 사용자에게 확인 메시지를 표시한다.
    /// </summary>
    public override void Close()
    {
        if (_popupAnimating) return;

        if (HasUnsavedChanges())
        {
            SpawnMessagePopup
            (
                _titleRoot,
                MessagePopup.MessagePopupType.YesNo,
                onYes: SaveAndClose,
                onNo: () => base.Close()
            );
        }
        else
        {
            base.Close();
        }
    }

    /// <summary>
    /// 저장되지 않은 변경 사항이 있는지 확인한다.
    /// </summary>
    private bool HasUnsavedChanges()
    {
        return
            true;
        // * Option 참조 주석 처리
            //!Mathf.Approximately(_tmpBgmValue,      Option.GetOption(Option.OptionType.BGMVolume, 0.4f)) ||
            //!Mathf.Approximately(_tmpSeValue,       Option.GetOption(Option.OptionType.SEVolume, 0.4f)) ||
            //!Mathf.Approximately(_tmpTypingSpeedValue, Option.GetOption(Option.OptionType.TypingSpeed, 0.4f)) ||
            //!Mathf.Approximately(_tmpAutoPlaySpeedValue, Option.GetOption(Option.OptionType.AutoPlaySpeed, 0.4f)) ||
            //!Mathf.Approximately(_tmpYaeunValue,    Option.GetOption(Option.OptionType.YaEunVolume, 0.4f)) ||
            //!Mathf.Approximately(_tmpDaeunValue,    Option.GetOption(Option.OptionType.DaEunVolume, 0.4f)) ||
            //!Mathf.Approximately(_tmpBomValue,      Option.GetOption(Option.OptionType.BomVolume, 0.4f)) ||
            //!Mathf.Approximately(_tmpHeewonValue,   Option.GetOption(Option.OptionType.HeeWonVolume, 0.4f)) ||
            //!Mathf.Approximately(_tmpYujinValue,    Option.GetOption(Option.OptionType.YuJinVolume, 0.4f)) ||
            //!Mathf.Approximately(_tmpRoaValue,      Option.GetOption(Option.OptionType.RoaVolume, 0.4f));
    }

    /// <summary>
    /// 저장하고 팝업을 닫는다.
    /// </summary>
    private void SaveAndClose()
    {
        ApplyAll();
        base.Close();
    }

    #endregion
}
