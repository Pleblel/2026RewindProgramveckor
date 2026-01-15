using UnityEngine;
using UnityEngine.UI;

public class CardButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Text titleText;
    [SerializeField] private Text descText;

    private UpgradeId id;
    private CardRewardManager mgr;

    public void Setup(CardRewardManager manager, UpgradeCardDefinition def)
    {
        mgr = manager;
        id = def.id;

        if (titleText != null) titleText.text = def.title;
        if (descText != null) descText.text = def.description;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => mgr.PickCard(id));
    }
}
