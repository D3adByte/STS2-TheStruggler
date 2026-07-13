using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Struggler.StrugglerCode.Powers;

namespace Struggler.StrugglerCode.Utils;

public static class StruggleHelper
{
    private const int Threshold = 3;

    public static async Task AddStruggle(
        PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        CardModel? source)
    {
        if (amount <= 0) return;

        await PowerCmd.Apply<StrugglePower>(
            new ThrowingPlayerChoiceContext(),
            player.Creature,
            amount,
            player.Creature,
            source);

        var struggle = player.Creature.GetPower<StrugglePower>();
        while (struggle != null && struggle.Amount >= Threshold)
        {
            await PowerCmd.Apply<StrengthPower>(
                new ThrowingPlayerChoiceContext(),
                player.Creature,
                1m,
                player.Creature,
                source);
            for (var i = 0; i < Threshold; i++)
                await PowerCmd.Decrement(struggle);
            struggle = player.Creature.GetPower<StrugglePower>();
        }
    }
}
