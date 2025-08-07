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
    /// ���ڿ� ��θ� �������� ���� ������Ʈ���� Component<T>�� ã�� ��ȯ
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
    /// ���ڿ� ��θ� �������� ���� ������Ʈ�� Transform�� ã�� ��ȯ
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
    /// EventTrigger�� Entry �߰� (PointerEnter, PointerExit ��)
    /// </summary>
    public static void AddEntry(EventTrigger trigger, EventTriggerType type, Action<BaseEventData> callback)
    {
        if (trigger == null) return;

        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(new UnityAction<BaseEventData>(callback));
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// Hover ����/���� �� ������ ������ ���� ���
    /// </summary>
    public static void AddHoverCallbacks(EventTrigger trigger, Action onEnter, Action onExit)
    {
        AddEntry(trigger, EventTriggerType.PointerEnter, _ => onEnter?.Invoke());
        AddEntry(trigger, EventTriggerType.PointerExit, _ => onExit?.Invoke());
    }

    /// <summary>
    /// Hover ����/���� �� ������ ������ ���� ��� (GameObject ����)
    /// </summary>
    public static void AddHoverCallbacks(GameObject go, Action onEnter, Action onExit)
    {
        var trigger= go.GetOrAddComponent<EventTrigger>();
        AddHoverCallbacks(trigger, onEnter, onExit);
    }

    /// <summary>
    /// Hover �� ������ offset��ŭ �̵�
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
    /// Hover �� ��������Ʈ ����
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
    /// ��ư Ŭ�� �� Ư�� �׼��� ����
    /// </summary>
    public static void SetClick(Button btn, UnityAction action)
    {
        if(btn == null || action == null) return;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
    }

    /// <summary>
    /// ��ư Ŭ�� �� Ư�� Ÿ���� ���ڸ� �����ϴ� �׼��� ����
    /// </summary>
    public static void SetClick<T>(Button btn, Action<T> action, T arg)
    {
        if (btn == null || action == null) return;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => action(arg));
    }

    /// <summary>
    /// ư Ŭ�� �� Ư�� ������ ������ ���� �׼��� ���� (���� ��)
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
    /// �̹����� ��������Ʈ�� �����ϰ�, �ʿ�� NativeSize�� ����
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
    /// ����� �����Ѵ�.
    /// 
    public static void SetBackground(string spritePath, bool setNativeSize = false)
    {
        var backgroundGO = GetBackground();

        // img_background�� ã��
        var img = backgroundGO.transform.Find("img_background")?.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("[UtilUI] 'img_background' not found under 'prefab_background'.");
            return;
        }

        // ��������Ʈ �ε� �� ����
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
    /// ���� ���� ��ġ�� prefab_background�� ��ȯ�Ѵ�.
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
        // �ߺ� ��� ���� Ȯ��
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
