using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Powers;
using Struggler.StrugglerCode.Utils;

namespace Struggler.StrugglerCode.Relics;

public sealed class GodotsSteel : StrugglerRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(2m, ValueProp.Move)];

    public override async Task BeforeCombatStart()
    {
        Flash();
        await PowerCmd.Apply<GodotsSteelPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            1m, Owner.Creature, null);
    }
}

public sealed class PucksDust : StrugglerRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public static bool ConsumeBerserkProtection(Player player)
    {
        var relic = player.Relics.OfType<PucksDust>().FirstOrDefault();
        if (relic == null || relic._used) return false;
        relic._used = true;
        relic.Flash();
        return true;
    }

    private bool _used;

    public override Task BeforeCombatStart()
    {
        _used = false;
        return Task.CompletedTask;
    }
}

public sealed class HawksFeather : StrugglerRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(5m, ValueProp.Unblockable | ValueProp.Unpowered)];

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        Flash();
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars.Damage.BaseValue, DynamicVars.Damage.Props, Owner.Creature, null);
    }
}

public sealed class WornCloak : StrugglerRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrugglePower>(1m)];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        if (Owner.Creature.GetPowerAmount<StrugglePower>() <= 0) return;
        Flash();
        await StruggleHelper.AddStruggle(choiceContext, Owner, DynamicVars["StrugglePower"].IntValue, null);
    }
}

public sealed class SkullKnightsBlessing : StrugglerRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.RoomType is not (RoomType.Elite or RoomType.Boss)) return;
        Flash();
        await PowerCmd.Apply<EliteHunterPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            1m, Owner.Creature, null);
    }
}

public sealed class BerserkerHelm : StrugglerRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(3m),
        new IntVar("HpThreshold", 15),
    ];

    private bool _used;

    public override Task BeforeCombatStart()
    {
        _used = false;
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner.Creature || _used || Owner.Creature.MaxHp <= 0) return;
        var pct = Owner.Creature.CurrentHp * 100 / Owner.Creature.MaxHp;
        if (pct > DynamicVars["HpThreshold"].IntValue) return;
        _used = true;
        Flash();
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars.Strength.BaseValue, Owner.Creature, null);
    }
}
