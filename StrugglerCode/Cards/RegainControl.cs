using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Powers;

namespace Struggler.StrugglerCode.Cards;

public sealed class RegainControl() : GutsCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{

    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [ new BlockVar(5m, ValueProp.Move) ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.BerserkOnly;
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var berserk = Owner.Creature.GetPower<BerserkPower>();
        if (berserk != null) await PowerCmd.Remove(berserk);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);

}
