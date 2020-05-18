using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utility.UI.Layout.Grid.Data;

namespace Utility.UI.Layout.Grid
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridContentController : MonoBehaviour
    {
        [Header("Get Ratio Values From")]
#pragma warning disable 649
        [SerializeField]
        private Camera cameraMain;
#pragma warning restore 649

        [Header("Predefined Aspect Ratios For Cell Size")] [SerializeField]
        private List<CellSizeData> cellSizeData = new List<CellSizeData>();

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private float currentAspectRatio = 0f;

#pragma warning disable 649
        private GridLayoutGroup grid;
#pragma warning restore 649

        private void OnValidate()
        {
            if (cameraMain == null) cameraMain = Camera.main;
            // ReSharper disable once PossibleNullReferenceException
            currentAspectRatio = cameraMain.aspect;

            if (grid == null) grid = GetComponent<GridLayoutGroup>();

            Resize();
        }

        private void Awake()
        {
            if (cameraMain == null) cameraMain = Camera.main;
            // ReSharper disable once PossibleNullReferenceException
            currentAspectRatio = cameraMain.aspect;

            if (grid == null) grid = GetComponent<GridLayoutGroup>();
        }

        private void OnEnable()
        {
            Resize();
        }

        private void Start()
        {
            Resize();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void Resize()
        {
            foreach (var data in cellSizeData.Where(data => Math.Abs(currentAspectRatio - data.Calculate()) < 0.01f))
            {
                grid.cellSize = data.cellSize;
                break;
            }
        }
    }
}