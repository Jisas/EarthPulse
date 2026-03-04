using UltimateFramework.ItemSystem;

namespace UltimateFramework.InventorySystem
{
    public class DualHandStartingEquipStrategy : IStartingEquipStrategy
    {
        public void SetStartingEquip(InventoryAndEquipmentComponent inventory, Item item, int equipSocketIndex, int amount = 1, bool equipOnbody = true)
        {
            inventory.EquipItem(item, equipSocketIndex, equipOnBody: equipOnbody);
        }
    }
}
