using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    private Frogger frogger;
    private Home[] homes;

    public GameObject gameOverMenu;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    public AudioClip Background;

    AudioSource audioSource;


    private int score;
    private int lives;
    private int time;

    private void Awake()
    {
        gameOverMenu.SetActive(false);
        homes = FindObjectsOfType<Home>();
        frogger = FindObjectOfType<Frogger>();
    }

    private void Start()
    {
        NewGame();
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("QUIT!");
            Application.Quit();
    }
    }
    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        NewLevel();
        PlaySound(Background);
    }

    private void NewLevel()
    {
        for (int i = 0; i < homes.Length; i++) {
            homes[i].enabled = false;
        }

        Respawn();
    }


    private void Respawn()
    {
        frogger.Respawn();

        StopAllCoroutines();
        StartCoroutine(Timer(30));
    }

    private IEnumerator Timer(int duration)
    {
        time = duration;
        timerText.text = time.ToString();

        while (time >0)
        {
            yield return new WaitForSeconds(1);
            time --;
            timerText.text = time.ToString();
        }

        frogger.Death();
    }

    public void AdvancedRow()
    {
        SetScore(score + 10);
    }

    public void Died()
    {
       SetLives(lives - 1);

        if (lives > 0) 
        {
            Invoke(nameof(Respawn), 1f);
        } 
        else 
        {
            Invoke(nameof(GameOver), 1f);
        }
    }

    private void GameOver()
    {
        frogger.gameObject.SetActive(false);
        SceneManager.LoadScene("GameOver");

        StopAllCoroutines();
        StartCoroutine(PlayAgain());
    }

    private IEnumerator PlayAgain()
    {
        bool playAgain = false;

        while (!playAgain)
        {
            if (Input.GetKeyDown(KeyCode.Return)) {
                playAgain = true;
            }

            yield return null;
        }

        NewGame();
    }
    public void HomeOccupied()
    {
        frogger.gameObject.SetActive(false);

        int bonusPoints = time * 20;
        SetScore(score + bonusPoints + 50);

        if (Cleared())
        {
            SetScore(score + 1000);
            Invoke(nameof(NewLevel), 1f);
            SceneManager.LoadScene("FroggerLevel2");

        }
        else 
        {
            Invoke(nameof(Respawn), 1f);
        }
    }

    private bool Cleared()
    {
        for (int i = 0; i < homes.Length; i++)
        {
            if (!homes[i].enabled) 
            {
                return false;
            }
        }

        return true;
    }


    private void SetScore (int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = lives.ToString();
    }

    public void PlayAgainButton() {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}

