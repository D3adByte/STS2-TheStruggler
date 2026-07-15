using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Struggler.StrugglerCode.Ammo;
using Struggler.StrugglerCode.Character;
using Struggler.StrugglerCode.Extensions;
using Struggler.StrugglerCode.Utils;

namespace Struggler.StrugglerCode.Cards;

[Pool(typeof(GutsCardPool))]
public abstract class GutsCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected bool HasAmmo(int amount = 1) =>
        Owner != null && AmmoResource.HasAmmo(Owner, amount);

    protected async Task SpendAmmo(int amount = 1)
    {
        if (Owner != null)
            await AmmoResource.SpendAmmo(amount, Owner);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (Type == CardType.Attack && Owner != null)
            StrugglerTurnState.RecordAttack(Owner);
        return Task.CompletedTask;
    }
}
