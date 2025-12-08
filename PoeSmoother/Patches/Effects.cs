using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using LibBundle3.Nodes;

namespace PoeSmoother.Patches;

public class Effects : IPatch
{
    public string Name => "Effects Patch (Experimental)";
    public object Description => "Disables all effects for aoc extension in the game.";

    private readonly string[] extensions = { ".aoc" };

    private readonly string[] _functions = {
        "ParticleEffects",
        "TrailsEffects",
        "DecalEvents",
        "ScreenShake",
        "Lights",
        "WindEvents",
        "SoundEvents",
    };

    private readonly List<string> exceptionList;

    public Effects()
    {
        string fileName = "ParticlesExceptList.txt";

        if (File.Exists(fileName))
        {
            exceptionList = File.ReadAllLines(fileName)
                .Select(l => l.Trim().ToLower())
                .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("//"))
                .ToList();
        }
        else
        {
            exceptionList = new List<string>();
        }
    }

    // === 路徑處理，與 Particles.cs 完全一致 ===
    private string BuildPath(string parentPath, string name)
    {
        if (string.IsNullOrEmpty(parentPath))
            return name.ToLower();

        return (parentPath + "/" + name).ToLower();
    }

    private bool IsException(string fullPath)
    {
        return exceptionList.Any(ex => fullPath.Contains(ex));
    }

    // 高效清空 function 內容，保留空括號
    private string ClearFunctionContent(string data, string functionName)
    {
        var sb = new StringBuilder(data);
        int index = 0;

        while (index < sb.Length)
        {
            // 找 functionName
            int funcIndex = sb.ToString().IndexOf(functionName, index, StringComparison.Ordinal);
            if (funcIndex < 0)
                break;

            // 確認後面有 {
            int braceStart = sb.ToString().IndexOf('{', funcIndex + functionName.Length);
            if (braceStart < 0)
            {
                index = funcIndex + functionName.Length;
                continue;
            }

            // 找閉合大括號
            int braceCount = 1;
            int i = braceStart + 1;
            while (i < sb.Length && braceCount > 0)
            {
                if (sb[i] == '{') braceCount++;
                else if (sb[i] == '}') braceCount--;
                i++;
            }

            if (braceCount == 0)
            {
                // 保留 functionName 和 { }，清空中間內容
                sb.Remove(braceStart + 1, i - braceStart - 2); // -2 保留最後的 }
                index = i; // 繼續搜尋下一個 function
            }
            else
            {
                break;
            }
        }

        return sb.ToString();
    }

    // === 套用例外清單與全 metadata 走訪 ===
    private void RecursivePatcher(DirectoryNode dir, string currentPath)
    {
        string dirPath = BuildPath(currentPath, dir.Name);

        foreach (var node in dir.Children)
        {
            if (node is DirectoryNode childDir)
            {
                RecursivePatcher(childDir, dirPath);
            }
            else if (node is FileNode file)
            {
                string fullPath = BuildPath(dirPath, file.Name);

                // 在例外清單 → 跳過
                if (IsException(fullPath))
                    continue;

                // 處理 .aoc 檔案
                if (extensions.Any(ext => fullPath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    var record = file.Record;
                    var bytes = record.Read();
                    string data = Encoding.Unicode.GetString(bytes.ToArray());

                    foreach (var func in _functions)
                    {
                        data = ClearFunctionContent(data, func);
                    }

                    var newBytes = Encoding.Unicode.GetBytes(data);
                    record.Write(newBytes);
                }
            }
        }
    }

	// 封包內的應用路徑，metadata下的全部檔案
    public void Apply(DirectoryNode root)
    {
        foreach (var child in root.Children)
        {
            if (child is DirectoryNode dir &&
                dir.Name.Equals("metadata", StringComparison.OrdinalIgnoreCase))
            {
                RecursivePatcher(dir, "");
            }
        }
    }
}
