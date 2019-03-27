using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Wokarol.PuzzleProcessors;

namespace Wokarol.TilemapUtils
{
    public class MapDisplay : MonoBehaviour
    {
        [SerializeField] Grid _grid = null;
        [Space]
        [SerializeField] Tilemap _tilemap = null;
        [SerializeField] TileBase _tile = null;
        [Space]
        [SerializeField] GameObject _winPrefab = null;
        [SerializeField] GameObject _playerPrefab = null;
        [Space]
        [SerializeField] int _border = 0;
        [SerializeField] float _speed = 5;

        GameObject _winPrefab_instance;
        GameObject _player_instance;
        public bool Moving { get; private set; }

#if UNITY_EDITOR
        SlidingPuzzleMap _editor_map;
#endif

        public void SetMap(SlidingPuzzleMap map) {
#if UNITY_EDITOR
            _editor_map = map;
#endif

            var coords = new List<Vector3Int>();
            var tiles = new List<TileBase>();

            _tilemap.ClearAllTiles();

            var walls = map.Walls;

            Vector3 offset = new Vector3(-(walls.GetLength(0) * .5f), -(walls.GetLength(1) * .5f), 0);
            _grid.transform.position = offset;

            for (int x = -_border; x < walls.GetLength(0) + _border; x++) {
                for (int y = -_border; y < walls.GetLength(1) + _border; y++) {
                    if (x < 0 || x >= walls.GetLength(0) ||
                        y < 0 || y >= walls.GetLength(1) ||
                        walls[x, y]) {
                        coords.Add(new Vector3Int(x, y, 0));
                        tiles.Add(_tile);
                    }
                }
            }

            if (_winPrefab_instance == null)
                _winPrefab_instance = Instantiate(_winPrefab, transform);

            _winPrefab_instance.transform.position = _grid.GetCellCenterWorld(new Vector3Int(map.WinCoords.x, map.WinCoords.y, 0));

            _tilemap.SetTiles(coords.ToArray(), tiles.ToArray());
        }

        public void ShowTransition(SlidingPuzzleProcessor.Transition[] transitions, Action callback) {
            if (!Moving) {
                StartCoroutine(ShowTransitionCoroutine(transitions, callback));
            }
        }

        private IEnumerator ShowTransitionCoroutine(SlidingPuzzleProcessor.Transition[] transitions, Action callback) {
            Moving = true;
            for (int i = 0; i < transitions.Length; i++) {
                Vector3 end = _grid.GetCellCenterWorld((Vector3Int)transitions[i].EndCoords);
                Transform playerTransform = _player_instance.transform;

                while (playerTransform.position != end) {
                    playerTransform.position = Vector3.MoveTowards(playerTransform.position, end, Time.deltaTime * _speed);
                    yield return null;
                }

            }
            Moving = false;
            callback?.Invoke();
        }

        public void ShowState(SlidingPuzzleState state) {
            if (_player_instance == null)
                _player_instance = Instantiate(_playerPrefab, transform);

            _player_instance.transform.position = _grid.GetCellCenterWorld((Vector3Int)state.PlayerCoords);
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (Application.isPlaying && _editor_map != null) {
                SetMap(_editor_map);
            }
        }
#endif
    }
}
