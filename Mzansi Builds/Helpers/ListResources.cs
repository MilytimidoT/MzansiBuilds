using System;
using System.Linq;
using System.Reflection;
using System.Resources;

public static class ListResources
{
    public static void DumpEntryAssemblyResources()
    {
        var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        Console.WriteLine("Entry assembly: " + asm.FullName);
        foreach (var n in asm.GetManifestResourceNames()) Console.WriteLine("ManifestResource: " + n);

        var gResourceName = asm.GetName().Name + ".g.resources";
        using (var s = asm.GetManifestResourceStream(gResourceName))
        {
            if (s == null) { Console.WriteLine(".g.resources not found: " + gResourceName); return; }
            using (var rr = new ResourceReader(s))
            {
                foreach (System.Collections.DictionaryEntry e in rr)
                {
                    Console.WriteLine("g.resource: " + e.Key);
                }
            }
        }
    }
}