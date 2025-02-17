﻿using System.Text;
using CentrED.Server;
using ImGuiNET;

namespace CentrED.UI.Windows; 

public class ServerWindow : Window {
    public ServerWindow(UIManager uiManager) : base(uiManager) { }
    public override string Name => "Local Server";
    private string _configPath = Config.ServerConfigPath;
    private StreamReader? _logReader;
    private StringBuilder _log = new();

    public override void Draw() {
        if (!Show) return;

        ImGui.Begin(Id, ref _show );
        ImGui.InputText("Config File", ref _configPath, 512);
        ImGui.SameLine();
        if (ImGui.Button("...")) {
            ImGui.OpenPopup("open-file");
        }
        var isOpen = true;
        if (ImGui.BeginPopupModal("open-file", ref isOpen, ImGuiWindowFlags.NoTitleBar)) {
            var picker = FilePicker.GetFilePicker(this, Environment.CurrentDirectory, ".xml");
            if (picker.Draw()) {
                _configPath = picker.SelectedFile;
                Config.ServerConfigPath = _configPath;
                FilePicker.RemoveFilePicker(this);
            }
            ImGui.EndPopup();
        }

        if (CentrED.Server != null && CentrED.Server.Running) {
            if (ImGui.Button("Stop")) {
                CentrED.Client.Disconnect();
                CentrED.Server.Quit = true;
            }
        }
        else {
            if (ImGui.Button("Start")) {
                if (CentrED.Server != null) {
                    CentrED.Server.Dispose();
                }

                _log.Clear();
                Config.ServerConfigPath = _configPath;
                CentrED.Server = new CEDServer(new[] { _configPath }, new StreamWriter(File.Open("cedserver.log", FileMode.Create, FileAccess.Write, FileShare.ReadWrite)){AutoFlush = true});
                new Task(() => {
                    try {
                        CentrED.Server.Run();
                    }
                    catch (Exception e) {
                        Console.WriteLine("Server stopped");
                        Console.WriteLine(e);
                    }
                }).Start();
                _logReader = new StreamReader(File.Open("cedserver.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            }
        }
        
        ImGui.Separator();
        ImGui.Text("Server Log:");
        ImGui.BeginChild("ServerLogRegion");
        if (_logReader != null) {
            do {
                var line = _logReader.ReadLine();
                if (line == null) break;
                _log.AppendLine(line);
            } while (true);
        }
        ImGui.TextUnformatted(_log.ToString());
        ImGui.EndChild();

        ImGui.End();
    }
}