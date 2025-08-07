using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UtilUI;

public class SaveSlotView : MonoBehaviour
{
    public Transform    noneSlot;
    public Image        img_dataBoxNone;
    public TMP_Text     txt_noneTitle;

    public Transform    saveSlot;
    public Image        img_dataBoxSave;
    public Image        img_thumb;
    public TMP_Text     txt_saveTitle;
    public TMP_Text     txt_date;

    private Button      _button;            // ���� ���� ��ư

    /// <summary>
    /// ���� �ε����� ������ ������ UI�� �ʱ�ȭ�ϰ� Ŭ�� �̺�Ʈ�� ����
    /// </summary>
    public void Init(int idx, TempSaveData data, System.Action<int> onClick)
    {
        _button = GetComponent<Button>();
        if(_button == null)
        {
            Debug.LogError("[SaveSlotUI] button not found");
            return;
        }

        CacheViews();
        name = $"SaveSlot_{idx}";       // ��ü�� ����
        SetClick(_button, () => onClick?.Invoke(idx)); // Ŭ�� �̺�Ʈ ���ε�
        ToggleValidSlot(data);          // �����Ϳ� ���� (noneSlot / saveSlot) ���

    }

    /// <summary>
    /// UI ������Ʈ ĳ��
    /// </summary>
    private void CacheViews()
    {
        Transform root = transform;

        noneSlot        = "noneSlot".FindIn(root);
        img_dataBoxNone = "img_dataBoxNone".FindIn<Image>(noneSlot);
        txt_noneTitle   = "txt_noneTitle".FindIn<TMP_Text>(noneSlot);

        saveSlot        = "saveSlot".FindIn(root);
        img_dataBoxSave = "img_dataBoxSave".FindIn<Image>(saveSlot);
        img_thumb       = "img_thumb".FindIn<Image>(saveSlot);
        txt_saveTitle   = "txt_saveTitle".FindIn<TMP_Text>(saveSlot);
        txt_date        = "txt_date".FindIn<TMP_Text>(saveSlot);
    }

    /// <summary>
    /// �����Ϳ� ���� ��ȿ�� slot�� ���
    /// </summary>
    /// <param name="data"></param>
    private void ToggleValidSlot(TempSaveData data)
    {
        if (data.HasData)
        {
            saveSlot.gameObject.SetActive(true);
            noneSlot.gameObject.SetActive(false);

            img_thumb.sprite = data.ThumbSprite;
            img_thumb.color = Color.white;
            txt_saveTitle.text = data.Title;
            txt_date.text = data.Time.ToString("yyyy/MM/dd HH:mm");
            _button.interactable = true;
        }
        else
        {
            saveSlot.gameObject.SetActive(false);
            noneSlot.gameObject.SetActive(true);
            _button.interactable = false;
        }
    }
}
