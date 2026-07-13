using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Powers;

namespace Struggler.StrugglerCode.Cards;

public sealed class BerserkerArmor() : GutsCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new PowerVar<BerserkPower>(1m) ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ HoverTipFactory.FromPower<BerserkPower>() ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) => await PowerCmd.Apply<BerserkPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["BerserkPower"].BaseValue = 1m; // placeholder innate on upgrade

}
