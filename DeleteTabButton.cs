using UdonSharp;

namespace TabSystem
{
    public class DeleteTabButton : UdonSharpBehaviour
    {
        void Start()
        {
        
        }

        public void OnClick()
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
