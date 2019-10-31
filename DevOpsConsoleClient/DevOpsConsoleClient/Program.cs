using System;
using Terminal.Gui;

namespace DevOpsConsoleClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            PaintGui();
        }

        private static void PaintGui()
        {
            Application.Init();

            var menu = new MenuBar(new MenuBarItem[] {
            new MenuBarItem ("_File", new MenuItem [] {
                new MenuItem ("_Add Settings", "", () => {
                    //AddSettings();
                }),
                new MenuItem ("_Add New organizatin", "", () => {
                   // AddOrganization();
                }),
                 new MenuItem ("_Quit", "", () => {
                    Application.RequestStop ();
                })
            }),
        });
            Label welcome = new Label(2, 2, "Welcome to DevOps Services");

            Application.Top.Add(menu, welcome);
            Application.Run();
        }
    }
}