using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Home : UI_BaseClass
{
    [SerializeField] TMP_Text title;
    [SerializeField] Transform trendingContent;
    [SerializeField] Transform latestContent;
    [SerializeField] Transform randomContent;
    [SerializeField] Button refreshButton;

    public override void Init()
    {
        base.Init();
        title.SetText(SO.env.app_name);
        DestroyPrev();
        FetchComics();
    }

    private void OnEnable()
    {
        refreshButton.onClick.AddListener(FetchComics);
    }

    private void OnDisable()
    {
        refreshButton.onClick.RemoveAllListeners();
    }

    private void DestroyPrev()
    {
        DestroyChildren(trendingContent);
        DestroyChildren(latestContent);
        DestroyChildren(randomContent);
    }

    private void DestroyChildren(Transform t)
    {
        foreach(Transform child in t)
        {
            if (child == t) continue;
            Destroy(child.gameObject);
        }
    }

    private void FetchComics()
    {
        RecommendationHandler.Fetch(OnFetched);
    }

    private void OnFetched(ApiResponse<RecommendationResponse> res)
    {
        if (!res.success)
        {
            Debug.LogWarning(res.message);
            return;
        }

        DestroyPrev();

        var trendings = res.data.popular_comics;
        var latests = res.data.latest_comics;
        var randoms = res.data.random_comics;

        var trendingCards = ComicCardFactory.GetComics(trendings);
        var latestCards = ComicCardFactory.GetComics(latests);
        var randomCards = ComicCardFactory.GetComics(randoms);

        PopulateComics(trendingContent, trendingCards);
        PopulateComics(latestContent, latestCards);
        PopulateComics(randomContent, randomCards);
    }

    private void PopulateComics(Transform parent, ComicCard[] comics)
    {
        foreach (var comic in comics)
        {
            comic.transform.SetParent(parent);
        }
    }
}
