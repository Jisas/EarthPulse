using UltimateFramework.StatisticsSystem;
using UltimateFramework.ItemSystem;
using UltimateFramework.Utils;
using UnityEngine;
using System;


namespace UltimateFramework.InventorySystem
{
    public class DualHandEquipStrategy : IEquipWeaponStrategy
    {
        public void Equip(InventoryAndEquipmentComponent inventory, Item item, int socketIndex, bool equipOnBody, bool isSlotSwitching = false, SocketOrientation orientation = SocketOrientation.Right)
        {
            GameObject itemObj_A = null;
            GameObject itemObj_B = null;
            string bodySlot = item.bodySlot;
            string handSlot = item.handSlot;

            if (item.prefab != null)
            {
                string[] bodySockets = bodySlot.Split(',');
                int valueBodySocket = bodySockets.Length - 1 == 0 ? 1 : bodySockets.Length - 1;

                if (socketIndex > valueBodySocket)
                {
                    throw new Exception
                        ($"The number of weapons must match the number of body slots, check the item: {item.name} " +
                        $"in the item database and make sure that the 'BodySlots' field contains more than one slot " +
                        $"name (slot names must be separated by a comma");
                }

                string concreteBodySocket = valueBodySocket == 1 ? bodySockets[0] : bodySockets[socketIndex];

                if (inventory.LastEquippedWeapon == null || inventory.LastEquippedWeapon != item.prefab)
                {
                    Transform socket;

                    if (equipOnBody)
                    {
                        socket = inventory.bodyBone.Find(concreteBodySocket.Trim());
                        itemObj_A = inventory.InstantiateItem(item.prefab, socket);
                    }
                    else
                    {
                        var rightHandsocket = inventory.rightHandBone.Find(handSlot);
                        var leftHandsocket = inventory.leftHandBone.Find(handSlot);

                        itemObj_A = inventory.InstantiateItem(item.prefab, rightHandsocket);
                        itemObj_B = inventory.InstantiateItem(item.prefab, leftHandsocket);

                        inventory.LastEquippedWeapon = itemObj_A;
                        inventory.SwitchEquipmentSlotType(Utils.SocketType.Hand);
                    }
                }

                if (itemObj_A != null && itemObj_B != null)
                {
                    GameObject owner = inventory.gameObject;
                    ItemBehaviour itemBehaviour_A = itemObj_A.GetComponent<WeaponBehaviour>();
                    ItemBehaviour itemBehaviour_B = itemObj_B.GetComponent<WeaponBehaviour>();
                    StatisticsComponent ownerStats = owner.GetComponent<StatisticsComponent>();

                    itemBehaviour_A.Owner = itemBehaviour_A != null ? owner : throw new NullReferenceException($"item behaviour not found on {item.name}");
                    if (itemBehaviour_A != null && itemBehaviour_A.Item.Scaled.Count > 0) itemBehaviour_A.SetUpScaling(ownerStats);

                    itemBehaviour_B.Owner = itemBehaviour_B != null ? owner : throw new NullReferenceException($"item behaviour not found on {item.name}");
                    if (itemBehaviour_B != null && itemBehaviour_B.Item.Scaled.Count > 0) itemBehaviour_B.SetUpScaling(ownerStats);

                    WeaponComponent weaponComponent_A = itemObj_A.GetComponent<WeaponComponent>();
                    WeaponComponent weaponComponent_B = itemObj_B.GetComponent<WeaponComponent>();
                    Transform bodyWeaponSocket = inventory.bodyBone.Find(concreteBodySocket.Trim());
                    Transform rightHandWeaponSocket = inventory.rightHandBone.Find(handSlot);
                    Transform leftHandWeaponSocket = inventory.leftHandBone.Find(handSlot);

                    if (!equipOnBody)
                    {
                        var equipInput = inventory.EntityInputs.FindInputAction("EquipMelee");
                        if (equipInput != null) equipInput.State = true;
                    }

                    inventory.SetupRightWeapon(itemObj_A, bodyWeaponSocket, rightHandWeaponSocket, weaponComponent_A);
                    inventory.SetupLeftWeapon(itemObj_B, bodyWeaponSocket, leftHandWeaponSocket, weaponComponent_B);

                    if (itemBehaviour_A != null) inventory.BaseLocomotionComponent.SwitchLocomotionMap(itemBehaviour_A.itemName, true);
                }
            }
        }
    }
}
