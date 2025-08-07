using System;
using System.Linq;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace BetterPartyFinder.Windows.Main;

public partial class MainWindow
{
    private void DrawCategoriesTab(ConfigurationFilter filter)
    {
        using var table = ImRaii.Table("CategoryTable", 2, ImGuiTableFlags.BordersInnerV);
        if (!table.Success)
            return;

        ImGui.TableSetupColumn("##Selected");
        ImGui.TableSetupColumn("##Add");

        ImGui.TableNextColumn();
        Helper.TextColored(ImGuiColors.HealerGreen, "Show:");
        ImGui.Separator();

        ImGui.TableNextColumn();
        Helper.TextColored(ImGuiColors.ParsedOrange, "Hide:");
        ImGui.Separator();

        ImGui.TableNextColumn();
        foreach (var category in filter.Categories.ToArray().OrderBy(c => c))
        {
            if (!ImGui.Selectable(category.Name()))
                continue;

            filter.Categories.Remove(category);
            Plugin.Config.Save();
        }

        ImGui.TableNextColumn();
        foreach (var category in Enum.GetValues<UiCategory>().Where(c => !filter.Categories.Contains(c)))
        {
            if (!ImGui.Selectable(category.Name()))
                continue;

            filter.Categories.Add(category);
            Plugin.Config.Save();
        }
    }
}