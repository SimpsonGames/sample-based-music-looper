using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicUI : MonoBehaviour
{
    [SerializeField] private SampleMusicLooper musicLooper;
    
    [SerializeField] private TMP_Text progressText;

    private void Update()
    {
        musicLooper.UpdateDebugText(progressText);
    }
}
