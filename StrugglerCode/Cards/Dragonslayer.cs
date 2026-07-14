using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Powers;

namespace Struggler.StrugglerCode.Cards;

public sealed class Dragonslayer() : GutsCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DragonslayerPower>(1m),
        new DamageVar(2m, ValueProp.Move),
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ HoverTipFactory.FromPower<DragonslayerPower>() ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) => await PowerCmd.Apply<DragonslayerPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);

}
