using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;

namespace DCF.Core
{
    public static class DirectConnectionsService
    {
        public static Result Run(
            ExternalCommandData commandData,
            ref string message)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                var sel = uidoc.Selection.GetElementIds();
                if (sel.Count != 1)
                {
                    TaskDialog.Show("Direct Connection Finder",
                        "Please select a single element.");
                    return Result.Failed;
                }

                Element element = doc.GetElement(sel.First());
                if (!(element is FamilyInstance fi))
                {
                    TaskDialog.Show("Direct Connection Finder",
                        "The selected element is not a FamilyInstance.");
                    return Result.Failed;
                }

                MEPModel mep = fi.MEPModel;
                if (mep == null)
                {
                    TaskDialog.Show("Direct Connection Finder",
                        "The element has no electrical connections.");
                    return Result.Failed;
                }

                // --- Grouping elements by electrical system ---
                Dictionary<ElectricalSystem, List<Element>> systemGroups =
                    new Dictionary<ElectricalSystem, List<Element>>();

                HashSet<ElementId> allIds = new HashSet<ElementId> { element.Id };

                foreach (ElectricalSystem sys in mep.GetAssignedElectricalSystems())
                {
                    if (!systemGroups.ContainsKey(sys))
                        systemGroups[sys] = new List<Element>();

                    foreach (Element e in sys.Elements)
                    {
                        if (e.Id == element.Id) continue;
                        if (allIds.Contains(e.Id)) continue;

                        systemGroups[sys].Add(e);
                        allIds.Add(e.Id);
                    }
                }

                // --- Icon map ---
                var iconMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Light", "◎" },
                    { "Lighting", "◎" },

                    { "Recept", "🔌" },
                    { "Outlet", "🔌" },
                    { "Socket", "🔌" },

                    { "Panel", "▣" },
                    { "Switchboard", "▣" },

                    { "Junction", "◆" },
                    { "Box", "◆" },

                    { "Electrical Fixture", "◈" },
                    { "Fixture", "●" },

                    { "Electrical Equipment", "▤" }
                };

                string GetIconForCategory(string category)
                {
                    if (string.IsNullOrEmpty(category))
                        return "●";

                    foreach (var kvp in iconMap)
                    {
                        if (category.IndexOf(kvp.Key,
                            StringComparison.OrdinalIgnoreCase) >= 0)
                            return kvp.Value;
                    }

                    return "●";
                }

                // --- Build dialog text ---
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Selected Element");
                sb.AppendLine(
                    $"└── ● {(element.Category?.Name ?? "—")} | {element.Name} | " +
                    $"{(element.LookupParameter("Comments")?.AsString() ?? "")}");

                sb.AppendLine("\nElements by Electrical System");

                if (systemGroups.Count == 0)
                {
                    sb.AppendLine("└── (no connected systems)");
                }
                else
                {
                    var sortedSystems = systemGroups.Keys
                        .OrderBy(sys =>
                        {
                            return int.TryParse(sys.CircuitNumber, out int num)
                                ? num
                                : int.MaxValue;
                        })
                        .ThenBy(sys => sys.CircuitNumber);

                    int sysIndex = 0;
                    int sysTotal = sortedSystems.Count();

                    foreach (var sys in sortedSystems)
                    {
                        bool lastSys = sysIndex == sysTotal - 1;
                        string sysBranch = lastSys ? "└─ " : "├─ ";

                        string sysName = string.IsNullOrEmpty(sys.CircuitNumber)
                            ? "—"
                            : sys.CircuitNumber;

                        string loadClass = string.IsNullOrEmpty(sys.LoadClassifications)
                            ? "—"
                            : sys.LoadClassifications;

                        sb.AppendLine($"{sysBranch}⚡ {sysName} | {loadClass}");

                        var sysElements = sys.Elements.Cast<Element>().ToList();

                        if (!sysElements.Any())
                        {
                            sb.AppendLine(
                                $"{(lastSys ? "    " : "│   ")}└── (no elements)");
                        }
                        else
                        {
                            for (int i = 0; i < sysElements.Count; i++)
                            {
                                bool lastEl = i == sysElements.Count - 1;

                                string prefix = lastSys ? "    " : "│   ";
                                string elBranch = lastEl ? "└──" : "├──";

                                var e = sysElements[i];
                                string cat = e.Category?.Name ?? "—";
                                string comment =
                                    e.LookupParameter("Comments")?.AsString() ?? "";

                                string displayName;

                                if (cat.IndexOf("Electrical Equipment",
                                    StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    displayName =
                                        e.LookupParameter("Panel")?.AsString()
                                        ?? e.Name;
                                }
                                else if (e is FamilyInstance fi2)
                                {
                                    displayName = fi2.Symbol.Name;
                                }
                                else
                                {
                                    displayName = e.Name;
                                }

                                string icon = GetIconForCategory(cat);

                                sb.AppendLine(
                                    $"{prefix}{elBranch} {icon} {cat} | " +
                                    $"{displayName} | {comment}");
                            }
                        }

                        sb.AppendLine(lastSys ? "" : "│");
                        sb.AppendLine(lastSys ? "" : "│");

                        sysIndex++;
                    }
                }

                sb.AppendLine($"\nTotal elements connected: {allIds.Count - 1}");

                TaskDialog td = new TaskDialog("Direct Connection Finder")
                {
                    MainInstruction =
                        "List of connected elements by Electrical System:",
                    MainContent = sb.ToString(),
                    CommonButtons = TaskDialogCommonButtons.Ok
                };
                td.Show();

                uidoc.Selection.SetElementIds(allIds.ToList());

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}