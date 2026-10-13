using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SaveOptions;

public static class SavePolicies
{
    public static FileInfo? LatestSave(IEnumerable<FileInfo> files) => files
        .OrderByDescending(file => file.LastWriteTimeUtc)
        .ThenBy(file => file.Name, StringComparer.OrdinalIgnoreCase)
        .FirstOrDefault();

    public static string EventSaveName(string label, bool appendLabel)
    {
        const string baseName = "Bad Event";
        if (!appendLabel)
        {
            return baseName;
        }

        StringBuilder result = new();
        bool replacing = false;
        foreach (char character in label)
        {
            // Use the Windows filename rules on every platform.
            if (character < 32 || "<>:\"/\\|?*".IndexOf(character) >= 0)
            {
                if (!replacing && result.Length > 0)
                {
                    result.Append('_');
                }
                replacing = true;
            }
            else
            {
                result.Append(character);
                replacing = false;
            }
            if (result.Length >= 80)
            {
                break;
            }
        }
        string safeLabel = result.ToString().Trim(' ', '.', '_');
        return safeLabel.Length == 0 ? baseName : baseName + "_" + safeLabel;
    }
}
