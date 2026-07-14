using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Ammo;
using Struggler.StrugglerCode.Powers;

namespace Struggler.StrugglerCode.Potions;

public sealed class GunpowderFlask : StrugglerPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("Ammo", 3).WithTooltip("STRUGGLER-AMMO")];

    public override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.AmmoOnly;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (Owner == null) return;
        await AmmoResource.GainAmmo(DynamicVars["Ammo"].IntValue, Owner);
    }
}

public sealed class FairyDust : StrugglerPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(10m)];

    public override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.BerserkOnly;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (Owner?.Creature == null) return;
        var berserk = Owner.Creature.GetPower<BerserkPower>();
        if (berserk != null) await PowerCmd.Remove(berserk);
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }
}

public sealed class RageDraft : StrugglerPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(2m),
        new DamageVar(3m, ValueProp.Unblockable | ValueProp.Unpowered),
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (Owner?.Creature == null) return;
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars.Strength.BaseValue, Owner.Creature, null);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.Damage.BaseValue,
            DynamicVars.Damage.Props, Owner.Creature, null);
    }
}
