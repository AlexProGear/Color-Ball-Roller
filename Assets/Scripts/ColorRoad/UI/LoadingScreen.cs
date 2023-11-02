using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColorRoad.UI
{
    public class LoadingScreen : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadSceneAsync(1);
        }
    }
}