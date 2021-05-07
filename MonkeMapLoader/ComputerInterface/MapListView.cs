﻿using ComputerInterface;
using ComputerInterface.ViewLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;

namespace VmodMonkeMapLoader.ComputerInterface
{
    public class MapListView : ComputerView
    {
        private readonly UIElementPageHandler<MapInfo> _pageHandler;
        private readonly UISelectionHandler _selectionHandler;

        private static List<MapInfo> _mapList = new List<MapInfo>();
        private MapInfo _selectedMap;

        private bool _isFirstView = true;
        private bool _isError;

        private CancellationTokenSource _splashCancelToken;

        private MapListView()
        {
            _selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);
            _selectionHandler.OnSelected += OnMapSelected;

            _pageHandler = new UIElementPageHandler<MapInfo>(EKeyboardKey.Left, EKeyboardKey.Right);
            _pageHandler.Footer = "    {2}:{3}  {0} {1}";
            _pageHandler.EntriesPerPage = 9;
        }

        public override async void OnShow(object[] args)
        {
            base.OnShow(args);
            
            if (_isFirstView)
            {
                SetText(str =>
                {
                    str.Repeat("=", SCREEN_WIDTH).AppendLine();
                    str.BeginCenter().Append("Monke Map Loader").AppendLine();
                    str.Append("by ").AppendClr("Vadix", "3fbc04").Append(" & ").AppendClr("Bobbie", "8dc2ef").EndAlign().AppendLine();
                    str.Repeat("=", SCREEN_WIDTH);
                });

                _splashCancelToken?.Cancel();
                _splashCancelToken?.Dispose();
                _splashCancelToken = new CancellationTokenSource();

                await HideSplashScreen(_splashCancelToken.Token);
                return;
            }

            RefreshMapList();
        }

        private void RefreshMapList(bool force = false)
        {
            if((_mapList == null || _mapList.Count == 0) || force) LoadMaps();

            _isError = false;
            DrawList();

            var selectedIdx = _pageHandler.GetAbsoluteIndex(_selectionHandler.CurrentSelectionIndex);

            if (_mapList.Count > 0)
                PreviewOrb.ChangeOrb(_mapList[selectedIdx]);
        }

        private void LoadMaps()
        {
            _mapList = MapFileUtils.GetMapList();
            _pageHandler.SetElements(_mapList.ToArray());
            _selectionHandler.CurrentSelectionIndex = 0;
        }

        private void DrawList()
        {
            if (_mapList.Count == 0)
            {
                SetText(DrawNoMaps);
                _isError = true;
                return;
            }

            if (_isError)
            {
                return;
            }

            var str = new StringBuilder();
            str.AppendClr("[^ / v] SELECT MAP        [ENTER] DETAILS", "8dc2ef").AppendLine().Append(new String(" ".ToCharArray()[0], 34) + "ONLINE:").AppendLine();

            _selectionHandler.MaxIdx = _pageHandler.ItemsOnScreen - 1;

            _pageHandler.EnumarateElements((map, idx) =>
            {
                // get player count stuff
                string mapName;
                if (!String.IsNullOrEmpty(map.PackageInfo.Config.GUID)) mapName = map.PackageInfo.Config.GUID + "_" + map.PackageInfo.Config.Version;
                else mapName = map.PackageInfo.Descriptor.Author + "_" + map.PackageInfo.Descriptor.Name;
                string playerCount = Behaviours.MonkeRoomManager.PlayersOnMap(mapName).ToString();

                var isSelected = idx == _selectionHandler.CurrentSelectionIndex;
                if (isSelected) str.BeginColor("8dc2ef").Append("> ");
                else str.Append("  ");

                str.Append(map.PackageInfo.Descriptor.Name.Clamp(30));
                str.Append(new String(" ".ToCharArray()[0], 39 - map.PackageInfo.Descriptor.Name.Clamp(30).Length - playerCount.Length) + playerCount);
                if (isSelected) str.EndColor();

                str.AppendLine();
            });

            str.AppendLine();

            _pageHandler.AppendFooter(str);

            SetText(str);

            var selectedIdx = _pageHandler.GetAbsoluteIndex(_selectionHandler.CurrentSelectionIndex);
            _selectedMap = _mapList[selectedIdx];
            PreviewOrb.ChangeOrb(_selectedMap);
        }

        private void DrawNoMaps(StringBuilder str)
        {
            str.Append("NO CUSTOM MAPS FOUND.").Repeat("\n", 2);
            str.Append("If you have map files in the folder").AppendLine();
            str.Append(" make sure they are in the right format.").Repeat("\n", 2);
            str.Append("You can find maps on the website:").AppendLine();
            str.BeginCenter().AppendClr("MonkeMapHub.com", "8dc2ef").EndAlign().Repeat("\n", 2);
            str.BeginCenter().Append("PRESS ANY BUTTON TO CONTINUE...").EndAlign();
        }

        private void OnMapSelected(int _)
        {
            if (_selectedMap == null) return;

            ShowView<MapDetailsView>(_selectedMap);
        }

        public override void OnKeyPressed(EKeyboardKey key)
        {
            if (_isFirstView)
            {
                _isFirstView = false;
                RefreshMapList();
                return;
            }

            if (_isError)
            {
                _isError = false;
                ReturnToMainMenu();
                return;
            }

            if (_selectionHandler.HandleKeypress(key))
            {
                DrawList();
                return;
            }

            if (_pageHandler.HandleKeyPress(key))
            {
                _selectionHandler.CurrentSelectionIndex = 0;
                DrawList();
                return;
            }

            switch (key)
            {
                case EKeyboardKey.Back:
                    ReturnToMainMenu();
                    break;
                case EKeyboardKey.Option1:
                    RefreshMapList(true);
                    break;
            }
        }

        private async Task HideSplashScreen(CancellationToken token)
        {
            try
            {
                await Task.Delay(5000, token);

                if (_isFirstView)
                {
                    _isFirstView = false;
                    RefreshMapList();
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}