using TMPro;
using UnityEngine;

public class ComicCard : MonoBehaviour
{
    [SerializeField] WebImage coverImage;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text noOfStars;
    [SerializeField] TMP_Text status;

    public void Init(string coverUrl, string title, int noOfStars, string status)
    {
        coverImage.Init(coverUrl);
        this.title.SetText(title);
        this.noOfStars.SetText(noOfStars.ToString());
        this.status.SetText(status);
    }
}
