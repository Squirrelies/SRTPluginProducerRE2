using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SRTPluginProducerRE2.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x24)]

    public struct GameInventoryEntry
    {
        [FieldOffset(0x10)] private readonly int itemID;
        [FieldOffset(0x14)] private readonly int weaponID;
        [FieldOffset(0x18)] private readonly int attachments;
        [FieldOffset(0x20)] private readonly int quantity;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#pragma warning disable IDE1006 // Naming Styles
        public string _DebuggerDisplay
#pragma warning restore IDE1006 // Naming Styles
        {
            get
            {
                if (IsItem)
                    return string.Format("Item {0} Quantity {1}", ItemID, Quantity);
                else if (IsWeapon)
                    return string.Format("Weapon {0} Quantity {1} Attachments {2}", WeaponID, Quantity, Attachments);
                else
                    return string.Format("Empty Slot");
            }
        }

        public ItemEnumeration ItemID => (ItemEnumeration)itemID;
        public WeaponEnumeration WeaponID => (WeaponEnumeration)weaponID;
        public AttachmentsFlag Attachments => (AttachmentsFlag)attachments;
        public int Quantity => quantity;
        public bool IsItem => ItemID != ItemEnumeration.None && WeaponID == WeaponEnumeration.None;
        public bool IsWeapon => ItemID == ItemEnumeration.None && WeaponID != WeaponEnumeration.None;
        public bool IsEmptySlot => !IsItem && !IsWeapon;
    }
}