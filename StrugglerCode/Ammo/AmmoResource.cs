using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Struggler.StrugglerCode.Ammo;

public static class AmmoResource
{
    public const int DefaultCombatAmmo = 3;
    public const int MaxAmmo = 6;

    private static readonly SpireField<PlayerCombatState, int> PlayerAmmo = new(() => 0);

    public static event Action<PlayerCombatState, int, int>? AmmoChanged;

    public static int GetAmmo(Player player) =>
        player.PlayerCombatState != null ? PlayerAmmo[player.PlayerCombatState] : 0;

    public static bool HasAmmo(Player player, int amount = 1) => GetAmmo(player) >= amount;

    public static void ResetCombatAmmo(Player player, int amount = DefaultCombatAmmo)
    {
        if (player.PlayerCombatState == null) return;
        var capped = Math.Min(MaxAmmo, amount);
        var old = PlayerAmmo[player.PlayerCombatState];
        PlayerAmmo[player.PlayerCombatState] = capped;
        if (old != capped)
            AmmoChanged?.Invoke(player.PlayerCombatState, old, capped);
    }

    public static async Task GainAmmo(int amount, Player player)
    {
        if (player.PlayerCombatState == null) return;
        for (var i = 0; i < amount; i++)
        {
            var oldVal = PlayerAmmo[player.PlayerCombatState];
            var newVal = Math.Min(MaxAmmo, oldVal + 1);
            if (newVal == oldVal) return;
            PlayerAmmo[player.PlayerCombatState] = newVal;
            AmmoChanged?.Invoke(player.PlayerCombatState, oldVal, newVal);
            await Task.CompletedTask;
        }
    }

    public static void SpendAmmo(int amount, Player player)
    {
        if (player.PlayerCombatState == null) return;
        var oldVal = PlayerAmmo[player.PlayerCombatState];
        var newVal = Math.Max(0, oldVal - amount);
        if (newVal == oldVal) return;
        PlayerAmmo[player.PlayerCombatState] = newVal;
        AmmoChanged?.Invoke(player.PlayerCombatState, oldVal, newVal);
    }
}
