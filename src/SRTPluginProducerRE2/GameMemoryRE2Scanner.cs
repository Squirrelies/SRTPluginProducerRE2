using ProcessMemory;
using SRTPluginProducerRE2.Structs;
using SRTPluginProducerRE2.Structs.GameStructs;
using System;
using System.Diagnostics;

namespace SRTPluginProducerRE2
{
    internal class GameMemoryRE2Scanner : IDisposable
    {
        // Private Constants and Statics
        private const int MAX_ENTITES = 32;
        private const int MAX_ITEMS = 20;
        private static readonly InventoryEntry emptyInventorySlot = new InventoryEntry();
        private static readonly EnemyHP emptyEnemyHP = new EnemyHP();

        // Private Fields
        private ProcessMemoryHandler? memoryAccess;
        private GameMemoryRE2 gameMemoryValues;
        private int pointerAddressIGT;
        private int pointerAddressRank;
        private int pointerAddressPlayerInfo;
        private int pointerAddressEnemies;

        // Public Properties
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

        // Private Properties
        private IntPtr BaseAddress { get; set; }
        private MultilevelPointer? PointerIGT { get; set; }
        private MultilevelPointer? PointerRank { get; set; }
        private MultilevelPointer? PointerCharacter { get; set; }
        private MultilevelPointer? PointerPlayerHP { get; set; }
        private MultilevelPointer? PointerPlayerPoison { get; set; }
        private MultilevelPointer? PointerInventoryCount { get; set; }
        private MultilevelPointer?[]? PointerInventoryEntries { get; set; }
        private MultilevelPointer?[]? PointerInventorySlots { get; set; }
        private MultilevelPointer?[]? PointerEnemyEntries { get; set; }

        internal GameMemoryRE2Scanner(Process? process = null)
        {
            gameMemoryValues = new GameMemoryRE2();
            if (process != null)
                Initialize(process);
        }

