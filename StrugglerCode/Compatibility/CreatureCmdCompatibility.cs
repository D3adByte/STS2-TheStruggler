using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Compatibility;

public static class CreatureCmdCompatibility
{
    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext, Creature target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
        => SingleWithDealer(choiceContext, target, amount, props, dealer, cardSource, cardPlay);

    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext, Creature target, decimal amount,
        ValueProp props, CardModel cardSource, CardPlay? cardPlay)
        => SingleCardOnly(choiceContext, target, amount, props, cardSource, cardPlay);

    private delegate Task<IEnumerable<DamageResult>> SingleDealerDel(
        PlayerChoiceContext ctx, Creature target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? card, CardPlay? play);

    private delegate Task<IEnumerable<DamageResult>> SingleCardDel(
        PlayerChoiceContext ctx, Creature target, decimal amount,
        ValueProp props, CardModel card, CardPlay? play);

    private static readonly SingleDealerDel SingleWithDealer = Build<SingleDealerDel>(
        typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal),
        typeof(ValueProp), typeof(Creature), typeof(CardModel));

    private static readonly SingleCardDel SingleCardOnly = Build<SingleCardDel>(
        typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal),
        typeof(ValueProp), typeof(CardModel));

    private static TDelegate Build<TDelegate>(params Type[] baseParams) where TDelegate : Delegate
    {
        var withPlay = baseParams.Append(typeof(CardPlay)).ToArray();
        var method = typeof(CreatureCmd).GetMethod("Damage",
                         BindingFlags.Public | BindingFlags.Static, null, withPlay, null)
                     ?? typeof(CreatureCmd).GetMethod("Damage",
                         BindingFlags.Public | BindingFlags.Static, null, baseParams, null)
                     ?? throw new MissingMethodException($"CreatureCmd.Damage not found");
        bool hasPlay = method.GetParameters().Length == withPlay.Length;
        var lambdaParams = withPlay.Select((t, i) => System.Linq.Expressions.Expression.Parameter(t, $"p{i}")).ToArray();
        var callArgs = hasPlay ? lambdaParams : lambdaParams[..^1];
        var call = System.Linq.Expressions.Expression.Call(method, callArgs.Cast<System.Linq.Expressions.Expression>());
        return System.Linq.Expressions.Expression.Lambda<TDelegate>(call, lambdaParams).Compile();
    }
}
