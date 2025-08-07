using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UtilUI
{
    #region Find

    /// <summary>
    /// 문자열 경로를 기준으로 하위 오브젝트에서 Component<T>를 찾아 반환
    /// </summary>
    public static T FindIn<T>(this string path, Transform parent) where T : Component
    {
        var t = parent.Find(path);
        if (t == null)
        {
            Debug.LogError($"[UtilUI] Transform '{path}' not found under '{parent.name}'");
            return null;
        }

        var comp = t.GetComponent<T>();
        if (comp == null)
        {
            Debug.LogError($"[UtilUI] Component '{typeof(T)}' not found on '{path}'");
        }

        return comp;
    }

    /// <summary>
    /// 문자열 경로를 기준으로 하위 오브젝트의 Transform을 찾아 반환
    /// </summary>
    public static Transform FindIn(this string path, Transform parent)
    {
        var t = parent.Find(path);
        if (t == null)
            Debug.LogError($"[UtilUI] Transform '{path}' not found under '{parent.name}'");
        return t;
    }

    #endregion

    #region Hover

    /// <summary>
    /// EventTrigger에 Entry 추가 (PointerEnter, PointerExit 등)
    /// </summary>
    public static void AddEntry(EventTrigger trigger, EventTriggerType type, Action<BaseEventData> callback)
    {
        if (trigger == null) return;

        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(new UnityAction<BaseEventData>(callback));
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// Hover 진입/종료 시 실행할 동작을 각각 등록
    /// </summary>
    public static void AddHoverCallbacks(EventTrigger trigger, Action onEnter, Action onExit)
    {
        AddEntry(trigger, EventTriggerType.PointerEnter, _ => onEnter?.Invoke());
        AddEntry(trigger, EventTriggerType.PointerExit, _ => onExit?.Invoke());
    }

    /// <summary>
    /// Hover 진입/종료 시 실행할 동작을 각각 등록 (GameObject 버전)
    /// </summary>
    public static void AddHoverCallbacks(GameObject go, Action onEnter, Action onExit)
    {
        var trigger= go.GetOrAddComponent<EventTrigger>();
        AddHoverCallbacks(trigger, onEnter, onExit);
    }

    /// <summary>
    /// Hover 시 지정된 offset만큼 이동
    /// </summary>
    public static void AddHoverMove(GameObject go, Vector2 offset, 
                                    float duration = 0.2f, 
                                    Func<bool> isAllowed = null, 
                                    Ease ease = Ease.OutBack)
    {
        var rect = go.GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.LogWarning($"[UtilUI] '{go.name}' has no RectTransform.");
            return;
        }

        var trigger = go.GetOrAddComponent<EventTrigger>();
        Vector2 origin = rect.anchoredPosition;

        AddHoverCallbacks(trigger,
            onEnter: () =>  { if (isAllowed?.Invoke() ?? true) rect.DOAnchorPos(origin + offset, duration).SetEase(ease); },
            onExit: () =>   { if (isAllowed?.Invoke() ?? true) rect.DOAnchorPos(origin, duration).SetEase(ease); });
    }

    /// <summary>
    /// Hover 시 스프라이트 스왑
    /// </summary>
    public static void AddHoverSwap(GameObject go)
    {
        if (go == null)
        {
            Debug.LogWarning("[UtilUI] buttonGO is null. Cannot add hover swap.");
            return;
        }

        var normal = go.transform.Find("normal")?.gameObject;
        var hover = go.transform.Find("hover")?.gameObject;

        if (normal == null || hover == null)
        {
            Debug.LogWarning($"[UtilUI] '{go.name}' is missing a 'normal' or 'hover' child GameObject.");
            return;
        }

        var trigger = go.GetOrAddComponent<EventTrigger>();
        AddHoverCallbacks(trigger,
        onEnter: () => { normal.SetActive(false); hover.SetActive(true); },
        onExit: () => { normal.SetActive(true); hover.SetActive(false); });
    }
    #endregion

    #region Button

    /// <summary>
    /// 버튼 클릭 시 특정 액션을 설정
    /// </summary>
    public static void SetClick(Button btn, UnityAction action)
    {
        if(btn == null || action == null) return;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
    }

    /// <summary>
    /// 버튼 클릭 시 특정 타입의 인자를 전달하는 액션을 설정
    /// </summary>
    public static void SetClick<T>(Button btn, Action<T> action, T arg)
    {
        if (btn == null || action == null) return;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => action(arg));
    }

    /// <summary>
    /// 튼 클릭 시 특정 조건을 만족할 때만 액션을 실행 (지연 평가)
    /// </summary>
    public static void SetClickIf(Button btn, Func<bool> condition, UnityAction action)
    {
        if (btn == null || condition == null || action == null) return;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            if (condition())
                action();
        });
    }

    #endregion

    #region Image

    /// <summary>
    /// 이미지에 스프라이트를 설정하고, 필요시 NativeSize로 조정
    /// </summary>
    public static void SetImage(Image img, string path, bool setNativeSize = false)
    {
        if (img == null || string.IsNullOrEmpty(path)) return;

        Sprite spr = ResManager.GetTypeResource<Sprite>(path);
        if (spr != null)
        {
            img.sprite = spr;
            if (setNativeSize) img.SetNativeSize();
        }
    }

    /// <summary>
    /// 배경을 변경한다.
    /// 
    public static void SetBackground(string spritePath, bool setNativeSize = false)
    {
        var backgroundGO = GetBackground();

        // img_background를 찾기
        var img = backgroundGO.transform.Find("img_background")?.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("[UtilUI] 'img_background' not found under 'prefab_background'.");
            return;
        }

        // 스프라이트 로드 및 적용
        Sprite sprite = ResManager.GetTypeResource<Sprite>(spritePath);
        if (sprite == null)
        {
            Debug.LogError($"[UtilUI] Sprite not found at path: {spritePath}");
            return;
        }

        img.sprite = sprite;
        if (setNativeSize) img.SetNativeSize();
    }

    /// <summary>
    /// 현재 씬에 배치된 prefab_background를 반환한다.
    /// </summary>
    public static GameObject GetBackground()
    {
        var backgroundGO = GameObject.Find("prefab_background");
        if (backgroundGO == null)
        {
            Debug.LogError("[UtilUI] 'prefab_background' not found in the current scene.");
        }
        return backgroundGO;
    }

    #endregion

    #region Popup
    public static T SpawnPopup<T>(string prefabPath, Transform parent, Action<T> setup = null,bool allowMultiple = false) 
      where T : PopupBase
    {
        // 중복 허용 여부 확인
        if (!allowMultiple && GameObject.FindObjectOfType<T>() != null)
            return null;

        var prefab = ResManager.GetTypeResource<GameObject>(prefabPath);
        var go = GameObject.Instantiate(prefab, parent);
        var popup = go.GetOrAddComponent<T>();
        popup.Init();
        setup?.Invoke(popup);
        popup.Open();
        return popup;
    }

    public static MessagePopup SpawnMessagePopup(
      Transform parent,
      MessagePopup.MessagePopupType type = MessagePopup.MessagePopupType.YesNo,
      Action onYes = null,
      Action onNo = null,
      Action onOk = null,
      bool useDim = true,
      bool allowMultiple = false)
    {
        return SpawnPopup<MessagePopup>(
            "Prefabs/prefab_message_popup", parent,
            popup => popup.Setup(type, onYes, onNo, onOk, useDim),
            allowMultiple
        );
    }

    #endregion
}
