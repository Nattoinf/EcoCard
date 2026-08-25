using UnityEngine;
using UnityEngine.UI;
using System.Collections;     

public class GaugeController : MonoBehaviour
{
    [SerializeField] private Image fillImage; // Inspector で Gauge_Fill の Image を入れる
    [SerializeField] private float current = 1f; // 現在値（0〜1）
    
    void Start()
    {
        if(fillImage == null) Debug.LogWarning("fillImage がアサインされていません。");
        SetValue(current);
    }

    public void SetValue(float value)
    {
        current = Mathf.Clamp01(value);
        if (fillImage != null) fillImage.fillAmount = current;
    }

    // アニメーション付きで変化させたい時
    public IEnumerator AnimateTo(float target, float duration)
    {
        float start = current;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(start, target, t / duration);
            SetValue(v);
            yield return null;
        }
        SetValue(target);
    }
}
