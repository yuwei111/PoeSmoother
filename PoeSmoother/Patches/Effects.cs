using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using LibBundle3.Nodes;

namespace PoeSmoother.Patches;

public class Effects : IPatch
{
    public string Name => "Effects Patch (Experimental)";
    public object Description => "Disables all effects for aoc extension in the game.";

    private readonly string[] extensions = {
        ".aoc",
    };

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

    // === 改良版，避免整個檔案被刪光 ===
    private string RemoveFunctionBlock(string data, string functionName)
    {
        // 尋找「獨立一行」的 functionName
        var pattern = $@"(?m)^\s*{Regex.Escape(functionName)}\s*\{{";

        while (true)
        {
            var match = Regex.Match(data, pattern);
            if (!match.Success)
                break;

            int funcIndex = match.Index;
            int braceStart = data.IndexOf('{', funcIndex);
            if (braceStart < 0) break;

            int braceCount = 1;
            int i = braceStart + 1;

            while (i < data.Length && braceCount > 0)
            {
                if (data[i] == '{') braceCount++;
                else if (data[i] == '}') braceCount--;
                i++;
            }

            if (braceCount == 0)
            {
                data = data.Remove(funcIndex, i - funcIndex);
            }
            else
            {
                break;
            }
        }

        return data;
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
                        data = RemoveFunctionBlock(data, func);
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
