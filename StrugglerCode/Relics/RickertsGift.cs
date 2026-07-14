using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using Struggler.StrugglerCode.Ammo;

namespace Struggler.StrugglerCode.Relics;

public sealed class RickertsGift : StrugglerRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.AmmoOnly;

    public override Task BeforeCombatStart()
    {
        AmmoResource.ResetCombatAmmo(Owner, AmmoResource.DefaultCombatAmmo + 2);
        return Task.CompletedTask;
    }
}
