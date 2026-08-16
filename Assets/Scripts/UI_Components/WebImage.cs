using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Image))]
public class WebImage : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Init(string url)
    {
        UrlImageHandler.LoadImage(url, RenderImage);
    }

    private void RenderImage(Sprite sprite)
    {
        if (sprite == null)
            return;

        float imageAspect = image.rectTransform.rect.width /
                            image.rectTransform.rect.height;

        float spriteAspect = sprite.rect.width /
                             sprite.rect.height;

        Rect cropRect;

        if (spriteAspect > imageAspect)
        {
            float newWidth = sprite.rect.height * imageAspect;

            cropRect = new Rect(
                sprite.rect.x + (sprite.rect.width - newWidth) / 2f,
                sprite.rect.y,
                newWidth,
                sprite.rect.height
            );
        }
        else
        {
            float newHeight = sprite.rect.width / imageAspect;

            cropRect = new Rect(
                sprite.rect.x,
                sprite.rect.y + (sprite.rect.height - newHeight) / 2f,
                sprite.rect.width,
                newHeight
            );
        }

        Sprite croppedSprite = Sprite.Create(
            sprite.texture,
            cropRect,
            new Vector2(0.5f, 0.5f),
            sprite.pixelsPerUnit
        );

        image.sprite = croppedSprite;
    }
}
