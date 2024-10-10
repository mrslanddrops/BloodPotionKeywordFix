using System;
using System.Threading.Tasks;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;

using Noggog;

namespace BloodPotionKeywordFix

{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "BloodPotionKeywordFix.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            state.LoadOrder.PriorityOrder.OnlyEnabled().Ingestible().WinningOverrides().ForEach(alch =>
            {
                if (alch.HasKeyword(Keyword.VendorItemPotion) || alch.HasKeyword(Keyword.VendorItemPoison))
                {
                    Console.WriteLine($"Patching {alch.Name}");
                    var nalch = state.PatchMod.Ingestibles.GetOrAddAsOverride(alch);
                    nalch = FormKey.Factory("08CDEC:Dawnguard.esm");// VendorItemPotion [KYWD:0008CDEC]
                }
            });
        }
    }
}
