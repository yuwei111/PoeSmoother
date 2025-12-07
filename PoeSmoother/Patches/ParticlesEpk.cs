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

    // ★ 組合完整路徑（解決 FullName 不存在問題）
    private string GetFullPath(FileNode file)
    {
        var parts = new List<string>();
        var current = file as Node;

        while (current != null)
        {
            if (current is DirectoryNode dir)
                parts.Add(dir.Name);
            else if (current is FileNode f)
                parts.Add(f.Name);

            current = current.Parent;
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
                string fullPath = GetFullPath(file);  // ★ 改用新的方法

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
