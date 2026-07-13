using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Compatibility;

namespace Struggler.StrugglerCode.Cards;

public sealed class PhantomPain() : GutsCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(5m),
        new CardsVar(1),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmdCompatibility.Damage(ctx, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered, this, play);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}
