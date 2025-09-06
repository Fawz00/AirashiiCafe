using UnityEngine;

[CreateAssetMenu(menuName = "Items/Beverage Item")]
public class BeverageItem_SO : BaseItem_SO
{
    public override Common.ItemType itemType => Common.ItemType.Beverage;
}
