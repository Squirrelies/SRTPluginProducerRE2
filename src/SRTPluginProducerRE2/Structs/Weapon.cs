using System;

namespace SRTPluginProducerRE2.Structs
{
    public struct Weapon : IEquatable<Weapon>
    {
        public WeaponEnumeration WeaponID;
        public AttachmentsFlag Attachments;

        public bool Equals(Weapon other) => (int)this.WeaponID == (int)other.WeaponID && (int)this.Attachments == (int)other.Attachments;
    }
}
