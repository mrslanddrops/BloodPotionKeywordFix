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
var BloodPotion = FormKey.Factory("018EF4:Dawnguard.esm");  // DLC1BloodPotionEffect "Blood Ingestion" [MGEF:02018EF4]
int patchedCount = 0;
            foreach (var potionGetter in state.LoadOrder.PriorityOrder.Ingestible().WinningOverrides())
 {
                bool isBloodPotion = potionGetter.MagicEffect.FormKey == FormKey.Null || !potionGetter.MagicEffect.IsNull;
                bool isNeedToFixMissingKeyword = potionGetter.Keywords == null || potionGetter.Keywords.Count == 0 || !potionGetter.Keywords.Contains(vendorItemPotionFormKey);

                if (!isBloodPotion & !isNeedToFixMissingKeyword) continue;

                patchedCount++;

                var potionToPatch = state.PatchMod.Ingestibles.GetOrAddAsOverride(potionGetter);

                if (isNeedToFixMissingKeyword)
                {
                    if (potionToPatch.Keywords == null) potionToPatch.Keywords = new Noggog.ExtendedList<IFormLinkGetter<IKeywordGetter>>();

                    potionToPatch.Keywords.Add(vendorItemPotionFormKey);
                }
            }

Console.WriteLine($"Fixed {patchedCount} records");
}
}
}
