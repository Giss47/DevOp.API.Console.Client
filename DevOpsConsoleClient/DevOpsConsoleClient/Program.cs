using DevOpsApiClient.Models;
using DevOpsApiClient.Models.PullRequest.Response;
using Hawk.Console.Client.FileService;
using Hawk.Console.Client.HawkApiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Gui;

namespace Hawk.Console.Client
{
    internal class Program
    {
        public static Dictionary<string, string> orgTokList;
        private static readonly DataFilesOps _dataFilesOps = new DataFilesOps();
        private static readonly IReadWriteOps _readWriteOps = new ReadWriteOps();
        private static Settings settings;
        private static readonly Window win = new Window("Welcome to DevOps Services") { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() - 1 };

        private static void Main()
        {
            try
            {
                _dataFilesOps.CreateDataFiles();
            }
            catch (UnauthorizedAccessException)
            {
                System.Console.WriteLine("Can't create Data files, Check access permissions to main drive");
                Thread.Sleep(4000);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                Thread.Sleep(4000);
                Environment.Exit(0);
            }
            settings = _readWriteOps.ReadSettings();
            orgTokList = _readWriteOps.ReadOrgTok();
            PaintGui();
        }

        /// <summary>
        /// Prepare the GUI main views and draw them to the Console
        /// </summary>
        private static void PaintGui()
        {
            Application.Init();
            var menu = CreateMenuBar();
            win.Add(CreateListPRButton());
            Application.Top.Add(menu, win);
            Application.Run();
        }

        /// <summary>
        /// Creates a MenuBar control
        /// </summary>
        /// <returns>MenuBar Object</returns>
        private static MenuBar CreateMenuBar()
        {
            var menu = new MenuBar(new MenuBarItem[] {
            new MenuBarItem ("_File", new MenuItem [] {
                new MenuItem ("_Settings", "", () => { CreateSettingEditDialog(); }),
                new MenuItem ("_Organizations", "", () => { CreateOrgEditDialog();}),
                 new MenuItem ("_Quit", "", () => { if (Quit ()) Application.Top.Running = false; })
            })});
            return menu;
        }

        /// <summary>
        /// Creates a Dialog that allow user to view and edit API settings
        /// </summary>
        private static void CreateSettingEditDialog()
        {
            var editSettingDialog = new Dialog("Edit Settings", 100, 100)
            {
                Height = Dim.Percent(50)
            };

            List<string> sourceList = new List<string>();
            var settingsListView = new ListViewSelection(sourceList)
            {
                X = 1,
                Y = 2,
                Width = Dim.Fill(6),
                Height = Dim.Fill() - 4
            };

            if (settings == null)
            {
                sourceList.Add("No Settings Added Yet");
                settingsListView.SetSource(sourceList);
            }
            else
            {
                sourceList.Add($"URI: {settings.BaseUrl}");
                sourceList.Add($"Key: {settings.ApiKey}");
                settingsListView.SetSource(sourceList);
            }

            var add = new Button("Edit Settings")
            {
                X = Pos.Center() - 20,
                Y = Pos.AnchorEnd(2),
                Clicked = () =>
                {
                    if (settings != null)
                    {
                        CreateInputDialog("Api", "Key", settings.BaseUrl, settings.ApiKey);
                    }
                    else
                    {
                        CreateInputDialog("Api", "Key");
                    }
                    if (settings != null)
                    {
                        sourceList.Clear();
                        sourceList.Add($"URI: {settings.BaseUrl}");
                        sourceList.Add($"Key: {settings.ApiKey}");
                        settingsListView.SetSource(sourceList);
                    }
                }
            };

            Button retrn = new Button("Return")
            {
                X = Pos.Center() + 10,
                Y = Pos.AnchorEnd(2),
                Clicked = () =>
                {
                    Application.RequestStop();
                }
            };
            editSettingDialog.Add(settingsListView, add, retrn);
            Application.Run(editSettingDialog);
        }

