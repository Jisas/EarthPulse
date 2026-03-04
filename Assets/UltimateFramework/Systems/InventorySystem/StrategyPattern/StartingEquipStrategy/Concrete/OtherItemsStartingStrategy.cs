using UltimateFramework.ItemSystem;
using UltimateFramework.Utils;

namespace UltimateFramework.InventorySystem
{
    internal class OtherItemsStartingStrategy : IStartingEquipStrategy
    {
        public void SetStartingEquip(InventoryAndEquipmentComponent inventory, Item item, int equipSocketIndex, int amount, bool equipOnbody)
        {
            item.SetAllValuesToBase();
            inventory.EquipItem(item, equipSocketIndex, amount);
        }
    }
}
