using UnityEngine;

namespace UI.SafeArea
{
    [ExecuteInEditMode]
    public class OnGUIDispatcher : MonoBehaviour
    {
        public delegate void OnGUIDelegate();
        public event OnGUIDelegate OnGUIEvent;

        void OnGUI() => OnGUIEvent?.Invoke();
    }
}