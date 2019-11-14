using UnityEngine;
using UnityEngine.UI;

sealed public class DigitalNewsPaperController : MonoBehaviour
{
    [SerializeField]
    private Scrollbar verticalBar = null;
    [SerializeField]
    private Image newsImage = null;
    [SerializeField]
    private Text headline = null;
    [SerializeField]
    private Text text1 = null;
    [SerializeField]
    private Text text2 = null;
    private News[] news = null;
    private uint current = 0;

    public void Initialize(News[] news)
    {
        current = 0;
        this.news = news;

        GetNews();
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    private void GetNews()
    {
        verticalBar.value = 1;

        News currentNews = news[current];
        headline.text = currentNews.headline;
        text1.text = currentNews.text1;
        text2.text = currentNews.text2;
        newsImage.sprite = currentNews.icon;
    }

    public void NextNews()
    {
        uint aux = current;

        current = (uint)Mathf.Min(current + 1, news.Length - 1);

        if (aux != current)
            GetNews();
    }

    public void PreviousNews()
    {
        uint aux = current;

        current = (uint)Mathf.Max(current - 1, 0);

        if (aux != current)
            GetNews();
    }

    public void Close()
    {
        GameInstance.HUD.EnableDigitalNewsPaper(false);
    }
}
