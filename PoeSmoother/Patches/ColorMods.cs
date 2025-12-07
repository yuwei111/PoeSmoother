using LibBundle3.Nodes;
using PoeSmoother.Models;
using System.Text.RegularExpressions;

namespace PoeSmoother.Patches;

public class ColorMods : IPatch
{
    public string Name => "Color Mods Patch";
    public object Description => "Changes colors of mods in the game.";
    public List<ColorModsOption> ColorModsOptions { get; set; } = new()
    {
		// Good Affixes
	    // maps prefix
		new ColorModsOption("map_beyond_rules", "yellow", true),
		new ColorModsOption("map_number_of_rare_packs_+%", "yellow", true),
		// 3.24 maps Uber prefix
		new ColorModsOption("map_monsters_base_block_%", "yellow", true),
        new ColorModsOption("map_monster_add_x_grasping_vines_on_hit", "yellow", true),
        new ColorModsOption("map_monster_all_damage_can_ignite_freeze_shock", "yellow", true),
        new ColorModsOption("map_player_death_mark_on_rare_unique_kill_ms", "yellow", true),
        new ColorModsOption("map_rare_unique_monsters_remove_%_life_mana_es_on_hit", "yellow", true),
		new ColorModsOption("map_monsters_chance_to_impale_%", "yellow", true),
		new ColorModsOption("map_number_of_additional_mines_to_place", "yellow", true),
		// 3.24 maps Uber suffix
        new ColorModsOption("map_player_global_defences_+%", "yellow", true),
        new ColorModsOption("map_petrificiation_statue_ambush", "yellow", true),
        new ColorModsOption("map_monster_maximum_endurance_charges", "yellow", true),
        new ColorModsOption("map_monster_maximum_frenzy_charges", "yellow", true),
        new ColorModsOption("map_monster_maximum_power_charges", "yellow", true),
        new ColorModsOption("map_minion_attack_speed_+%", "yellow", true),
        new ColorModsOption("map_monster_debuff_time_passed_+%", "yellow", true),
        new ColorModsOption("map_rare_monster_fracture_on_death_chance_%", "yellow", true),
		// 3.15 探險 Remnant Mod
		new ColorModsOption("expedition_twinned_elites", "yellow", true),
        // 3.17 Eldritch Altar(異能祭壇): Both
        new ColorModsOption("map_item_drop_quantity_+%", "yellow", true),
        new ColorModsOption("map_item_drop_rarity_+%", "yellow", true),
		new ColorModsOption("%_chance_to_duplicate_dropped_currency", "yellow", true),
        new ColorModsOption("%_chance_to_duplicate_dropped_divination_cards", "yellow", true),
        new ColorModsOption("%_chance_to_duplicate_dropped_scarabs", "yellow", true),
        new ColorModsOption("%_chance_to_duplicate_dropped_uniques", "yellow", true),
		new ColorModsOption("%_chance_to_duplicate_dropped_maps", "yellow", true),
        // 3.17 Eldritch Altar(異能祭壇): The Eater of Worlds
        new ColorModsOption("chance_%_to_drop_additional_divine_orb", "pink", true),
        new ColorModsOption("map_boss_additional_divine_orb_to_drop", "pink", true),
        new ColorModsOption("chance_%_to_drop_additional_exalted_orb", "pink", true),
        new ColorModsOption("map_boss_additional_exalted_orb_to_drop", "pink", true),
        new ColorModsOption("chance_%_to_drop_additional_grand_eldritch_ichor", "pink", true),
		new ColorModsOption("map_boss_additional_grand_eldritch_ichor_to_drop", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_greater_eldritch_ichor", "yellow", true),
        new ColorModsOption("chance_%_to_drop_additional_scarab_ultimatum", "pink", true),
        new ColorModsOption("chance_%_to_drop_additional_scarab_blight", "pink", true),
        new ColorModsOption("chance_%_to_drop_additional_scarab_harvest", "pink", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_breach", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_delirium", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_legion", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_ritual", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_abyss", "yellow", true),		
		new ColorModsOption("chance_%_to_drop_additional_scarab_expedition", "yellow", true),		
		new ColorModsOption("chance_%_to_drop_additional_scarab_betrayal", "yellow", true),		
		new ColorModsOption("chance_%_to_drop_additional_scarab_beasts", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_incursion", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_sulphite", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_settlers", "yellow", true),
        // 3.17 Eldritch Altar(異能祭壇): The Searing Exarch
		new ColorModsOption("map_boss_additional_orb_of_annulment_to_drop", "yellow", true),
        new ColorModsOption("chance_%_to_drop_additional_grand_eldritch_ember", "pink", true),
		new ColorModsOption("map_boss_additional_grand_eldritch_ember_to_drop", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_greater_eldritch_ember", "yellow", true),
        new ColorModsOption("chance_%_to_drop_additional_scarab_strongbox", "pink", true),
        new ColorModsOption("chance_%_to_drop_additional_scarab_domination", "pink", true),
        new ColorModsOption("chance_%_to_drop_additional_scarab_maps", "pink", true),
        new ColorModsOption("chance_%_to_drop_additional_scarab_divination", "pink", true),
        new ColorModsOption("chance_%_to_drop_additional_scarab_harbinger", "pink", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_essence", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_anarchy", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_beyond", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_influence", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_misc", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_torment", "yellow", true),
		new ColorModsOption("chance_%_to_drop_additional_scarab_uniques", "yellow", true),

		// Annoyed Affixes
	    // maps prefix
        new ColorModsOption("map_monsters_reflect_%_physical_damage", "cyan", false),
        new ColorModsOption("map_monsters_reflect_%_elemental_damage", "cyan", true),
        new ColorModsOption("map_monsters_movement_speed_+%", "cyan", true),
        new ColorModsOption("map_players_no_regeneration_including_es", "cyan", true),
		// maps suffix
        new ColorModsOption("map_monsters_cannot_be_leeched_from", "cyan", false),
        new ColorModsOption("map_player_life_and_es_recovery_speed_+%_final", "cyan", true),
		// maps prefix and 3.24 maps Uber suffix
        new ColorModsOption("map_monster_action_speed_cannot_be_reduced_below_base", "cyan", true),
        new ColorModsOption("	map_monsters_movement_speed_cannot_be_reduced_below_base", "cyan", true),
		// 3.24 maps Uber prefix
        new ColorModsOption("map_exarch_traps", "cyan", false),
		new ColorModsOption("map_uber_sawblades_ambush", "cyan", true),
        new ColorModsOption("map_rare_monster_volatile_on_death_%", "cyan", true),
        new ColorModsOption("map_rare_monsters_shaper_touched", "cyan", true),
		// 3.24 maps Uber suffix
		new ColorModsOption("map_players_skill_area_of_effect_+%_final	", "cyan", false),
        new ColorModsOption("map_ground_orion_meteor", "cyan", false),
        new ColorModsOption("map_uber_drowning_orb_ambush", "cyan", false),
        new ColorModsOption("map_player_create_enemy_meteor_daemon_on_flask_use_%_chance", "cyan", true),
        new ColorModsOption("map_player_action_speed_+%_per_recent_skill_use", "cyan", true),
        new ColorModsOption("map_packs_have_uber_tentacle_fiends", "cyan", true),
		// strongboxes
		new ColorModsOption("chest_display_ice_nova", "cyan", true),
		// 3.15 探險
        new ColorModsOption("cannot_have_life_leeched_from", "cyan", false),
        new ColorModsOption("cannot_have_mana_leeched_from", "cyan", false),
        new ColorModsOption("all_damage_can_freeze", "cyan", false),
        new ColorModsOption("apply_petrification_for_X_seconds_on_hit", "cyan", false),
        new ColorModsOption("base_cold_immunity", "cyan", true),
        new ColorModsOption("base_fire_immunity", "cyan", false),
        new ColorModsOption("base_lightning_immunity", "cyan", false),
        new ColorModsOption("chaos_immunity", "cyan", false),
        new ColorModsOption("physical_immunity", "cyan", false),
		// 3.17 Eldritch Altar(異能祭壇): The Eater of Worlds
        new ColorModsOption("random_projectile_direction", "cyan", false),
		// 3.17 Eldritch Altar(異能祭壇): The Searing Exarch
        new ColorModsOption("drain_x_flask_charges_over_time_on_hit_for_6_seconds", "cyan", false),
		new ColorModsOption("life_mana_es_recovery_rate_+%_per_endurance_charge", "cyan", true),
		new ColorModsOption("create_enemy_meteor_daemon_on_flask_use_%_chance", "cyan", true),

		// Dangerous Affixes
	    // maps prefix
        new ColorModsOption("map_monsters_damage_+%", "red", true),
		new ColorModsOption("map_monsters_%_physical_damage_to_add_as_fire", "red", true),
		new ColorModsOption("map_monsters_%_physical_damage_to_add_as_cold", "red", true),
		new ColorModsOption("map_monsters_%_physical_damage_to_add_as_lightning", "red", true),
		new ColorModsOption("map_monsters_%_physical_damage_to_add_as_chaos", "red", true),
		// maps suffix
        new ColorModsOption("map_additional_player_maximum_resistances_%", "red", true),
		new ColorModsOption("map_monsters_critical_strike_multiplier_+", "red", true),
		// strongboxes
        new ColorModsOption("chest_display_explodes_corpses", "red", true),
        new ColorModsOption("chest_display_freeze", "red", true),
		new ColorModsOption("chest_display_explosion", "red", false),
        new ColorModsOption("chest_spawn_rogue_exiles", "yellow", false),
		// 3.17 Eldritch Altar(異能祭壇): The Eater of Worlds
        new ColorModsOption("global_defences_+%_per_frenzy_charge", "red", false),
		// 3.17 Eldritch Altar(異能祭壇): The Searing Exarch
		new ColorModsOption("chaos_damage_per_minute_while_affected_by_flask", "red", true),
		// 3.24 maps Uber suffix
		new ColorModsOption("map_monsters_physical_damage_%_to_add_as_random_element", "red", true),
		new ColorModsOption("map_monsters_penetrate_elemental_resistances_%", "red", true),

		// Less Damage Affixes
	    // maps prefix		
        new ColorModsOption("map_monsters_maximum_life_%_to_add_to_maximum_energy_shield", "green", true),
        new ColorModsOption("map_monsters_spell_suppression_chance_%", "green", true),
        new ColorModsOption("map_monsters_life_+%", "green", true),
        new ColorModsOption("map_monsters_additional_elemental_resistance", "green", true),
        new ColorModsOption("map_boss_is_possessed", "green", true),
		// maps suffix
        new ColorModsOption("map_player_non_curse_aura_effect_+%", "green", false),
        new ColorModsOption("map_player_cannot_expose", "green", false),
        new ColorModsOption("map_monsters_base_self_critical_strike_multiplier_-%", "green", true),
        new ColorModsOption("map_monsters_avoid_elemental_ailments_%", "green", true),
		// 3.24 Uber Map Prefix
        new ColorModsOption("map_uber_map_player_damage_cycle", "green", true),
		// 3.24 Uber Map Prefix
        new ColorModsOption("map_unique_monsters_have_X_shrine_effects", "green", true),
        // 3.15 探險
        new ColorModsOption("never_take_critical_strike", "green", true),
        new ColorModsOption("avoid_damage_%", "green", true),
        new ColorModsOption("base_damage_taken_+%", "green", true),
		// 3.17 Eldritch Altar(異能祭壇): The Eater of Worlds
        new ColorModsOption("critical_strike_multiplier_+_per_power_charge", "green", true),

        // maps and more
        new ColorModsOption("map_monsters_cannot_be_stunned", "red", false),
        new ColorModsOption("map_monsters_are_hexproof", "red", false),
        new ColorModsOption("map_player_cooldown_speed_+%_final", "cyan", false),
        new ColorModsOption("map_uber_map_additional_synthesis_boss", "red", false),
        new ColorModsOption("map_supporter_maven_follower", "red", false),
    };

    private readonly Dictionary<string, string> _color_conversions = new()
    {
        { "red", "MapTierHigh" },
        { "green", "quest" },
        { "blue", "divination" },
		{ "cyan", "ItemRelic_BlueGlow" },
        { "yellow", "nemesismod" },
        { "pink", "archnemesismodchaospurple" },
    };
    private enum ReadState
    {
        WritingData,
        ReadingData,
        ReadingDescription,
        ReadingToDescription
    }

    private readonly string[] _extensions = {
        ".txt",
    };

    private void RecursivePatcher(DirectoryNode dir)
    {
        foreach (var d in dir.Children)
        {
            if (d is DirectoryNode childDir)
            {
                RecursivePatcher(childDir);
            }
            else if (d is FileNode file)
            {

                if (Array.Exists(_extensions, ext => file.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    var record = file.Record;
                    var bytes = record.Read();
                    string data = System.Text.Encoding.Unicode.GetString(bytes.ToArray());
                    var lines = data.Split("\r\n").ToList();

                    ReadState state = ReadState.ReadingToDescription;

                    string? currentAnnotation = null;
                    bool? currentIsEnabled = null;

                    int linesToWrite = 0;

                    for (int i = 0; i < lines.Count; i++)
                    {
                        string line = lines[i];

                        if (line.StartsWith("description"))
                        {
                            state = ReadState.ReadingDescription;
                            continue;
                        }

                        if (state == ReadState.ReadingToDescription) continue;

                        // Read the description on the next line.
                        if (state == ReadState.ReadingDescription)
                        {
                            string[] description = line.Split(' ');
                            if (description.Length < 2)
                            {
                                state = ReadState.ReadingToDescription;
                                continue;
                            }

                            string modType = description[1];

                            if (ColorModsOptions.FirstOrDefault(x => x.Name == modType) is ColorModsOption option
                                && _color_conversions.TryGetValue(option.Color.ToLower(), out string? annotation))
                            {
                                currentAnnotation = annotation;
                                currentIsEnabled = option.IsEnabled;
                                state = ReadState.ReadingData;
                            }
                            else
                            {
                                currentAnnotation = null;
                                state = ReadState.ReadingToDescription;
                            }

                            continue;
                        }

                        if (state == ReadState.ReadingData)
                        {
                            // Replace tabs in line with nothing.
                            string firstNumber = line.Replace("\t", "").Split(' ')[0];

                            // May be a "lang" value. If the value is a number write those lines.
                            if (int.TryParse(firstNumber, out int value))
                            {
                                state = ReadState.WritingData;
                                linesToWrite = value;
                                continue;
                            }
                            ;
                        }

                        if (state == ReadState.WritingData)
                        {
                            if (line.Contains('<')) // Already annotated.
                            {
                                if (currentIsEnabled == false)
                                {
                                    // Remove annotation.
                                    // <.*?>{{value}} -> "value".
                                    string pattern = "<.*?>{{(.*?)}}";
                                    string replacement = Regex.Replace(line, pattern, new MatchEvaluator(match =>
                                    {
                                        return $"{match.Groups[1].Value}";
                                    }));
                                    lines[i] = replacement;
                                }
                                else
                                {
                                    // Replace text between brackets with new annotation.
                                    string pattern = "<.*?>";
                                    string replacement = Regex.Replace(line, pattern, new MatchEvaluator(match =>
                                    {
                                        return $"<{currentAnnotation}>";
                                    }));
                                    lines[i] = replacement;
                                }
                            }
                            else
                            {
                                // Surround the value with the annotation.
                                // "value" -> "<annotation>{{value}}".

                                string pattern = "\".*?\"";
                                string replacement = Regex.Replace(line, pattern, new MatchEvaluator(match =>
                                {
                                    return $"\"<{currentAnnotation}>{{{{{match.Value.Replace("\"", "")}}}}}\"";
                                }));
                                lines[i] = replacement;
                            }

                            linesToWrite--;
                            if (linesToWrite == 0)
                            {
                                state = ReadState.ReadingData;
                                continue;
                            }
                        }
                    }

                    var newData = string.Join("\r\n", lines);
                    var newBytes = System.Text.Encoding.Unicode.GetBytes(newData);
                    record.Write(newBytes);
                }
            }
        }
    }

    public void Apply(DirectoryNode root)
    {
        // go to metadata/statdescriptions/
        foreach (var d in root.Children)
        {
            if (d is DirectoryNode dir && dir.Name == "metadata")
            {
                foreach (var dir1 in dir.Children)
                {
                    if (dir1 is DirectoryNode subDir && subDir.Name == "statdescriptions")
                    {
                        RecursivePatcher(subDir);
                    }
                }
            }
        }
    }
}
