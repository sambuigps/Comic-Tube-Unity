using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Image))]
public class WebImage : MonoBehaviour
{
    private Image image;

    public void Init(string url)
    {
        UrlImageHandler.LoadImage(url, RenderImage);
    }

    private void RenderImage(Sprite sprite)
    {
        if ( sprite != null )
        {
            image.sprite = sprite;
        }
    }
}
