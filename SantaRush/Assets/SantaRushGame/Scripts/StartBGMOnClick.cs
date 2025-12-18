using UnityEngine;

public class StartBGMOnClick : MonoBehaviour
{
    public void PlaySceneBGM()
    {
        BGMManager.Instance?.PlayPreparedBGM();
    }
}
