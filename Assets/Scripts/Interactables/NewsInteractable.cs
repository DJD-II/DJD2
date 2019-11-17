using System.Collections.Generic;
using System.Linq;
using UnityEngine;

sealed public class NewsInteractable : Interactable
{
    [SerializeField]
    private NewsID[] news = null;

    protected override void OnInteract(PlayerController controller)
    {
        News[] currentNews = GetNews();

        foreach (News n in currentNews)
            if (n.headline.ToLower().Equals("scientist killed at his home."))
            {
                GameInstance.GameState.
                    EventController.Add(Event.GetOutOfShowcase);
                GameInstance.GameState.
                    QuestController.Add(QuestUtility.Get("Get Out!"));
                break;
            }

        if (currentNews != null)
            GameInstance.HUD.EnableDigitalNewsPaper(true, currentNews);
    }

    private News[] GetNews()
    {
        List<NewsID> reversedNews = news.ToList();
        reversedNews.Reverse();
        foreach (NewsID id in reversedNews)
            if (id.Fullfills())
                return id.News;

        return null;
    }
}
