﻿using Dalamud.Game.Gui.PartyFinder.Types;
using Lumina.Excel.Sheets;

namespace BetterPartyFinder;

public enum UiCategory
{
    None,
    DutyRoulette,
    Dungeons,
    Guildhests,
    Trials,
    Raids,
    HighEndDuty,
    Pvp,
    QuestBattles,
    Fates,
    TreasureHunt,
    TheHunt,
    GatheringForays,
    DeepDungeons,
    AdventuringForays,
    VCDungeon,
    Chaotic,
}

internal static class UiCategoryExt
{
    internal static string? Name(this UiCategory category)
    {
        var ct = Plugin.DataManager.GetExcelSheet<ContentType>();
        var addon = Plugin.DataManager.GetExcelSheet<Addon>();

        return category switch
        {
            UiCategory.None => addon.GetRow(1_562).Text.ExtractText(), // best guess
            UiCategory.DutyRoulette => ct.GetRow((uint) ContentType2.DutyRoulette).Name.ExtractText(),
            UiCategory.Dungeons => ct.GetRow((uint) ContentType2.Dungeons).Name.ExtractText(),
            UiCategory.Guildhests => ct.GetRow((uint) ContentType2.Guildhests).Name.ExtractText(),
            UiCategory.Trials => ct.GetRow((uint) ContentType2.Trials).Name.ExtractText(),
            UiCategory.Raids => ct.GetRow((uint) ContentType2.Raids).Name.ExtractText(),
            UiCategory.HighEndDuty => addon.GetRow(10_822).Text.ExtractText(), // best guess
            UiCategory.Pvp => ct.GetRow((uint) ContentType2.Pvp).Name.ExtractText(),
            UiCategory.QuestBattles => ct.GetRow((uint) ContentType2.QuestBattles).Name.ExtractText(),
            UiCategory.Fates => ct.GetRow((uint) ContentType2.Fates).Name.ExtractText(),
            UiCategory.TreasureHunt => ct.GetRow((uint) ContentType2.TreasureHunt).Name.ExtractText(),
            UiCategory.TheHunt => addon.GetRow(8_613).Text.ExtractText(),
            UiCategory.GatheringForays => addon.GetRow(2_306).Text.ExtractText(),
            UiCategory.DeepDungeons => ct.GetRow((uint) ContentType2.DeepDungeons).Name.ExtractText(),
            UiCategory.AdventuringForays => addon.GetRow(2_307).Text.ExtractText(),
            UiCategory.VCDungeon => ct.GetRow((uint)ContentType2.VCDungeon).Name.ExtractText(),
            UiCategory.Chaotic => ct.GetRow((uint)ContentType2.Chaotic).Name.ExtractText(),
            _ => null,
        };
    }

    internal static bool ListingMatches(this UiCategory category, IPartyFinderListing listing)
    {
        var cr = Plugin.DataManager.GetExcelSheet<ContentRoulette>();

        var isDuty = listing.Category is DutyCategory.None or DutyCategory.DutyRoulette or DutyCategory.Dungeon
            or DutyCategory.Guildhest or DutyCategory.Trial or DutyCategory.Raid or DutyCategory.HighEndDuty
            or DutyCategory.PvP; // tldr: "high byte is 0"
        var isNormal = listing.DutyType == DutyType.Normal;
        var isOther = listing.DutyType == DutyType.Other;
        var isNormalDuty = isNormal && isDuty;

        Plugin.Log.Verbose($"name {category.Name()}/{listing.Name.TextValue}, isduty {isDuty} {isNormal} {isOther} {isNormalDuty}, cat {listing.Category}, type {listing.DutyType}, raw {listing.RawDuty}");

        var result = category switch
        {
            UiCategory.None => isOther && isDuty && listing.RawDuty == 0,
            UiCategory.DutyRoulette => listing.DutyType == DutyType.Roulette && isDuty && !cr.GetRow(listing.RawDuty).IsPvP,
            UiCategory.Dungeons => isNormalDuty && listing.Duty.Value.ContentType.RowId == (uint) ContentType2.Dungeons,
            UiCategory.Guildhests => isNormalDuty && listing.Duty.Value.ContentType.RowId == (uint) ContentType2.Guildhests,
            UiCategory.Trials => isNormalDuty && !listing.Duty.Value.HighEndDuty && listing.Duty.Value.ContentType.RowId == (uint) ContentType2.Trials,
            UiCategory.Raids => isNormalDuty && !listing.Duty.Value.HighEndDuty && listing.Duty.Value.ContentType.RowId == (uint) ContentType2.Raids,
            UiCategory.HighEndDuty => isNormalDuty && listing.Duty.Value.HighEndDuty,
            UiCategory.Pvp => listing.DutyType == DutyType.Roulette && isDuty && cr.GetRow(listing.RawDuty).IsPvP || isNormalDuty && listing.Duty.Value.ContentType.RowId == (uint) ContentType2.Pvp,
            UiCategory.QuestBattles => isOther && listing.Category == DutyCategory.GoldSaucer,
            UiCategory.Fates => isOther && listing.Category == DutyCategory.Fate,
            UiCategory.TreasureHunt => isOther && listing.Category == DutyCategory.TreasureHunt,
            UiCategory.TheHunt => isOther && listing.Category == DutyCategory.TheHunt,
            UiCategory.GatheringForays => isNormal && listing.Category == DutyCategory.GatheringForay,
            UiCategory.DeepDungeons => isOther && listing.Category == DutyCategory.DeepDungeon,
            UiCategory.AdventuringForays => isNormal && listing.Category == DutyCategory.FieldOperation,
            UiCategory.VCDungeon => isNormal && listing.Duty.Value.ContentType.RowId == (uint) ContentType2.VCDungeon,
            UiCategory.Chaotic => isNormalDuty && listing.Duty.Value.ContentType.RowId == (uint) ContentType2.Chaotic,
            _ => false,
        };

        Plugin.Log.Verbose($"result: {result}");

        return result;
    }

    private enum ContentType2
    {
        DutyRoulette = 1,
        Dungeons = 2,
        Guildhests = 3,
        Trials = 4,
        Raids = 5,
        Pvp = 6,
        QuestBattles = 7,
        Fates = 8,
        TreasureHunt = 9,
        Levequests = 10,
        GrandCompany = 11,
        Companions = 12,
        BeastTribeQuests = 13,
        OverallCompletion = 14,
        PlayerCommendation = 15,
        DisciplesOfTheLand = 16,
        DisciplesOfTheHand = 17,
        RetainerVentures = 18,
        GoldSaucer = 19,
        DeepDungeons = 21,
        WondrousTails = 24,
        CustomDeliveries = 25,
        Eureka = 26,
        UltimateRaids = 28,
        VCDungeon = 30,
        Chaotic = 37,
    }
}