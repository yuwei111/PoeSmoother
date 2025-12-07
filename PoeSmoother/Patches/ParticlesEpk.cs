using LibBundle3.Nodes;

namespace PoeSmoother.Patches;

public class ParticlesEpk : IPatch
{
    public string Name => "ParticlesEpk Patch";
    public object Description => "Disables all particle effects in the game.";

    private readonly string[] extensions = {
        ".epk",
    };

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
            // 檔案不存在 → 使用空清單
            exceptionList = new List<string>();
        }
    }

    private bool IsException(string filePath)
    {
        var lower = filePath.Replace("\\", "/").ToLower();
        return exceptionList.Any(ex => lower.Contains(ex));
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
                string fullPath = file.FullName?.Replace("\\", "/").ToLower()
                                  ?? file.Name.ToLower();

                // 命中例外 → 跳過
                if (IsException(fullPath))
                    continue;

                // 若副檔名符合 → 清空
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
            if (d is DirectoryNode dir && dir.Name.Equals("metadata", StringComparison.OrdinalIgnoreCase))
            {
                RecursivePatcher(dir);
            }
        }
    }
}
