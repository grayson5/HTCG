using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneReloader: MonoBehaviour {

    public void ReloadScene()
    {
        // Command has some static members, so let`s make sure that there are no commands in the Queue
        Debug.Log("Scene reloaded");
        // reset all card and creature IDs
        IDFactory.ResetIDs();
        IDHolder.ClearIDHoldersList();
        Command.CommandQueue.Clear();
        Command.CommandExecutionComplete();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void LoadScene(string SceneName)
    {
#if UNITY_IOS
        SceneManager.LoadScene("BattlesceneiOS");
#elif UNITY_STANDALONE_WIN
        SceneManager.LoadScene("BattlescenePC");
#endif
    }

    public void Quit()
    {
        Debug.Log("In Quit");
        Application.Quit();
    }
}
