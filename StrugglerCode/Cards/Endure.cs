using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Utils;

namespace Struggler.StrugglerCode.Cards;

public sealed class Endure() : GutsCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{

    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [ new BlockVar(6m, ValueProp.Move), new PowerVar<StrugglePower>(2m) ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.StruggleOnly;
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await StruggleHelper.AddStruggle(ctx, Owner, DynamicVars["StrugglePower"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);

}
