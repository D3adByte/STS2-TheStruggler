using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Cards;

public sealed class BandageWrap() : GutsCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new HealVar(8m) ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [ CardKeyword.Exhaust ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) => await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    protected override void OnUpgrade() => DynamicVars.Heal.UpgradeValueBy(4m);

}