        /// <summary>
        /// Creates a Dialog that allow user to view, add, edit organiazations and tokens
        /// </summary>
        private static void CreateOrgEditDialog()
        {
            var editOrgDialog = new Dialog("Edit Organization/Token", 100, 100)
            {
                Height = Dim.Percent(90)
            };
            var sourceList = new List<string>();
            ListViewSelection orgViewList = new ListViewSelection(sourceList)
            {
                X = 1,
                Y = 4,
                Width = Dim.Fill() - 4,
                Height = Dim.Fill() - 4,
                AllowsMarking = true
            };

            var myColor = Application.Driver.MakeAttribute(Color.Blue, Color.White);

            if (orgTokList != null && orgTokList.Count != 0)
            {
                sourceList = orgTokList.Select(o => o.Key + "/" + o.Value).ToList();
                Label msg = new Label("Use space bar or control-t to toggle selection")
                {
                    X = 1,
                    Y = 1,
                    Width = Dim.Percent(50),
                    Height = 1,
                    TextColor = myColor
                };
                orgViewList.SetSource(sourceList);
                editOrgDialog.Add(orgViewList);
                editOrgDialog.Add(msg);
            }
            else
            {
                sourceList.Add("No Organizations saved");
                orgViewList.AllowsMarking = false;
                orgViewList.SetSource(sourceList);
                editOrgDialog.Add(orgViewList);
            }

            var add = new Button("Add new Org")
            {
                X = 5,
                Y = Pos.AnchorEnd(2),
                Clicked = () =>
                {
                    CreateInputDialog("Org", "Token");

                    if (sourceList.Count != 0)
                    {
                        sourceList.Clear();
                    }
                    if (orgTokList != null)
                    {
                        sourceList = orgTokList.Select(o => o.Key + "/" + o.Value).ToList();
                        orgViewList.Clear();
                        orgViewList.AllowsMarking = true;
                        orgViewList.SetSource(sourceList);
                        editOrgDialog.SetFocus(orgViewList);
                    }
                }
            };

            var edit = new Button("Edit Selected")
            {
                X = Pos.Right(add) + 10,
                Y = Pos.AnchorEnd(2),
                Clicked = () =>
                {
                    int index = 0;
                    for (int i = 0; i < sourceList.Count; i++)
                    {
                        if (orgViewList.Source.IsMarked(i))
                        {
                            int lengthBeforeEditing = orgTokList.Count;
                            index = sourceList[i].IndexOf('/');
                            string key = sourceList[i].Substring(0, index);
                            string value = sourceList[i].Substring(index + 1, (sourceList[i].Length - 1) - (index));
                            orgTokList.Remove(sourceList[i].Substring(0, index));
                            CreateInputDialog("Org", "Token", key, value);
                            if (orgTokList.Count == lengthBeforeEditing)
                            {
                                sourceList = orgTokList.Select(o => o.Key + "/" + o.Value).ToList();
                            }
                            else
                            {
                                orgTokList.Add(key, value);
                            }
                        }
                    }

                    orgViewList.Clear();
                    orgViewList.SetSource(sourceList);
                    editOrgDialog.SetFocus(orgViewList);
                }
            };

            var delete = new Button("Delete Selected")
            {
                X = Pos.Right(edit) + 30,
                Y = Pos.AnchorEnd(2),
                Clicked = () =>
                {
                    int index = 0;
                    for (int i = 0; i < sourceList.Count; i++)
                    {
                        if (orgViewList.Source.IsMarked(i))
                        {
                            index = sourceList[i].IndexOf('/');
                            orgTokList.Remove(sourceList[i].Substring(0, index));
                            sourceList.Remove(sourceList[i]);
                        }
                    }

                    if (orgTokList.Count == 0)
                    {
                        sourceList.Clear();
                        sourceList.Add("No Organizations Saved");
                        orgViewList.AllowsMarking = false;
                    }
                    _readWriteOps.SaveOrgTok(orgTokList);
                    orgViewList.Clear();
                    orgViewList.SetSource(sourceList);
                    editOrgDialog.SetFocus(orgViewList);
                }
            };

            Button retrn = new Button("Return")
            {
                X = Pos.Right(delete) + 50,
                Y = Pos.AnchorEnd(2),
                Clicked = () =>
                {
                    Application.RequestStop();
                }
            };

            editOrgDialog.Add(add, edit, delete, retrn, orgViewList);

            Application.Run(editOrgDialog);
        }

