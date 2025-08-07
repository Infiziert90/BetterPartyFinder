using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace BetterPartyFinder.Windows.Config;

public partial class ConfigWindow
{
    private void General()
    {
        using var tabItem = ImRaii.TabItem("General");
        if (!tabItem.Success)
            return;

        var changed = false;

        changed |= ImGui.Checkbox("Open with PF", ref Plugin.Config.ShowWhenPfOpen);

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

        ImGuiHelpers.ScaledDummy(5.0f);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);

        changed |= ImGui.Checkbox("Disable in World tab", ref Plugin.Config.DisableInWorld);
        changed |= ImGui.Checkbox("Disable in Private tab", ref Plugin.Config.DisableInPrivate);

        if (changed)
            Plugin.Config.Save();
    }
}