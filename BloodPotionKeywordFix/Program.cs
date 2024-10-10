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
var vendorItemPotionFormKey = FormKey.Factory("08CDEC:Skyrim.esm");// VendorItemPotion [KYWD:0008CDEC]
var ConsumeSound = FormKey.Factory("106614:Skyrim.esm");
int patchedCount = 0;
            foreach (var potionGetter in state.LoadOrder.PriorityOrder.Ingestible().WinningOverrides())
 {
                bool useConsumeSound = !potionGetter.ConsumeSound.IsNull || potionGetter.ConsumeSound.FormKey == ConsumeSound;
                bool isNeedToFixMissingKeyword = potionGetter.Keywords == null || potionGetter.Keywords.Count == 0 || !potionGetter.Keywords.Contains(vendorItemPotionFormKey);

                if (!useConsumeSound && !isNeedToFixMissingKeyword) continue;

                patchedCount++;

                var keyToPatch = state.PatchMod.Keys.GetOrAddAsOverride(keyGetter);

                if (isNeedToFixMissingKeyword)
                {
                    if (keyToPatch.Keywords == null) keyToPatch.Keywords = new Noggog.ExtendedList<IFormLinkGetter<IKeywordGetter>>();

                    keyToPatch.Keywords.Add(vendorItemPotionFormKey);
                }
            }

Console.WriteLine($"Fixed {patchedCount} records");
}
}
}
