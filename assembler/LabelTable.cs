using System;
using System.Collections.Generic;

public class LabelTable
{
    private readonly Dictionary<string, byte> labels = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase);

    public void AddLabel(string name, byte address)
    {
        if (labels.ContainsKey(name))
        {
            throw new Exception($"Duplicate label: {name}");
        }
        labels.Add(name, address);
    }

    public byte GetAddress(string name)
    {
        if (!labels.TryGetValue(name, out byte address))
        {
            throw new Exception($"Unknown label: {name}");
        }
        return address;
    }

    public bool Contains(string name)
    {
        return labels.ContainsKey(name);
    }
}