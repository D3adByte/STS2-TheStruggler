using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using Struggler.StrugglerCode.Cards;
using Struggler.StrugglerCode.Extensions;
using Struggler.StrugglerCode.Relics;

namespace Struggler.StrugglerCode.Character;

public class Guts : PlaceholderCharacterModel
{
    public const string CharacterId = "Struggler";

    public static readonly Color Color = StsColors.red;

    public override string PlaceholderID => "ironclad";
    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 88;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Cleave>(),
        ModelDb.Card<CannonShot>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BrandOfSacrifice>(),
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<GutsCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<GutsRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<GutsPotionPool>();

    public override string CustomIconTexturePath => "character_icon_guts.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_guts.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_guts_locked.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => "res://Struggler/scenes/character_select/guts_background.tscn";
    public override string CustomMapMarkerPath => "map_marker_guts.png".CharacterUiPath();
    public override string CustomVisualPath => "res://Struggler/scenes/creature_visuals/guts.tscn";
    public override string CustomRestSiteAnimPath => "res://Struggler/scenes/creature_visuals/guts_rest.tscn";
    public override float DeathAnimTime => 1.2f;
}
