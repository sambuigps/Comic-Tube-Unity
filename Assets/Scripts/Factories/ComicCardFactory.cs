using UnityEngine;

public static class ComicCardFactory
{
    public static ComicCard[] GetComics(Comic[] comics)
    {
        ComicCard[] res = new ComicCard[comics.Length];
        int i = 0;
        foreach (Comic c in comics)
        {
            var comic = GameObject.Instantiate(SO.prefabs.comicCardPf).GetComponent<ComicCard>();
            comic.Init(c.coverImg, c.title, c.starCnt, c.status);
            res[i] = comic;
            i++;
        }
        return res;
    }
}
