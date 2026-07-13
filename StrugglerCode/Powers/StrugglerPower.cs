using BaseLib.Abstracts;
using BaseLib.Extensions;
using Struggler.StrugglerCode.Compatibility;
using Struggler.StrugglerCode.Extensions;

namespace Struggler.StrugglerCode.Powers;

public abstract class StrugglerPower : CustomPowerModelCompatibility
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
