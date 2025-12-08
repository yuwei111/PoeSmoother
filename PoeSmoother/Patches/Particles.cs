using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using LibBundle3.Nodes;

namespace PoeSmoother.Patches;

public class Particles : IPatch
{
    public string Name => "Particles Patch";
    public object Description => "Disables all particle effects for pet&trl extension in the game.";

    private readonly string[] extensions = {
        ".pet",
        ".trl",
    };
	private readonly List<string> exceptionList;

	// 讀取例外不修改檔案
    public Particles()
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

    private bool IsException(string fullPath)
    {
        return exceptionList.Any(ex => fullPath.Contains(ex));
    }

	// 字串處理，不處理在例外清單的檔案
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

                if (IsException(fullPath))
                    continue;

                if (extensions.Any(ext => fullPath.EndsWith(ext)))
                {
                    var record = file.Record;
                    byte[] newBytes = Encoding.Unicode.GetBytes("0");

                    if (!newBytes.AsSpan().StartsWith(Encoding.Unicode.GetPreamble()))
                    {
                        newBytes = Encoding.Unicode.GetPreamble()
                            .Concat(newBytes)
                            .ToArray();
                    }

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
                // ★ 起始路徑為空
                RecursivePatcher(dir, "");
            }
        }
    }
}
