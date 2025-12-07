using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using LibBundle3.Nodes;

namespace PoeSmoother.Patches;

public class ParticlesEpk : IPatch
{
    public string Name => "ParticlesEpk Patch";
    public object Description => "Disables all particle effects in the game.";

    private readonly string[] extensions = { ".epk" };

    private readonly List<string> exceptionList;

    public ParticlesEpk()
    {
        string fileName = "EpkExceptList.txt";

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

    // ★ 正確：LibBundle3 所有 Node 是 BundleNode
    private string GetFullPath(FileNode file)
    {
        var parts = new List<string>();
        BundleNode? current = file;   // FileNode → BundleNode

        while (current != null)
        {
            if (current is DirectoryNode dir)
                parts.Add(dir.Name);
            else if (current is FileNode f)
                parts.Add(f.Name);

            current = current.Parent; // ★ BundleNode 才有 Parent
        }

        parts.Reverse();
        return string.Join("/", parts).ToLower();
    }

    private bool IsException(string filePath)
    {
        return exceptionList.Any(ex => filePath.Contains(ex));
    }

    private void RecursivePatcher(DirectoryNode dir)
    {
        foreach (var d in dir.Children)
        {
            if (d is DirectoryNode childDir)
            {
                RecursivePatcher(childDir);
            }
            else if (d is FileNode file)
            {
                string fullPath = GetFullPath(file);

                if (IsException(fullPath))
                    continue;

                if (extensions.Any(ext => fullPath.EndsWith(ext)))
                {
                    var record = file.Record;
                    byte[] newBytes = Encoding.Unicode.GetBytes("");

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

    public void Apply(DirectoryNode root)
    {
        foreach (var d in root.Children)
        {
            if (d is DirectoryNode dir &&
                dir.Name.Equals("metadata", StringComparison.OrdinalIgnoreCase))
            {
                RecursivePatcher(dir);
            }
        }
    }
}