        internal unsafe void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            if (!SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule?.FileName)))
                return; // Unknown version.

            int? pid = GetProcessId(process);

            if (pid.HasValue)
            {
                memoryAccess = new ProcessMemoryHandler(pid.Value);
                if (ProcessRunning)
                {
                    BaseAddress = NativeWrappers.GetProcessBaseAddress(pid.Value, PInvoke.ListModules.LIST_MODULES_64BIT); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.

                    // Setup the pointers.
                    PointerIGT = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressIGT), 0x60);
                    PointerRank = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressRank));
                    PointerCharacter = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressPlayerInfo), 0x50, 0x88);
                    PointerPlayerHP = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressPlayerInfo), 0x50, 0x20);
                    PointerPlayerPoison = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressPlayerInfo), 0x50, 0x88);

                    PointerInventoryCount = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressPlayerInfo), 0x50);
                    PointerInventoryEntries = new MultilevelPointer[MAX_ITEMS];
                    PointerInventorySlots = new MultilevelPointer[MAX_ITEMS];
                    gameMemoryValues.playerInventory = new InventoryEntry[MAX_ITEMS];
                    for (int i = 0; i < MAX_ITEMS; ++i)
                    {
                        PointerInventoryEntries[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressPlayerInfo), 0x50, 0x98, 0x10, 0x20 + (i * 0x08), 0x18, 0x10);
                        PointerInventorySlots[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressPlayerInfo), 0x50, 0x98, 0x10, 0x20 + (i * 0x08), 0x18);
                        gameMemoryValues.playerInventory[i] = emptyInventorySlot;
                    }

                    gameMemoryValues.enemyHealth = new EnemyHP[MAX_ENTITES];
                    for (int i = 0; i < MAX_ENTITES; ++i)
                        gameMemoryValues.enemyHealth[i] = emptyEnemyHP;

                    GenerateEnemyEntries();
                }
            }
        }

        private bool SelectPointerAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.RE2_WW_20220613_1:
                    {
                        // pointerAddress
                        pointerAddressIGT = 0x091689E0;
                        pointerAddressRank = 0x0913F000;
                        pointerAddressPlayerInfo = 0x09160F30; // HP, Poison, Inv.
                        pointerAddressEnemies = 0x0913DC70;
                        return true;
                    }

                case GameVersion.RE2_WW_20211217_1:
                    {
                        // pointerAddress
                        pointerAddressIGT = 0x0709D250;
                        pointerAddressRank = 0x070A6AB0;
                        pointerAddressPlayerInfo = 0x070A0958; // HP, Poison, Inv.
                        pointerAddressEnemies = 0x07095248;
                        return true;
                    }

                case GameVersion.RE2_WW_20210201_1:
                case GameVersion.RE2_CEROZ_20210201_1:
                    {
                        // pointerAddress
                        pointerAddressIGT = 0x0709D240;
                        pointerAddressRank = 0x070A6AA0;
                        pointerAddressPlayerInfo = 0x070A0948; // HP, Poison, Inv.
                        pointerAddressEnemies = 0x07095238;
                        return true;
                    }

                case GameVersion.RE2_WW_20200718_1:
                    {
                        // pointerAddress
                        pointerAddressIGT = 0x07097EF8;
                        pointerAddressRank = 0x070A7C88;
                        pointerAddressPlayerInfo = 0x070A17E0; // HP, Poison, Inv.
                        pointerAddressEnemies = 0x070960E0;
                        return true;
                    }
                default:
                    {
                        // pointerAddress
                        pointerAddressIGT = 0x0709D240;
                        pointerAddressRank = 0x070A6AA0;
                        pointerAddressPlayerInfo = 0x070A0948; // HP, Poison, Inv.
                        pointerAddressEnemies = 0x07095238;
                        return true;
                    }
            }
        }

        /// <summary>
        /// Dereferences a 4-byte signed integer via the PointerEnemyEntryCount pointer to detect how large the enemy pointer table is and then create the pointer table entries if required.
        /// </summary>
        private unsafe void GenerateEnemyEntries()
        {
            if (PointerEnemyEntries is null) // Enter if the pointer table is null (first run) or the size does not match.
            {
                PointerEnemyEntries = new MultilevelPointer?[MAX_ENTITES]; // Create a new enemy pointer table array with the detected size.
                for (int i = 0; i < MAX_ENTITES; ++i) // Loop through and create all of the pointers for the table.
                    PointerEnemyEntries[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressEnemies), 0x80 + (i * 0x08), 0x88, 0x18, 0x1A0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdatePointers()
        {
            PointerCharacter?.UpdatePointers();
            PointerIGT?.UpdatePointers();
            PointerRank?.UpdatePointers();
            PointerPlayerHP?.UpdatePointers();
            PointerPlayerPoison?.UpdatePointers();

            PointerInventoryCount?.UpdatePointers();
            for (int i = 0; i < MAX_ITEMS; ++i)
            {
                PointerInventoryEntries?[i]?.UpdatePointers();
                PointerInventorySlots?[i]?.UpdatePointers();
            }

            GenerateEnemyEntries(); // This has to be here for the next part.
            for (int i = 0; i < MAX_ENTITES; ++i)
                PointerEnemyEntries?[i]?.UpdatePointers();
        }

        internal unsafe IGameMemoryRE2 Refresh()
        {
            // IGT
            if (PointerIGT is not null)
                gameMemoryValues.timer = PointerIGT.Deref<GameTimer>(0x18);

            // Player Info
            if (PointerCharacter is not null)
                gameMemoryValues.playerCharacter = PointerCharacter.DerefInt(0x54);
            if (PointerPlayerHP is not null)
                gameMemoryValues.player = PointerPlayerHP.Deref<GamePlayer>(0x54);
            if (PointerPlayerPoison is not null)
                gameMemoryValues.isPoisoned = PointerPlayerPoison.DerefByte(0x258);

            if (PointerRank is not null)
                gameMemoryValues.rankManager = PointerRank.Deref<GameRankManager>(0x58);

            // Inventory
            if (PointerInventoryCount is not null && PointerInventoryEntries is not null)
            {
                gameMemoryValues.playerInventoryCount = PointerInventoryCount?.DerefInt(0x90);
                for (int i = 0; i < MAX_ITEMS; ++i)
                {
                    if (gameMemoryValues.PlayerInventory?[i] is not null)
                    {
                        GameInventoryEntry? entry = PointerInventoryEntries?[i]?.Deref<GameInventoryEntry>(0x0);
                        gameMemoryValues.PlayerInventory[i].SlotPosition = PointerInventorySlots?[i]?.DerefInt(0x28);
                        gameMemoryValues.PlayerInventory[i].ItemID = entry?.ItemID;
                        gameMemoryValues.PlayerInventory[i].WeaponID = entry?.WeaponID;
                        gameMemoryValues.PlayerInventory[i].Attachments = entry?.Attachments;
                        gameMemoryValues.PlayerInventory[i].Quantity = entry?.Quantity;
                    }
                }
            }

            // Enemy HP
            GenerateEnemyEntries();
            if (gameMemoryValues.EnemyHealth is not null)
            {
                for (int i = 0; i < MAX_ENTITES; ++i)
                {
                    try
                    {
                        // Check to see if the pointer is currently valid. It can become invalid when rooms are changed.
                        if (PointerEnemyEntries?[i]?.Address != IntPtr.Zero)
                        {
                            GamePlayer? enemy = PointerEnemyEntries?[i]?.Deref<GamePlayer>(0x54);
                            gameMemoryValues.EnemyHealth[i].maximumHP = enemy?.MaxHP;
                            gameMemoryValues.EnemyHealth[i].currentHP = enemy?.CurrentHP;
                        }
                        else
                        {
                            // Clear these values out so stale data isn't left behind when the pointer address is no longer value and nothing valid gets read.
                            // This happens when the game removes pointers from the table (map/room change).
                            gameMemoryValues.EnemyHealth[i].maximumHP = 0;
                            gameMemoryValues.EnemyHealth[i].currentHP = 0;
                        }
                    }
                    catch
                    {
                        gameMemoryValues.EnemyHealth[i].maximumHP = 0;
                        gameMemoryValues.EnemyHealth[i].currentHP = 0;
                    }
                }
            }

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
