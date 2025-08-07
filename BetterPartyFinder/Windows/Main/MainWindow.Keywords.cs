using System.Collections.Generic;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace BetterPartyFinder.Windows.Main;

public partial class MainWindow
{
    private bool WhitelistSelected = true;
    private string KeywordText = string.Empty;

    private void DrawKeywordsTab(ConfigurationFilter filter)
    {
        using (ImRaii.ItemWidth(ImGui.GetWindowWidth() * 0.50f))
            ImGui.InputText("###keyword-text", ref KeywordText, 64);

        ImGui.SameLine();
        if (ImGui.Button(WhitelistSelected ? "Whitelist" : "Blacklist"))
            WhitelistSelected = !WhitelistSelected;

        if (ImGui.IsItemHovered())
            Helper.Tooltip("Click to switch between whitelist and blacklist.");

        ImGui.SameLine();
        if (Helper.IconButton(FontAwesomeIcon.Plus, "add-keyword"))
        {
            var word = KeywordText.Trim();
            if (!string.IsNullOrEmpty(word))
            {
                (WhitelistSelected ? filter.Keywords.Whitelist : filter.Keywords.Blacklist).Add(word);

                KeywordText = string.Empty;
                Plugin.Config.Save();
            }
        }

        ImGui.NewLine();
        DrawKeywordList("Whitelist", filter.Keywords.Whitelist, filter);

        ImGui.NewLine();
        DrawKeywordList("Blacklist", filter.Keywords.Blacklist, filter);
    }

    private void DrawKeywordList(string label, List<string> keywords, ConfigurationFilter filter)
    {
        if (label == "Whitelist")
        {
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted("Whitelist Mode:");
            ImGui.SameLine();
            if (ImGui.Button($"{(filter.Keywords.Mode == WhitelistMode.All ? "ALL" : "ANY")}"))
            {
                filter.Keywords.Mode = filter.Keywords.Mode == WhitelistMode.All ? WhitelistMode.Any : WhitelistMode.All; // toggle between ALL and ANY
                Plugin.Config.Save();
            }

            ImGuiComponents.HelpMarker("Toggle if any or all whitelist terms must be in the description.");

            ImGui.Separator();

            ImGui.TextUnformatted("Whitelist:");
        }
        else
        {
            ImGui.Separator();

            ImGui.TextUnformatted("Blacklist:");
        }

        var toDelete = string.Empty;
        foreach (var word in keywords)
        {
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted(word);

            ImGui.SameLine();

            if (Helper.IconButton(FontAwesomeIcon.Trash, $"delete-keyword-{word}"))
                toDelete = word;
        }

        if (toDelete != string.Empty)
        {
            keywords.Remove(toDelete);
            Plugin.Config.Save();
        }
    }
}