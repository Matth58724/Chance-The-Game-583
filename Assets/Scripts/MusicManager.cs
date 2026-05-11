using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    public AudioSource audioSource;
    public AudioClip[] playlist; // Songs go in here
    private int currentTrackIndex = 0;

    void Awake()
    {
        // Check if an instance already exists
        if (instance != null)
        {
            Destroy(gameObject); // Delete the duplicate
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Keep this alive between scenes
    }

    void Start()
    {
        if (playlist.Length > 0)
        {
            PlayTrack(0);
        }
    }

    void Update()
    {
        // Only check if song ended when the app is focused. 
        // Audio used to change songs when tabbing in and out
        if (!Application.isFocused) return;


        // If the song ends then just move to the next one
        if (!audioSource.isPlaying)
        {
            currentTrackIndex = (currentTrackIndex + 1) % playlist.Length;
            PlayTrack(currentTrackIndex);
        }
    }

    void PlayTrack(int index)
    {
        audioSource.clip = playlist[index];
        audioSource.Play();
    }
}