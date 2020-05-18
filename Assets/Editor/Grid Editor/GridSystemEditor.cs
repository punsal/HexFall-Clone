using Grid;
using UnityEditor;
using Utility.Editor;

namespace Editor.Grid_Editor
{
    [CustomEditor(typeof(GridSystem))]
    public class GridSystemEditor : ExtendedInspector<GridSystem>
    {
        protected override void OnEnableAction()
        {
            GenericObject = (GridSystem) target;
            GenericObjectType = typeof(GridSystem);
        }
    }
}