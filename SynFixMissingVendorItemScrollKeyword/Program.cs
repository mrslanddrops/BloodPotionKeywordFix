using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;

namespace SynFixMissingVendorItemScrollKeyword
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynFixMissingVendorItemScrollKeyword.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var vendorItemScrollFormKey = FormKey.Factory("0A0E57:Skyrim.esm");// VendorItemScroll [KYWD:000A0E57]

            int patchedCount = 0;
            foreach (var scroolGetter in state.LoadOrder.PriorityOrder.Scroll().WinningOverrides())
            {
                if (scroolGetter.Keywords != null && scroolGetter.Keywords.Contains(vendorItemScrollFormKey)) continue;

                patchedCount++;

                var scrollToPatch = state.PatchMod.Scrolls.GetOrAddAsOverride(scroolGetter);
                if (scrollToPatch.Keywords == null) scrollToPatch.Keywords = new Noggog.ExtendedList<IFormLinkGetter<IKeywordGetter>>();

                scrollToPatch.Keywords.Add(vendorItemScrollFormKey);
            }

            Console.WriteLine($"Fixed {patchedCount} records");
        }
    }
}
