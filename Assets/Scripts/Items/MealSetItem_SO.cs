using UnityEngine;

[CreateAssetMenu(menuName = "Items/Meal Set Item")]
public class MealSetItem_SO : BaseItem_SO
{
    public override Common.ItemType itemType => Common.ItemType.MealSet;
}
