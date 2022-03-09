using System;
using ContextMenuCreator;
using ContextMenuCreator.Common;

namespace create_build_context_menus
{
    class Program
    {
        const string devToolsFolder = "nipkg-dev-tools\\nipkg-context-menu\\";

        static void Main(string[] args)
        {
            string sysDrive = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%") + "\\";
            string buildExe = "nipkg-context-menu.exe";
            string images = "images" + "\\";
            string path = sysDrive + devToolsFolder;

            Menu top = new Menu("nipkgDevTools");
            top.SetRoot(Roots.DIRECTORY);
            top.AssignDisplay(new ItemDisplay("nipkg-dev-tools", path + images + "Tools.ico"));

            ContextMenuItem buildDefaultItem = new ContextMenuItem("buildDefault");
            buildDefaultItem.AssignDisplay(new ItemDisplay("Build to Default", path + images + "Hammer.ico"));
            buildDefaultItem.MapExe(new Executable(path + buildExe, new string[] {"%1"}));

            ContextMenuItem buildHereItem = new ContextMenuItem("buildHere");
            buildHereItem.AssignDisplay(new ItemDisplay("Build Here", path + images + "Hammer.ico"));
            buildHereItem.MapExe(new Executable(path + buildExe, new string[] {"%1", "--here"}));

            top.AddContextMenuItem(buildDefaultItem);
            top.AddContextMenuItem(buildHereItem);
            top.CreateAll();
        }
    }
}
