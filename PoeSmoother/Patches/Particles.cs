using System;
using System.IO;
using System.Reflection;
using LibBundle3.Nodes;
using LibGGPK3.Records;

namespace PoeSmoother.Patches;

public class Particles : IPatch
{
    public string Name => "Particles Patch";
    public object Description => "Disables all particle effects in the game.";

    private readonly string[] extensions = {
        ".pet",
        ".trl",
    };

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

                if (Array.Exists(extensions, ext => file.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    var record = file.Record;
                    var bytes = record.Read();
                    string data = System.Text.Encoding.Unicode.GetString(bytes.ToArray());

                    List<string> lines = data.Split("\r\n").ToList();
                    if (lines.Count == 0) continue;

                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("version", StringComparison.OrdinalIgnoreCase)) continue;
                        lines[i] = "0";
                        break;
                    }

                    string newData = string.Join("\r\n", lines);
                    var newBytes = System.Text.Encoding.Unicode.GetBytes(newData);
                    record.Write(newBytes);
                }
            }
        }
    }

    public void Apply(DirectoryNode root)
    {
        // go to metadata/particles/
        foreach (var d in root.Children)
        {
            if (d is DirectoryNode dir && dir.Name == "metadata")
            {
                foreach (var d2 in dir.Children)
                {
                    if (d2 is DirectoryNode subDir && subDir.Name == "particles")
                    {
                        RecursivePatcher(subDir);
                    }
                }
            }
        }
    }
}