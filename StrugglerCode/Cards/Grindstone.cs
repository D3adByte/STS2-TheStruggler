using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Cards;

public sealed class Grindstone() : GutsCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new CardsVar(1) ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) => await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);

}
