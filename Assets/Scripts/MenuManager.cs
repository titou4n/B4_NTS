using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    //__________________________________________________________//
    //________________________MainMenu__________________________//
    //__________________________________________________________//

    public void PlayGame()
    {
        SceneManager.LoadScene("ARScene2");
    }

    public void QuitGame()
    {
        Debug.Log("Quitter le jeu !");
        Application.Quit();
    }
}