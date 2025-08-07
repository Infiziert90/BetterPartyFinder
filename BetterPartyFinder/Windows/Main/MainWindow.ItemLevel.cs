using Dalamud.Bindings.ImGui;

namespace BetterPartyFinder.Windows.Main;

public partial class MainWindow
{
    private void DrawItemLevelTab(ConfigurationFilter filter)
    {
        var hugePfs = filter.AllowHugeItemLevel;
        if (ImGui.Checkbox("Show PFs above maximum item level", ref hugePfs))
        {
            filter.AllowHugeItemLevel = hugePfs;
            Plugin.Config.Save();
        }

        var width = ImGui.GetContentRegionAvail().X / 3;
        var minLevel = (int?)filter.MinItemLevel ?? 0;
        ImGui.TextUnformatted("Minimum/Maximum item level (0 to disable)");
        ImGui.SetNextItemWidth(width);
        if (ImGui.InputInt("###min-ilvl", ref minLevel))
        {
            filter.MinItemLevel = minLevel == 0 ? null : (uint)minLevel;
            Plugin.Config.Save();
        }

        ImGui.SameLine();

        var maxLevel = (int?)filter.MaxItemLevel ?? 0;
        ImGui.SetNextItemWidth(width);
        if (ImGui.InputInt("###max-ilvl", ref maxLevel))
        {
            filter.MaxItemLevel = maxLevel == 0 ? null : (uint)maxLevel;
            Plugin.Config.Save();
        }
    }
}