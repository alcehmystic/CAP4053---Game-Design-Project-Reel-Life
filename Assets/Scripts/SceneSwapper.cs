using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //dont forget me!
public class SceneSwapper : MonoBehaviour
{
#pragma warning disable 0649 //private variables
    [SerializeField] private string sceneName;
#pragma warning restore 0649
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Player")
            SceneManager.LoadScene(sceneName);
    }
}
