﻿using CentrED.Map;
using CentrED.UI;

namespace CentrED.Tools;

public class HueTool : Tool {
    internal HueTool(UIManager uiManager) : base(uiManager) { }

    public override string Name => "HueTool";
    
    private bool _pressed;
    private StaticObject _focusObject;

    public override void OnActivated(MapObject? o) {
        _uiManager._huesWindow.Show = true;
    }

    public override void OnMouseEnter(MapObject? o) {
        if (o is StaticObject so) {
            so.Hue = (ushort)(_uiManager._huesWindow.SelectedId + 1);
        }
    }
    
    public override void OnMouseLeave(MapObject? o) {
        if (o is StaticObject so) {
            so.Hue = so.StaticTile.Hue;
        }
    }

    public override void OnMousePressed(MapObject? o) {
        if (!_pressed && o is StaticObject so) {
            _pressed = true;
            _focusObject = so;
        }
    }
    
    public override void OnMouseReleased(MapObject? o) {
        if (_pressed && o is StaticObject so && so == _focusObject) {
            if(_uiManager._huesWindow.SelectedId != -1)
                so.StaticTile.Hue = (ushort)(_uiManager._huesWindow.SelectedId + 1);
        }
        _pressed = false;
    }
}