using UnityEngine;

namespace CustomPostProcess.Runtime
{
    public class CustomPostProcessComponent : MonoBehaviour
    {
        [SerializeField]
        private CustomPostProcessAdapter adapter;

        public void Activate(bool isOn) => adapter.Activate(isOn);
    }
}