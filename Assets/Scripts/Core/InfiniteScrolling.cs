using UnityEngine;
using UnityEngine.UI;

public class InfiniteScrolling : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform viewportTransform;
    public RectTransform contentPanelTransform;
    public HorizontalLayoutGroup horizontalLayoutGroup;

    public RectTransform[] itemList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int itemsToAdd = Mathf.CeilToInt(viewportTransform.rect.width / (itemList[0].rect.width + horizontalLayoutGroup.spacing));

        for (int i = 0; i < itemsToAdd; i++)
        {
            RectTransform rectTransform = Instantiate(itemList[i % itemList.Length], contentPanelTransform);
            rectTransform.SetAsLastSibling();
        }
        
        for (int i = 0; i < itemsToAdd; i++)
        {
            int num = itemList.Length - i - 1;
            while (num < 0)
            {
                num += itemList.Length;
            }
            RectTransform rectTransform = Instantiate(itemList[num], contentPanelTransform);
            rectTransform.SetAsFirstSibling();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
