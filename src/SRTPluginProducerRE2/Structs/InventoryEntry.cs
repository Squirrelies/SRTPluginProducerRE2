using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SRTPluginProducerRE2.Structs
{
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    public struct InventoryEntry
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#pragma warning disable IDE1006 // Naming Styles
        public string _DebuggerDisplay
#pragma warning restore IDE1006 // Naming Styles
        {
            get
            {
                if (IsItem)
                    return string.Format("[#{0}] Item {1} Quantity {2}", SlotPosition, ItemID, Quantity);
                else if (IsWeapon)
                    return string.Format("[#{0}] Weapon {1} Quantity {2} Attachments {3}", SlotPosition, WeaponID, Quantity, Attachments);
                else
                    return string.Format("[#{0}] Empty Slot", SlotPosition);
            }
        }

        public int? SlotPosition { get; set; }
        public ItemEnumeration? ItemID { get; set; }
        public WeaponEnumeration? WeaponID { get; set; }
        public AttachmentsFlag? Attachments { get; set; }
        public int? Quantity { get; set; }
        public bool IsItem => ItemID != ItemEnumeration.None && (WeaponID == WeaponEnumeration.None || WeaponID == 0);
        public bool IsWeapon => ItemID == ItemEnumeration.None && WeaponID != WeaponEnumeration.None && WeaponID != 0;
        public bool IsEmptySlot => !IsItem && !IsWeapon;
    }
}
