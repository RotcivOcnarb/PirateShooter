using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Fade fade;
    [SerializeField] Animator optionsAnimator;
    [SerializeField] TextMeshProUGUI sessionTimeText;
    [SerializeField] TextMeshProUGUI enemySpawnText;

    string sessionTimePattern;
    string enemySpawnPattern;

    private void Start() {
        sessionTimePattern = sessionTimeText.text;
        enemySpawnPattern = enemySpawnText.text;
        if (!PlayerPrefs.HasKey("EnemySpawn")) {
            PlayerPrefs.SetFloat("EnemySpawn", 5);
        }
    }

    private void Update() {
        sessionTimeText.text = string.Format(sessionTimePattern, TimeSpan.FromSeconds(Time.unscaledTime).ToString(@"hh\:mm\:ss"));
        float enemySpawn = PlayerPrefs.GetFloat("EnemySpawn");
        enemySpawnText.text = string.Format(enemySpawnPattern, enemySpawn + "s");
    }

    public void OnSliderChanged(float value) {
        PlayerPrefs.SetFloat("EnemySpawn", value);
    }

    public void OpenOptionsMenu() {
        optionsAnimator.Play("Open", 0, 0);
    }

    public void CloseOptionsMenu() {
        optionsAnimator.Play("Close", 0, 0);
    }

    public void PlayGame() {
        fade.LoadScene("GameScene");
    }
}
