using UnityEngine;

[ExecuteAlways]
public class ContentScaler : MonoBehaviour
{
    private float referenceHeight;
    [SerializeField] RectTransform content;

    RectTransform rt;
    float lastHeight = -1f;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        referenceHeight = rt.rect.height;
    }

    void Update()
    {
        if (rt == null) rt = GetComponent<RectTransform>();
        if (content == null) return;

        float currentHeight = rt.rect.height;

        if (!Mathf.Approximately(currentHeight, lastHeight))
        {
            float scale = currentHeight / referenceHeight;
            content.localScale = new Vector3(scale, scale, 1f);
            lastHeight = currentHeight;
        }
    }
}