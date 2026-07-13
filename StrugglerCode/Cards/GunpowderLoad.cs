using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Ammo;

namespace Struggler.StrugglerCode.Cards;

public sealed class GunpowderLoad() : GutsCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new IntVar("Ammo", 2) ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) => await AmmoResource.GainAmmo(DynamicVars["Ammo"].IntValue, Owner);
    protected override void OnUpgrade() => DynamicVars["Ammo"].UpgradeValueBy(1m);

}
