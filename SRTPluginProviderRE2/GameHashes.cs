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
        private static readonly byte[] re2WW_20200718_1 = new byte[32];

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
