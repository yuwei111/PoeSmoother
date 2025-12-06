using LibBundle3.Nodes;

namespace PoeSmoother.Patches;

public class AtlasFog : IPatch
{
    public string Name => "Atlas Fog Patch";
    public object Description => "Removes fog from the Atlas.";

    private string ReplaceArrayProperty(string data, string propertyName)
    {
        string searchPattern = $"\"{propertyName}\":";
        int index = data.IndexOf(searchPattern);
        
        if (index < 0) return data;
        
        int bracketStart = data.IndexOf('[', index);
        if (bracketStart < 0) return data;
        
        int bracketCount = 1;
        int i = bracketStart + 1;
        
        while (i < data.Length && bracketCount > 0)
        {
            if (data[i] == '[') bracketCount++;
            else if (data[i] == ']') bracketCount--;
            i++;
        }
        
        if (bracketCount == 0)
        {
            int commaIndex = data.IndexOf(',', i - 1);
            if (commaIndex > 0 && commaIndex < i + 5)
            {
                data = data.Remove(index, commaIndex - index + 1);
                data = data.Insert(index, $"\"{propertyName}\": [],");
            }
        }
        
        return data;
    }

    public void Apply(DirectoryNode root)
    {
        // go to metadata/materials/environment/worldmap/worldmap_fogofwar.fxgraph
        foreach (var d in root.Children)
        {
            if (d is DirectoryNode dir && dir.Name == "metadata")
            {
                foreach (var d1 in dir.Children)
                {
                    if (d1 is DirectoryNode subDir && subDir.Name == "materials")
                    {
                        foreach (var d2 in subDir.Children)
                        {
                            if (d2 is DirectoryNode subDir2 && subDir2.Name == "environment")
                            {
                                foreach (var d3 in subDir2.Children)
                                {
                                    if (d3 is DirectoryNode subDir3 && subDir3.Name == "worldmap")
                                    {
                                        foreach (var d4 in subDir3.Children)
                                        {
                                            if (d4 is FileNode file && file.Name == "worldmap_fogofwar.fxgraph")
                                            {
                                                var record = file.Record;
                                                var bytes = record.Read();
                                                string data = System.Text.Encoding.Unicode.GetString(bytes.ToArray());

                                                data = ReplaceArrayProperty(data, "nodes");
                                                data = ReplaceArrayProperty(data, "links");

                                                var newBytes = System.Text.Encoding.Unicode.GetBytes(data);
                                                record.Write(newBytes);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
