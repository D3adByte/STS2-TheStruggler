using MegaCrit.Sts2.Core.Entities.Relics;
using Struggler.StrugglerCode.Ammo;

namespace Struggler.StrugglerCode.Relics;

public sealed class RickertsGift : StrugglerRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override Task BeforeCombatStart()
    {
        AmmoResource.ResetCombatAmmo(Owner, AmmoResource.DefaultCombatAmmo + 2);
        return Task.CompletedTask;
    }
}
