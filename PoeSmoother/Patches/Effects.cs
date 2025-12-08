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

	// 讀取例外不修改檔案
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

    // 檔案路徑處理，不依賴 Parent，靠遞迴時傳入的 path 組完整路徑
    private string BuildPath(string parentPath, string name)
    {
        if (string.IsNullOrEmpty(parentPath))
            return name.ToLower();

        return (parentPath + "/" + name).ToLower();
    }

    // 判斷例外
    private bool IsException(string fullPath)
    {
        string path = fullPath.ToLower();
        return exceptionList.Any(ex => path.Contains(ex));
    }

    // 清空函式內容，保留 {}
    private string ClearFunctionContent(string data, string functionName)
    {
        var sb = new StringBuilder(data);
        int index = 0;

        while (index < sb.Length)
        {
            int funcIndex = sb.ToString().IndexOf(functionName, index, StringComparison.Ordinal);
            if (funcIndex < 0)
                break;

            int braceStart = sb.ToString().IndexOf('{', funcIndex + functionName.Length);
            if (braceStart < 0)
            {
                index = funcIndex + functionName.Length;
                continue;
            }

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
                sb.Remove(braceStart + 1, i - braceStart - 2);
                index = i;
            }
            else
            {
                break;
            }
        }

        return sb.ToString();
    }

    // 遞迴處理 metadata
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

                // ★ 先檢查例外，符合就跳過
                if (IsException(fullPath))
                    continue;

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

    // 套用到 metadata
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
