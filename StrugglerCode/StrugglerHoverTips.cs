using MegaCrit.Sts2.Core.HoverTips;
using Struggler.StrugglerCode.Powers;

namespace Struggler.StrugglerCode;

public static class StrugglerHoverTips
{
    public static IHoverTip Ammo => HoverTipFactory.Static(StrugglerStaticTips.Ammo);

    public static IHoverTip Struggle => HoverTipFactory.FromPower<StrugglePower>();

    public static IHoverTip Berserk => HoverTipFactory.FromPower<BerserkPower>();

    public static IHoverTip EliteHunter => HoverTipFactory.FromPower<EliteHunterPower>();

    public static IEnumerable<IHoverTip> AmmoOnly => [Ammo];

    public static IEnumerable<IHoverTip> StruggleOnly => [Struggle];

    public static IEnumerable<IHoverTip> BerserkOnly => [Berserk];

    public static IEnumerable<IHoverTip> EliteHunterOnly => [EliteHunter];

    public static IEnumerable<IHoverTip> AmmoAndStruggle => [Ammo, Struggle];
}
