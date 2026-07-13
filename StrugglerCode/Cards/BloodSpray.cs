using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Compatibility;

namespace Struggler.StrugglerCode.Cards;

public sealed class BloodSpray() : GutsCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(3m),
        new PowerVar<StrengthPower>(2m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmdCompatibility.Damage(ctx, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered, this, play);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars.Strength.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Strength.UpgradeValueBy(1m);
}
