using UnityEngine;

[CreateAssetMenu(menuName = "Items/Meal Item")]
public class MealItem_SO : BaseItem_SO
{
    public override Common.ItemType itemType => Common.ItemType.Meal;
}
