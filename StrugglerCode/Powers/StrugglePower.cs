using MegaCrit.Sts2.Core.Entities.Powers;

namespace Struggler.StrugglerCode.Powers;

public sealed class StrugglePower : StrugglerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