        /// <summary>
        /// Create dialogs to allow users to save app settings and organizations
        /// </summary>
        /// <param name="nameField"></param>
        /// <param name="keyField"></param>
        /// <returns></returns>
        private static void CreateInputDialog(string nameField, string keyField, string name = "", string key = "")
        {
            Dialog inputDialog = new Dialog($"Add new {nameField}", 60, 18);
            var namef = new Label(nameField) { X = 2, Y = 2 };
            var keyf = new Label(keyField) { X = Pos.Left(namef), Y = 5 };
            var nameText = new TextField(name) { X = Pos.Left(namef) + 7, Y = Pos.Top(namef), Width = Dim.Fill() - 2, Used = true };
            var keyText = new TextField(key) { X = Pos.X(nameText), Y = Pos.Y(keyf), Width = Dim.Fill() - 2, Used = true };

            Button ok = new Button("Save")
            {
                X = Pos.Left(keyText) + 15,
                Y = Pos.Bottom(keyText) + 10,
                Clicked = () =>
                {
                    name = nameText.Text.ToString();
                    key = keyText.Text.ToString();

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(key))
                    {
                        if (nameField == "Api")
                        {
                            if (settings != null)
                            {
                                settings.BaseUrl = name;
                                settings.ApiKey = key;
                                _readWriteOps.SaveSettings(name, key);
                                MessageBox.Query(50, 10, "", "\nDone!\n", "Esc");
                                Application.RequestStop();
                            }
                            else
                            {
                                settings = new Settings(name, key);
                                _readWriteOps.SaveSettings(name, key);
                                Application.RequestStop();
                            }
                        }
                        else
                        {
                            if (orgTokList != null)
                            {
                                if (orgTokList.ContainsKey(name))
                                {
                                    MessageBox.ErrorQuery(50, 10, "Warning", "Organization Already Exists", "Return");
                                }
                                else
                                {
                                    orgTokList.Add(name, key);
                                    _readWriteOps.SaveOrgTok(orgTokList);
                                    Application.RequestStop();
                                }
                            }
                            else
                            {
                                orgTokList = new Dictionary<string, string> { { name, key } };
                                _readWriteOps.SaveOrgTok(orgTokList);
                                Application.RequestStop();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.ErrorQuery(50, 10, "Warning", $"Both {nameField} and {keyField} can't be empty", "Esc");
                    }
                }
            };
            Button cancel = new Button("Esc")
            {
                X = Pos.Right(ok) + 25,
                Y = Pos.Bottom(keyText) + 10,
                Clicked = () =>
                {
                    Application.RequestStop();
                }
            };

            inputDialog.Add(namef, keyf, nameText, keyText, ok, cancel);

            Application.Run(inputDialog);
        }

        /// <summary>
        /// Creats and add a listView with all Pull Request to the GUI asynchronous
        /// </summary>
        private static async Task ListPullRequest()
        {
            var PRDialog = new Dialog("Pull Requests", 0, 0)
            {
                Width = Dim.Percent(90),
                Height = Dim.Percent(90)
            };
            var PRList = new List<string>();
            var PrView = new ListView(PRList)
            {
                X = 1,
                Y = 4,
                Width = Dim.Fill() - 4,
                Height = Dim.Fill() - 1
            };
            PRDialog.Add(PrView);
            var seperator = "--------------------------------------------------------------------------------------------------";
            var response = new PullRequestResponse();
            try
            {
                response = await PullRequestService.GetAllPR(orgTokList, settings);
                if (response != null)
                {
                    foreach (var org in response.Organizations)
                    {
                        if (org.Success)
                        {
                            PRList.Add(seperator);
                            var organization = "************ Org Name: " + org.OrganizationName + " ***************";
                            PRList.Add(organization);

                            foreach (var rep in org.Repositories)
                            {
                                if (rep.PullRequests.Length != 0)
                                {
                                    var repoName = "--REPO NAME--: " + rep.Name;
                                    PRList.Add(repoName);

                                    foreach (var pr in rep.PullRequests)
                                    {
                                        var pullReqcont = "  contributor: " + pr.DisplayName;
                                        var pullReqTitleDesc = "  Title/Description: " + pr.Title + "/" + pr.Description;
                                        var pullReqUri = "  Uri: " + pr.Url;
                                        PRList.Add(pullReqcont);
                                        PRList.Add(pullReqTitleDesc);
                                        PRList.Add(pullReqUri);
                                    }
                                }
                            }
                            PRList.Add(seperator);
                        }
                        else
                        {
                            var errorMessage = "Organiazation with name: " + "'" + org.OrganizationName + "' could not be found";
                            var adviceMessage = "Please check name spelling and access token";
                            PRList.Add(seperator);
                            PRList.Add(errorMessage);
                            PRList.Add(adviceMessage);
                            PRList.Add(seperator);
                        }
                    }
                }
                else if (response == null && orgTokList.Count != 0)
                {
                    PRList.Add("CONNECTION ERROR, Check settings or  internet connection");
                }
                else if (response == null && orgTokList.Count == 0)
                {
                    PRList.Add("NO ORGANIZATIONS ARE ADDED");
                }
                else
                {
                    if (PRList.Count == 0)
                        PRList.Add("NO PULL REQEUSTS ARE AVAILABLE AT THE MOMOENT");
                }
            }
            catch (Exception ex)
            {
                PRList.Add("Warning: Please check app Settings or internet Connection! ");
                PRList.Add("Http exception: " + ex.Message);
            }

            Button retrn = new Button("Return")
            {
                X = Pos.Center(),
                Y = Pos.AnchorEnd(1),
                Clicked = () =>
                {
                    Application.RequestStop();
                }
            };

            PRDialog.Add(retrn);
            Application.Run(PRDialog);
        }

        /// <summary>
        /// Quit the application
        /// </summary>
        /// <returns>returns True for Yes answer or False for no answer</returns>
        private static bool Quit()
        {
            var n = MessageBox.Query(50, 7, "Quit", "Are you sure you want to quit?", "Yes", "No");
            return n == 0;
        }

        /// <summary>
        /// Creates a button that calls ListPullRequest method
        /// </summary>
        /// <returns>A button object</returns>
        private static Button CreateListPRButton()
        {
            var button = new Button("Get Pull Requests")
            {
                X = Pos.Center(),
                Y = 10,
                Clicked = async () => { await ListPullRequest(); }
            };

            return button;
        }
    }
}