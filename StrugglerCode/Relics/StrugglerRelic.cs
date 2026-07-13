using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Struggler.StrugglerCode.Character;
using Struggler.StrugglerCode.Extensions;

namespace Struggler.StrugglerCode.Relics;

[Pool(typeof(GutsRelicPool))]
public abstract class StrugglerRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}
