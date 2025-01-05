using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Gui.PartyFinder.Types;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Lumina.Excel.Sheets;

namespace BetterPartyFinder;

public static class Util
{
    internal static uint MaxItemLevel { get; private set; }

    internal static void CalculateMaxItemLevel()
    {
        if (MaxItemLevel > 0)
            return;

        var max = Plugin.DataManager.GetExcelSheet<Item>()
            .Where(item => item.EquipSlotCategory.Value.Body != 0)
            .Select(item => item.LevelItem.Value.RowId)
            .Max();

        MaxItemLevel = max;
    }

    internal static bool ContainsIgnoreCase(this string haystack, string needle)
    {
        return CultureInfo.InvariantCulture.CompareInfo.IndexOf(haystack, needle, CompareOptions.IgnoreCase) >= 0;
    }

    internal static IEnumerable<World> WorldsOnDataCentre(IPlayerCharacter character)
    {
        var dcRow = character.HomeWorld.Value.DataCenter.RowId;
        return Plugin.DataManager.GetExcelSheet<World>().Where(world => world.IsPublic && world.DataCenter.RowId == dcRow);
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