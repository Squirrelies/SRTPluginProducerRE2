using ProcessMemory;
using SRTPluginProviderRE2.Structures;
using System;
using System.Diagnostics;

namespace SRTPluginProviderRE2
{
    internal class GameMemoryRE2Scanner : IDisposable
    {
        // Variables
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryRE2 gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

        // Pointer Address Variables
        private int pointerAddressIGT;
        private int pointerAddressRank;
        private int pointerAddressPlayerInfo;
        private int pointerAddressEnemies;

        // Pointer Classes
        private IntPtr BaseAddress { get; set; }
        private MultilevelPointer PointerIGT { get; set; }
        private MultilevelPointer PointerRank { get; set; }
        private MultilevelPointer PointerPlayerHP { get; set; }
        private MultilevelPointer PointerPlayerPoison { get; set; }
        private MultilevelPointer[] PointerInventoryEntries { get; set; }
        private MultilevelPointer[] PointerEnemyEntries { get; set; }
        

        internal GameMemoryRE2Scanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryRE2();
            if (process != null)
                Initialize(process);
        }

        internal unsafe void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            if (!SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName)))
                return; // Unknown version.

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid);
            if (ProcessRunning)
            {
                BaseAddress = NativeWrappers.GetProcessBaseAddress(pid, PInvoke.ListModules.LIST_MODULES_64BIT); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.

                // Setup the pointers.
                PointerIGT = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressIGT), 0x2E0, 0x218, 0x610, 0x710, 0x60);
                PointerRank = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressRank));
                PointerPlayerHP = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressPlayerInfo), 0x50, 0x20);
                PointerPlayerPoison = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressPlayerInfo), 0x50, 0x20, 0xF8);

                PointerInventoryEntries = new MultilevelPointer[20];
                for (int i = 0; i < PointerInventoryEntries.Length; ++i)
                    PointerInventoryEntries[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressPlayerInfo), 0x50, 0x98, 0x10, 0x20 + (i * 0x08), 0x18);

                GenerateEnemyEntries();
            }
        }

        private bool SelectPointerAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.RE2_WW_20200718_1:
                    {
                        // pointerAddress
                        pointerAddressIGT = 0x07097EF8;
                        pointerAddressRank = 0x070A7C88;
                        pointerAddressPlayerInfo = 0x070A17E0; // HP, Poison, Inv.
                        pointerAddressEnemies = 0x070960E0;

                        return true;
                    }
            }

            // If we made it this far... rest in pepperonis. We have failed to detect any of the correct versions we support and have no idea what pointer addresses to use. Bail out.
            return false;
        }

        /// <summary>
        /// Dereferences a 4-byte signed integer via the PointerEnemyEntryCount pointer to detect how large the enemy pointer table is and then create the pointer table entries if required.
        /// </summary>
        private unsafe void GenerateEnemyEntries()
        {
            if (PointerEnemyEntries == null) // Enter if the pointer table is null (first run) or the size does not match.
            {
                PointerEnemyEntries = new MultilevelPointer[32]; // Create a new enemy pointer table array with the detected size.
                for (int i = 0; i < PointerEnemyEntries.Length; ++i) // Loop through and create all of the pointers for the table.
                    PointerEnemyEntries[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressEnemies), 0x80 + (i * 0x08), 0x88, 0x18, 0x1A0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdatePointers()
        {
            PointerIGT.UpdatePointers();
            PointerRank.UpdatePointers();
            PointerPlayerHP.UpdatePointers();
            PointerPlayerPoison.UpdatePointers();

            for (int i = 0; i < PointerInventoryEntries.Length; ++i)
                PointerInventoryEntries[i].UpdatePointers();

            GenerateEnemyEntries(); // This has to be here for the next part.
            for (int i = 0; i < PointerEnemyEntries.Length; ++i)
                PointerEnemyEntries[i].UpdatePointers();
        }

        internal unsafe IGameMemoryRE2 Refresh()
        {
            // IGT
            fixed (long* p = &gameMemoryValues._igtRunningTimer)
                PointerIGT.TryDerefLong(0x18, p);
            fixed (long* p = &gameMemoryValues._igtCutsceneTimer)
                PointerIGT.TryDerefLong(0x20, p);
            fixed (long* p = &gameMemoryValues._igtMenuTimer)
                PointerIGT.TryDerefLong(0x28, p);
            fixed (long* p = &gameMemoryValues._igtPausedTimer)
                PointerIGT.TryDerefLong(0x30, p);

            // Player Info
            fixed (int* p = &gameMemoryValues._playerMaxHealth)
                PointerPlayerHP.TryDerefInt(0x54, p);
            fixed (int* p = &gameMemoryValues._playerCurrentHealth)
                PointerPlayerHP.TryDerefInt(0x58, p);
            fixed (byte* p = &gameMemoryValues._playerPoisoned)
                PointerPlayerPoison.TryDerefByte(0x258, p);
            fixed (int* p = &gameMemoryValues._rank)
                PointerRank.TryDerefInt(0x58, p);
            fixed (float* p = &gameMemoryValues._rankScore)
                PointerRank.TryDerefFloat(0x5C, p);

            // Inventory
            if (gameMemoryValues._playerInventory == null)
            {
                gameMemoryValues._playerInventory = new InventoryEntry[20];
                for (int i = 0; i < gameMemoryValues.PlayerInventory.Length; ++i)
                {
                    gameMemoryValues.PlayerInventory[i] = new InventoryEntry() { _slotPosition = -1, _data = new int[5] };
                    Array.Copy(InventoryEntry.EMPTY_INVENTORY_ITEM, 0, gameMemoryValues.PlayerInventory[i]._data, 0, 5);
                }
            }
            for (int i = 0; i < PointerInventoryEntries.Length; ++i)
            {
                try
                {
                    fixed (long* p = &gameMemoryValues.PlayerInventory[i]._invDataOffset)
                        PointerInventoryEntries[i].TryDerefLong(0x10, p);
                    gameMemoryValues.PlayerInventory[i]._invDataOffset -= PointerInventoryEntries[i].Address.ToInt64();

                    fixed (int* p = &gameMemoryValues.PlayerInventory[i]._slotPosition)
                        PointerInventoryEntries[i].TryDerefInt(0x28, p);
                    fixed (int* p = &gameMemoryValues.PlayerInventory[i]._data[0])
                        PointerInventoryEntries[i].TryDerefByteArray((int)gameMemoryValues.PlayerInventory[i]._invDataOffset + 0x10, 20, (byte*)p);
                }
                catch
                {
                    fixed (int* p = &gameMemoryValues.PlayerInventory[i]._slotPosition)
                        PointerInventoryEntries[i].TryDerefInt(0x28, p);
                    gameMemoryValues.PlayerInventory[i]._data = InventoryEntry.EMPTY_INVENTORY_ITEM;
                }
            }

            // Enemy HP
            GenerateEnemyEntries();
            if (gameMemoryValues._enemyHealth == null)
            {
                gameMemoryValues._enemyHealth = new EnemyHP[32];
                for (int i = 0; i < gameMemoryValues._enemyHealth.Length; ++i)
                    gameMemoryValues._enemyHealth[i] = new EnemyHP();
            }
            for (int i = 0; i < gameMemoryValues._enemyHealth.Length; ++i)
            {
                try
                {
                    // Check to see if the pointer is currently valid. It can become invalid when rooms are changed.
                    if (PointerEnemyEntries[i].Address != IntPtr.Zero)
                    {
                        fixed (int* p = &gameMemoryValues.EnemyHealth[i]._maximumHP)
                            PointerEnemyEntries[i].TryDerefInt(0x54, p);
                        fixed (int* p = &gameMemoryValues.EnemyHealth[i]._currentHP)
                            PointerEnemyEntries[i].TryDerefInt(0x58, p);
                    }
                    else
                    {
                        // Clear these values out so stale data isn't left behind when the pointer address is no longer value and nothing valid gets read.
                        // This happens when the game removes pointers from the table (map/room change).
                        gameMemoryValues.EnemyHealth[i]._maximumHP = 0;
                        gameMemoryValues.EnemyHealth[i]._currentHP = 0;
                    }
                }
                catch
                {
                    gameMemoryValues.EnemyHealth[i]._maximumHP = 0;
                    gameMemoryValues.EnemyHealth[i]._currentHP = 0;
                }
            }

            HasScanned = true;
            return gameMemoryValues;
        }

        private int? GetProcessId(Process process) => process?.Id;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (memoryAccess != null)
                        memoryAccess.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~REmake1Memory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
