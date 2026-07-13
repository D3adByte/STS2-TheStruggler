using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Powers;

namespace Struggler.StrugglerCode.Cards;

public sealed class Hatred() : GutsCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new PowerVar<HatredPower>(1m) ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ HoverTipFactory.FromPower<HatredPower>(), HoverTipFactory.FromPower<StrengthPower>() ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) => await PowerCmd.Apply<HatredPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["HatredPower"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["HatredPower"].UpgradeValueBy(1m);

}
