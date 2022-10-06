using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour {

    bool intro;
    bool outro;
    float alpha = 0;
    string sceneToLoad = "";

    Image image;

    private void Start() {
        intro = true;
        image = GetComponent<Image>();
        alpha = 1;
    }

    private void Update() {
        if (intro) {
            alpha -= Time.deltaTime;
            if (alpha <= 0) {
                alpha = 0;
                intro = false;
            }
        }
        if (outro) {
            alpha += Time.deltaTime;
            if (alpha >= 1) {
                alpha = 1;
                outro = false;
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        image.color = new Color(0, 0, 0, alpha);
        image.raycastTarget = intro || outro;

    }

    public void LoadScene(string scene) {
        outro = true;
        intro = false;
        sceneToLoad = scene;
    }
}
