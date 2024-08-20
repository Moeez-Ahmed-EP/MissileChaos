using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gb_GameOver;
    [SerializeField] GameObject gb_Controls;
    [SerializeField] GameObject gb_HealthPrefab;
    [SerializeField] GameObject gb_Menu;


    [SerializeField] TMP_Text txt_ScoreText;

    [SerializeField] int i_CurrentScore = 0;
    [SerializeField] int i_MaxHealth = 3;
    [SerializeField] int i_CurrentHealth = 3;
    [SerializeField] int i_HouseDestroyed = 0;

    [SerializeField] RectTransform rct_HealthParent;

    [SerializeField] HouseManager cmp_HouseManager;

    public static Action<int> OnEnemyDeath;
    public static Action<int> OnPlayerDeath;
    public static Action OnHouseDestroyed;


    private void OnEnable()
    {
        OnEnemyDeath += UpdateScoreText;
        OnPlayerDeath += UpdateHealthImage;
        OnHouseDestroyed += spawnNewWave;
        spawnHealthImages();
    }

    private void OnDisable()
    {
        OnEnemyDeath -= UpdateScoreText;
        OnPlayerDeath -= UpdateHealthImage;
        OnHouseDestroyed -= spawnNewWave;
    }

    public void GameOver()
    {
        gb_Controls.SetActive(false);
        gb_GameOver.SetActive(true);
    }

    public void OnClick_Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClick_Play()
    {
        gb_Menu.SetActive(false);
        gb_Controls.SetActive(true);
        for (int i = 0; i < cmp_HouseManager.numberOfHouses; i++)
        {
            cmp_HouseManager.SpawnHouse();
        }
        PlayerMovement.OnStartbutton(true);
    }

    public void UpdateScoreText(int score)
    {
        i_CurrentScore += score;
        txt_ScoreText.text = i_CurrentScore.ToString();
    }

    public void spawnHealthImages()
    {
        int HealthToInstantiate = 0;
        if (rct_HealthParent.childCount == 0)
        {
            HealthToInstantiate = i_MaxHealth;
        }
        else
        {
            HealthToInstantiate = i_MaxHealth - rct_HealthParent.childCount;
        }

        for (int i = 0; i < HealthToInstantiate; i++)
        {
            Instantiate(gb_HealthPrefab, rct_HealthParent);
        }

    }

    public void UpdateHealthImage(int count)
    {
        if (count >= 0)
        {
            Destroy(rct_HealthParent.GetChild(count).gameObject);
        }


        if (count == 0)
        {
            //Show Game Over screen
            Debug.Log("GameOver");
            gb_GameOver.SetActive(true);
        }
        else
        {
            Debug.Log("Respawn");
            PlayerHealth.respawnPlayer();
            i_CurrentHealth--;
        }
    }

    public void spawnNewWave()
    {
        i_HouseDestroyed++;
        if(i_HouseDestroyed == cmp_HouseManager.i_HouseSpawned)
        {
            cmp_HouseManager.numberOfHouses++;
            i_HouseDestroyed = 0;
            cmp_HouseManager.i_HouseSpawned = 0;
            cmp_HouseManager.spawnedPositions.Clear();
            for (int i = 0; i < cmp_HouseManager.numberOfHouses; i++)
            {
                cmp_HouseManager.SpawnHouse();
            }
        }
    }

}
