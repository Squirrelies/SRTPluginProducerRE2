using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SRTPluginProviderRE2
{
    /// <summary>
    /// SHA256 hashes for the RE2/BIO2 REmake game executables.
    /// 
    /// Resident Evil 2 (WW): https://steamdb.info/app/883710/ / https://steamdb.info/depot/883711/
    /// Biohazard 2 (CERO Z): https://steamdb.info/app/895950/ / https://steamdb.info/depot/895951/
    /// </summary>
    public static class GameHashes
    {
        private static readonly byte[] re2WW_20200718_1 = new byte[32] { 0x25, 0xED, 0x0A, 0xE8, 0xEB, 0xBE, 0x1E, 0x4D, 0xA7, 0x04, 0x56, 0x75, 0xF4, 0x14, 0x6A, 0xBD, 0x0A, 0x43, 0x9B, 0xA7, 0xAC, 0x57, 0x2C, 0xFB, 0xEA, 0xAC, 0x99, 0xCB, 0xC6, 0xCD, 0xB3, 0x78 };

        public static GameVersion DetectVersion(string filePath)
        {
            byte[] checksum;
            using (SHA256 hashFunc = SHA256.Create())
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                checksum = hashFunc.ComputeHash(fs);

            if (checksum.SequenceEqual(re2WW_20200718_1))
                return GameVersion.RE2_WW_20200718_1;
            else
                return GameVersion.Unknown;
        }
    }
}
