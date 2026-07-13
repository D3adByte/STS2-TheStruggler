using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Struggler.StrugglerCode.Ammo;
using Struggler.StrugglerCode.Character;
using Struggler.StrugglerCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

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

    protected void SpendAmmo(int amount = 1)
    {
        if (Owner != null)
            AmmoResource.SpendAmmo(amount, Owner);
    }
}
