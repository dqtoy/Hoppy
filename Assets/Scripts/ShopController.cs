using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopController : MonoBehaviour {

    public void onBackButtonClicked()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        // Load the previous scene.
        SceneManager.LoadScene("GamePlay");
    }
}
