using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Gui.PartyFinder.Types;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace BetterPartyFinder;

public static class Util
{
    public static string UpperCaseStr(this ReadOnlySeString s, sbyte article = 0)
    {
        if (article == 1)
            return s.ExtractText();

        var sb = new StringBuilder(s.ExtractText());
        var lastSpace = true;
        for (var i = 0; i < sb.Length; ++i)
        {
            if (sb[i] == ' ')
            {
                lastSpace = true;
            }
            else if (lastSpace)
            {
                lastSpace = false;
                sb[i]     = char.ToUpperInvariant(sb[i]);
            }
        }

        return sb.ToString();
    }

    internal static bool ContainsIgnoreCase(this string haystack, string needle)
    {
        return CultureInfo.InvariantCulture.CompareInfo.IndexOf(haystack, needle, CompareOptions.IgnoreCase) >= 0;
    }

    internal static IEnumerable<World> WorldsOnDataCentre(IPlayerCharacter character)
    {
        var dcRow = character.HomeWorld.Value.DataCenter.Value.Region;
        return Sheets.WorldSheet.Where(world => world.IsPublic && world.DataCenter.Value.Region == dcRow);
    }

    /// <summary> Iterate over enumerables with additional index. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IEnumerable<(T Value, int Index)> WithIndex<T>(this IEnumerable<T> list)
        => list.Select((x, i) => (x, i));

    // From: https://stackoverflow.com/a/1415187
    public static string GetDescription(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (name != null)
        {
            var field = type.GetField(name);
            if (field != null)
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    return attr.Description;
        }

        return string.Empty;
    }

    internal static JobFlags GetJobFlagsForClassJob(ClassJob classJob)
    {
        var jobFlags = new JobFlags();
        switch (classJob.Abbreviation.ExtractText())
        {
            case "GLD":
                jobFlags = JobFlags.Gladiator;
                break;
            case "PGL":
                jobFlags = JobFlags.Pugilist;
                break;
            case "MRD":
                jobFlags = JobFlags.Marauder;
                break;
            case "LNC":
                jobFlags = JobFlags.Lancer;
                break;
            case "ARC":
                jobFlags = JobFlags.Archer;
                break;
            case "CNJ":
                jobFlags = JobFlags.Conjurer;
                break;
            case "THM":
                jobFlags = JobFlags.Thaumaturge;
                break;
            case "PLD":
                jobFlags = JobFlags.Paladin;
                break;
            case "MNK":
                jobFlags = JobFlags.Monk;
                break;
            case "WAR":
                jobFlags = JobFlags.Warrior;
                break;
            case "DRG":
                jobFlags = JobFlags.Dragoon;
                break;
            case "BRD":
                jobFlags = JobFlags.Bard;
                break;
            case "WHM":
                jobFlags = JobFlags.WhiteMage;
                break;
            case "BLM":
                jobFlags = JobFlags.BlackMage;
                break;
            case "ACN":
                jobFlags = JobFlags.Arcanist;
                break;
            case "SMN":
                jobFlags = JobFlags.Summoner;
                break;
            case "SCH":
                jobFlags = JobFlags.Scholar;
                break;
            case "ROG":
                jobFlags = JobFlags.Rogue;
                break;
            case "NIN":
                jobFlags = JobFlags.Ninja;
                break;
            case "MCH":
                jobFlags = JobFlags.Machinist;
                break;
            case "DRK":
                jobFlags = JobFlags.DarkKnight;
                break;
            case "AST":
                jobFlags = JobFlags.Astrologian;
                break;
            case "SAM":
                jobFlags = JobFlags.Samurai;
                break;
            case "RDM":
                jobFlags = JobFlags.RedMage;
                break;
            case "BLU":
                jobFlags = JobFlags.BlueMage;
                break;
            case "GNB":
                jobFlags = JobFlags.Gunbreaker;
                break;
            case "DNC":
                jobFlags = JobFlags.Dancer;
                break;
            case "RPR":
                jobFlags = JobFlags.Reaper;
                break;
            case "SGE":
                jobFlags = JobFlags.Sage;
                break;
            case "VPR":
                jobFlags = JobFlags.Viper;
                break;
            case "PCT":
                jobFlags = JobFlags.Pictomancer;
                break;
        }
        return jobFlags;
    }

    internal static List<JobFlags> GetCurrentPartyJobs()
    {
        List<JobFlags> newJobs = new();
        int plen = new();
        if (InfoProxyCrossRealm.IsCrossRealmParty())
        {
            plen = InfoProxyCrossRealm.GetGroupMemberCount(0);
            for (uint i = 0; i < plen; i++)
            {
                unsafe
                {
                    var crossRealmMember = InfoProxyCrossRealm.GetGroupMember(i, 0);
                    var classJob = Plugin.DataManager.GetExcelSheet<ClassJob>().GetRow((uint)crossRealmMember->ClassJobId);
                    newJobs.Add(Util.GetJobFlagsForClassJob(classJob));
                }
            }
        }
        else if (Plugin.PartyList.Length > 0)
        {
            plen = Plugin.PartyList.Length;
            for (var i = 0; i < plen; i++)
            {
                newJobs.Add(Util.GetJobFlagsForClassJob(Plugin.PartyList[i].ClassJob.Value));
            }
        }
        else if (Plugin.PartyList.Length == 0)
        {
            newJobs.Add(Util.GetJobFlagsForClassJob(Plugin.ClientState.LocalPlayer.ClassJob.Value));
        }
        return newJobs;
    }
}