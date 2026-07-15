using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Powers;
using Struggler.StrugglerCode.Utils;

namespace Struggler.StrugglerCode.Cards;

public sealed class BerserkerArmor() : GutsCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new PowerVar<BerserkPower>(1m) ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.BerserkOnly;
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await PowerCmd.Apply<BerserkPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this);
        if (Owner.Creature.GetPower<BloodlustPower>() != null && StrugglerTurnState.TryTriggerBloodlust(Owner))
        {
            var cards = Owner.Creature.GetPower<BloodlustPower>()!.DynamicVars.Cards.BaseValue;
            await CardPileCmd.Draw(ctx, cards, Owner);
        }
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);

}
