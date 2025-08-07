using UnityEngine;
using UnityEditor;
using TMPro;

/// <summary>
/// 2025 - 07 - 17 (JUN)
/// Awake���� ��� ������ �ǵ���༭ ������ Start
/// </summary>
public class LanguageApply : MonoBehaviour
{
    [SerializeField] int _id;

    void Start()
    {
        var label = gameObject.GetComponent<TMP_Text>();
        if (label == null) 
            return;

        label.text = TableManager.String(_id);

#if !UNITY_EDITOR
        Invoke(nameof(DestroySelf), 0.1f);
#endif
    }

    void DestroySelf()
    {
        Destroy(this);
    }
}