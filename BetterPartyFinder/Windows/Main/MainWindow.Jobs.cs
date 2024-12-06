using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Gui.PartyFinder.Types;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace BetterPartyFinder.Windows.Main;

public partial class MainWindow
{
    private void DrawJobsTab(ConfigurationFilter filter)
    {
        using var tabItem = ImRaii.TabItem("Jobs");
        if (!tabItem.Success)
            return;

        if (ImGui.Button("Add slot"))
        {
            filter.Jobs.Add(0);
            Plugin.Config.Save();
        }

        if (ImGui.Button("Overwrite with current Jobs"))
        {
            filter.Jobs = Util.GetCurrentPartyJobs();
            Plugin.Config.Save();
        }


        var toRemove = new HashSet<int>();
        for (var i = 0; i < filter.Jobs.Count; i++)
        {
            var slot = filter.Jobs[i];

            if (!ImGui.CollapsingHeader($"Slot {i + 1}"))
                continue;

            if (ImGui.Button("Select all"))
            {
                filter.Jobs[i] = Enum.GetValues<JobFlags>().Aggregate(slot, (current, job) => current | job);
                Plugin.Config.Save();
            }

            ImGui.SameLine();

            if (ImGui.Button("Clear"))
            {
                filter.Jobs[i] = 0;
                Plugin.Config.Save();
            }

            ImGui.SameLine();

            if (ImGui.Button("Delete"))
                toRemove.Add(i);

            foreach (var job in Enum.GetValues<JobFlags>())
            {
                var selected = (slot & job) > 0;
                if (!ImGui.Selectable(job.ClassJob(Plugin.DataManager)?.Name.ExtractText() ?? "???", ref selected))
                    continue;

                if (selected)
                    slot |= job;
                else
                    slot &= ~job;

                filter.Jobs[i] = slot;

                Plugin.Config.Save();
            }
        }

        foreach (var idx in toRemove)
            filter.Jobs.RemoveAt(idx);

        if (toRemove.Count > 0)
            Plugin.Config.Save();
    }
}