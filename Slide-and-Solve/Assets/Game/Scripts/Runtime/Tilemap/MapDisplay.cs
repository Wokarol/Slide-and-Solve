using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Wokarol.TilemapUtils
{
    public class MapDisplay : MonoBehaviour
    {
        [SerializeField] Tilemap tilemap;

        private void Start() {
            UpdateMap();
        }

        private void UpdateMap() {
            throw new NotImplementedException();
        }
    } 
}
