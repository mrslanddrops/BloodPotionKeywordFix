using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.SkyrimInterfaceAssetType;

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

public static void RunPatch(Inamed nameGetter, IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
{
var vendorItemPotionFormKey = FormKey.Factory("08CDEC:Skyrim.esm");// VendorItemPotion [KYWD:0008CDEC]
   var bloodPotion = ("Blood");
int patchedCount = 0;
  
            foreach (var potionGetter in state.LoadOrder.PriorityOrder.Name().Ingestible().WinningOverrides())
            {
               if (potionGetter.Keywords != null && potionGetter.Keywords.Contains(vendorItemPotionFormKey)) continue;
               if (nameGetter.Name.Contains(bloodPotion)) continue;

                patchedNpcCount++;
              
                var potionToPatch = state.PatchMod.Ingestibless.GetOrAddAsOverride(potionGetter);
                if (potionToPatch.Keywords == null) potionToPatch.Keywords = new Noggog.ExtendedList<IFormLinkGetter<IKeywordGetter>>();

                potionToPatch.Keywords.Add(vendorItemPotionFormKey);
            }

            Console.WriteLine($"Fixed {patchedCount} records");
        }
      }
}
