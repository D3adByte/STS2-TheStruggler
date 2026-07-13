using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using Struggler.StrugglerCode.Character;

namespace Struggler.StrugglerCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "Struggler";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(typeof(MainFile).Assembly);
        Harmony harmony = new(ModId);
        harmony.PatchAll();
        CustomCharacterUtils.TryOrderCustomCharacters([typeof(Guts)]);
    }
}
