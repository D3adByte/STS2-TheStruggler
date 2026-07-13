using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using Struggler.StrugglerCode.Character;
using Struggler.StrugglerCode.Extensions;

namespace Struggler.StrugglerCode.Potions;

[Pool(typeof(GutsPotionPool))]
public abstract class StrugglerPotion : CustomPotionModel
{
    public override string? CustomPackedImagePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();
            return ResourceLoader.Exists(path) ? path : null;
        }
    }
}
