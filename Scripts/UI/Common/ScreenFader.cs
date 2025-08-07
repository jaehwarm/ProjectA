using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면 전체 페이드인/아웃 전용 전역 헬퍼
/// </summary>
public static class ScreenFader
{
    private static CanvasGroup _cg;
    private static Tween _tween;

    public static IEnumerator FadeOutRoutine(float duration)
    {
        var canvas = EnsureCanvas();
        canvas.blocksRaycasts = true;
        _tween?.Kill();
        _tween = canvas.DOFade(1f, duration).SetUpdate(true).SetRecyclable();

        yield return _tween.WaitForCompletion();
    }

    public static IEnumerator FadeInRoutine(float duration)
    {
        var canvas = EnsureCanvas();
        _tween?.Kill();
        _tween = canvas.DOFade(0f, duration).SetUpdate(true).SetRecyclable();

        yield return _tween.WaitForCompletion();
        canvas.blocksRaycasts = false;
    }

    #region Internal Methods
    /// <summary>
    /// CanvasGroup 생성 및 초기화
    /// </summary>
    private static CanvasGroup EnsureCanvas()
    {
        if(_cg != null)
        {
            // 이미 CanvasGroup이 존재하는 경우 즉시 반환
            return _cg;
        }

        // GameObject 생성 및 초기화
        var obj = new GameObject("ScreenFader");
        Object.DontDestroyOnLoad(obj);

        // Canvas 컴포넌트 추가
        var canvas = obj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        // CanvasScaler 컴포넌트 추가
        var scaler = obj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Fade용 Image 컴포넌트 추가
        var img = new GameObject("FadeImage").AddComponent<Image>();
        img.transform.SetParent(canvas.transform, false);
        img.rectTransform.anchorMin = Vector2.zero;
        img.rectTransform.anchorMax = Vector2.one;
        img.color = Color.black;

        // AlphaTransition을 위한 CanvasGroup 컴포넌트 추가
        _cg = img.gameObject.AddComponent<CanvasGroup>();
        _cg.alpha = 0f;
        _cg.blocksRaycasts = false;

        return _cg;
    }
    #endregion
}