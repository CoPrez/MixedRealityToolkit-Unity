﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Interfaces;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.MarkerDetection
{
    /// <summary>
    /// Controls displaying of the AR marker on the mobile device
    /// </summary>
    public class ArUcoMarkerVisual : MonoBehaviour,
        IMarkerVisual
    {
        static string[] cCodes = new string[] {
            "000111100011110111011000001010100110",
            "000011101111101110100011100010010001",
            "000101011001000001111110101011001101",
            "110010010001101100110000011010011110",
            "110101100000011111010110111000010101",
            "110110001110100011100000111001101000",
            "010000100110100010110100000111110101",
            "100010001010010100001111001010011010",
            "001100000111110101010010010011111101",
            "001111000010111100110100101100111100",
            "010001011101111111000111010011100011",
            "010010001101100001011011001001010111",
            "011100010000010101011000111111000110",
            "100001101101110011111010110100000111",
            "100011010111001010101001001111110110",
            "101000101011100010011101110011011110",
            "000010011111110100011110100111000100",
            "000101010100110110111101000110001111",
            "001100000000101000110001000011100010",
            "010010000000011111101111101011111101",
            "010101101101111100010001110110110110",
            "011001101000100000110010011101001100",
            "011101101110100011001011011110000001",
            "100110100101001111011001110011110011",
            "101010011100101110000100000000100100",
            "110001100111010101001001010010010000",
            "110000011101001010001000100101000001",
            "111001110100100000001000010100101011",
            "111010100010111111001010100001001000",
            "111010010110001110110111011110110001",
            "111110100011011001100101001010101111",
            "000001100101101111111111011110111101",
            "000001010100000111010111001011010110",
            "000011001111011100100100011010100010",
            "000100110011100010100011100111101011",
            "000101011010100010010011111001110100",
            "001110100100000101111110111010011110",
            "010011110001000111100010011011000000",
            "010100110000110110110110110100100000",
            "010110001001101111111010111000110100",
            "011001000000100111101000101000001011",
            "011000000101001101111010100010010001",
            "011000010101100100000110100110111010",
            "011010111111111101111000110101111011",
            "011100001010110110010110101001001111",
            "011101011000010001101111011100011010",
            "011110101001010100011001001011111100",
            "100001100000100101110110000010101010",
            "100010100010110101000100110000111111",
            "100100111110101101111000101100010100",
            "100110001000110110101000010011010100"
        };

        #region Fields
        [Tooltip("Background plane")]
        [SerializeField] GameObject backgroundPlane;
        [Tooltip("GameObject that will contain the code")]
        [SerializeField] Transform _arUcoCodeContainer;
        [Tooltip("Material applied to white sections of SpectatorView marker")]
        [SerializeField] Material _whiteMaterial;
        [Tooltip("Material applied to black sections of SpectatorView marker")]
        [SerializeField] Material _blackMaterial;

        List<GameObject> _cubes = new List<GameObject>();
        ScreenOrientation _originalOrientation;
        #endregion

        #region Events
        private void OnEnable()
        {
            // Setting screen rotation to portrait when dispalying AR code
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _originalOrientation = Screen.orientation;
                Screen.orientation = ScreenOrientation.Portrait;
            }
        }

        private void OnDisable()
        {
            OnDismiss();
        }

        private void OnDestroy()
        {
            OnDismiss();
        }

        private void OnDismiss()
        {
            // Setting screen rotation to autorotation when AR code is dismissed
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Screen.orientation = _originalOrientation;
            }
        }
        #endregion

        public void ShowMarker(int id)
        {
            gameObject.SetActive(true);
            _arUcoCodeContainer.localRotation = Quaternion.Euler(360, 360, 360);
            CreateMarker(id);
        }

        public void HideMarker()
        {
            ClearMarker();
            backgroundPlane.GetComponent<Renderer>().sharedMaterial.color = Color.white;
            gameObject.SetActive(false);
        }

        #region Marker Code
        [ContextMenu("Generate")]
        void TestGenerator()
        {
            CreateMarker(0);
        }

        void ClearMarker()
        {
            for (int i = 0; i < _cubes.Count; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(_cubes[i]);
                }
                else
                {
                    DestroyImmediate(_cubes[i]);
                }
            }
            _cubes.Clear();
        }

        void CreateMarker(int id)
        {
            ClearMarker();
            Texture2D marker = MakeMarkerTex(cCodes[id]);

            int xW = (marker.width) * 2;
            for (int x = 0; x < xW; x++)
            {
                for (int y = 0; y < xW; y++)
                {
                    float col = marker.GetPixel((x / 2), (y / 2)).r;

                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.layer = gameObject.layer;
                    cube.transform.parent = _arUcoCodeContainer;
                    cube.transform.localPosition = new Vector3(x / (float)xW - 0.5f, 0, y / (float)xW - 0.5f);
                    cube.transform.localScale = Vector3.one * 1.0f / xW;
                    cube.transform.localRotation = Quaternion.identity;

                    _cubes.Add(cube);
                    if (col > 0.1f)
                        cube.GetComponent<Renderer>().enabled = false;
                    else
                        cube.GetComponent<Renderer>().sharedMaterial = _blackMaterial;
                }
            }
        }

        static Texture2D MakeMarkerTex(string data, int dataSize = 6, int border = 1)
        {
            bool[] boolData = new bool[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                boolData[i] = data[i] == '1';
            }
            return MakeMarkerTex(boolData, dataSize, border);
        }

        static Texture2D MakeMarkerTex(bool[] data, int dataSize = 6, int border = 1)
        {
            int size = dataSize + border * 2;
            Color[] colorData = new Color[size * size];

            int dataId = 0;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int i = x + ((size - 1) - y) * size;

                    if (x >= border && y >= border && x < size - border && y < size - border)
                    {
                        colorData[i] = dataId > data.Length ? Color.black : (data[dataId] ? Color.white : Color.black);
                        dataId += 1;
                    }
                    else
                    {
                        colorData[i] = Color.black;
                    }
                }
            }

            Texture2D result = new Texture2D(size, size, TextureFormat.RGBA32, false);
            result.SetPixels(colorData);
            result.Apply();
            result.filterMode = FilterMode.Point;

            return result;
        }
        #endregion
    }
}