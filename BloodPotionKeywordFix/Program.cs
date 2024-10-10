using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;

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
            var vendorItemPotionFormKey = FormKey.Factory("08CDEC:Dawnguard.esm");// VendorItemPotion [KYWD:0008CDEC]
            int patchedCount = 0;
            foreach (var scroolGetter in state.LoadOrder.PriorityOrder.Potion().WinningOverrides())
            {
                if (potionGetter.Keywords != null && potionGetter.Keywords.Contains(vendorItemPotionFormKey)) continue;

                patchedCount++;

                var potionToPatch = state.PatchMod.Potions.GetOrAddAsOverride(potionGetter);
                if (potionToPatch.Keywords == null) potionToPatch.Keywords = new Noggog.ExtendedList<IFormLinkGetter<IKeywordGetter>>();

                potionToPatch.Keywords.Add(vendorItemPotionFormKey);
            }

            Console.WriteLine($"Fixed {patchedCount} records");
        }
    }
}
