using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace BetterPartyFinder.Windows.Config;

public partial class ConfigWindow
{
    private void General()
    {
        using var tabItem = ImRaii.TabItem("General");
        if (!tabItem.Success)
            return;

        var openWithPf = Plugin.Config.ShowWhenPfOpen;
        if (ImGui.Checkbox("Open with PF", ref openWithPf))
        {
            Plugin.Config.ShowWhenPfOpen = openWithPf;
            Plugin.Config.Save();
        }

        var groupSearchMode = Plugin.Config.GroupSearchMode;
        if (ImGui.Checkbox("Group search mode(Ignores preset Jobs, uses active party members ClassJobs or own ClassJob", ref groupSearchMode))
        {
            Plugin.Config.GroupSearchMode = groupSearchMode;
            Plugin.Config.Save();
        }

        var sideOptions = new[]
        {
            "Left",
            "Right",
        };
        var sideIdx = Plugin.Config.WindowSide == WindowSide.Left ? 0 : 1;

        ImGui.TextUnformatted("Side of PF window to dock to");
        if (ImGui.Combo("###window-side", ref sideIdx, sideOptions, sideOptions.Length))
        {
            Plugin.Config.WindowSide = sideIdx switch
            {
                0 => WindowSide.Left,
                1 => WindowSide.Right,
                _ => Plugin.Config.WindowSide,
            };

            Plugin.Config.Save();
        }
    }
}