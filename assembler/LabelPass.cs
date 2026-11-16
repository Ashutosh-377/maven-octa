using System;

public class LabelPass
{
    public LabelTable DiscoverLabels(string source)
    {
        LabelTable labels = new LabelTable();

        string[] lines = source.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
        
        int address = 0;
        foreach(string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (line.Length == 0)
            {
                continue;
            }    

            if (line.EndsWith(":"))
            {
                string labelName = line.Substring(0, line.Length - 1).Trim();
                if (labelName.Length == 0)
                {
                    throw new Exception("Empty label name.");
                }
                if (address > 254)
                {
                    throw new Exception($"Label '{labelName}' is outside addressable memory.");
                }

                labels.AddLabel(labelName, (byte)address);
            }
            else
            {
                address += 2;

                if (address > 256)
                {
                    throw new Exception("Program exceeds 256 bytes of memory");
                }
            }
        }

        return labels;
    }

    public bool IsLabelDefinition(string line)
    {
        line = line.Trim();
        return line.EndsWith(":");
    }
}