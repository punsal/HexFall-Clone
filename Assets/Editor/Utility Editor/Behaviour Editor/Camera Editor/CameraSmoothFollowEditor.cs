using UnityEditor;
using Utility.Behaviour.Camera;
using Utility.Editor;

namespace Editor.Utility_Editor.Behaviour_Editor.Camera_Editor
{
    [CustomEditor(typeof(CameraSmoothFollow))]
    public class CameraSmoothFollowEditor : ExtendedInspector<CameraSmoothFollow>
    {
        protected override void OnEnableAction()
        {
            GenericObject = (CameraSmoothFollow) target;
            GenericObjectType = typeof(CameraSmoothFollow);
        }
    }
}