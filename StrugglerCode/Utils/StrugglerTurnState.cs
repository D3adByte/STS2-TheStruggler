using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using Struggler.StrugglerCode.Ammo;

namespace Struggler.StrugglerCode.Utils;

public static class StrugglerTurnState
{
    private static readonly SpireField<PlayerCombatState, int> AttacksThisTurn = new(() => 0);
    private static readonly SpireField<PlayerCombatState, int> AmmoSpentThisTurn = new(() => 0);
    private static readonly SpireField<PlayerCombatState, int> HpLostThisTurn = new(() => 0);
    private static readonly SpireField<PlayerCombatState, bool> PowderKegPending = new(() => false);
    private static readonly SpireField<PlayerCombatState, bool> BloodlustTriggered = new(() => false);

    public static void ResetCombat(Player player)
    {
        if (player.PlayerCombatState == null) return;
        var state = player.PlayerCombatState;
        AttacksThisTurn[state] = 0;
        AmmoSpentThisTurn[state] = 0;
        HpLostThisTurn[state] = 0;
        PowderKegPending[state] = false;
        BloodlustTriggered[state] = false;
    }

    public static void ResetTurn(Player player)
    {
        if (player.PlayerCombatState == null) return;
        var state = player.PlayerCombatState;
        AttacksThisTurn[state] = 0;
        AmmoSpentThisTurn[state] = 0;
        HpLostThisTurn[state] = 0;
        PowderKegPending[state] = false;
    }

    public static int GetAttacksThisTurn(Player player) =>
        player.PlayerCombatState != null ? AttacksThisTurn[player.PlayerCombatState] : 0;

    public static int GetAmmoSpentThisTurn(Player player) =>
        player.PlayerCombatState != null ? AmmoSpentThisTurn[player.PlayerCombatState] : 0;

    public static int GetHpLostThisTurn(Player player) =>
        player.PlayerCombatState != null ? HpLostThisTurn[player.PlayerCombatState] : 0;

    public static void RecordAttack(Player player)
    {
        if (player.PlayerCombatState == null) return;
        AttacksThisTurn[player.PlayerCombatState]++;
    }

    public static void RecordAmmoSpent(Player player, int amount = 1)
    {
        if (player.PlayerCombatState == null) return;
        AmmoSpentThisTurn[player.PlayerCombatState] += amount;
    }

    public static void RecordHpLost(Player player, decimal amount)
    {
        if (player.PlayerCombatState == null || amount <= 0) return;
        HpLostThisTurn[player.PlayerCombatState] += (int)Math.Ceiling(amount);
    }

    public static void ArmPowderKeg(Player player)
    {
        if (player.PlayerCombatState == null) return;
        PowderKegPending[player.PlayerCombatState] = true;
    }

    public static bool ConsumePowderKeg(Player player)
    {
        if (player.PlayerCombatState == null || !PowderKegPending[player.PlayerCombatState]) return false;
        PowderKegPending[player.PlayerCombatState] = false;
        return true;
    }

    public static bool TryTriggerBloodlust(Player player)
    {
        if (player.PlayerCombatState == null || BloodlustTriggered[player.PlayerCombatState]) return false;
        BloodlustTriggered[player.PlayerCombatState] = true;
        return true;
    }

    public static async Task OnAmmoSpent(Player player, int amount)
    {
        RecordAmmoSpent(player, amount);
        if (!ConsumePowderKeg(player)) return;
        await AmmoResource.GainAmmo(2, player);
    }
}
