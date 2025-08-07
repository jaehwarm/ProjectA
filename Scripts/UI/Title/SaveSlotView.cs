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

    private Button      _button;            // 저장 슬롯 버튼

    /// <summary>
    /// 슬롯 인덱스와 데이터 정보로 UI를 초기화하고 클릭 이벤트를 설정
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
        name = $"SaveSlot_{idx}";       // 객체명 지정
        SetClick(_button, () => onClick?.Invoke(idx)); // 클릭 이벤트 바인딩
        ToggleValidSlot(data);          // 데이터에 따라 (noneSlot / saveSlot) 토글

    }

    /// <summary>
    /// UI 컴포넌트 캐싱
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
    /// 데이터에 따라 유효한 slot을 토글
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
