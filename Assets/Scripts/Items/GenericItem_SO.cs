using UnityEngine;

[CreateAssetMenu(menuName = "Items/Generic Item")]
public class GenericItem_SO : BaseItem_SO
{
    public override Common.ItemType itemType => Common.ItemType.Generic;
}
