using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerEntity : GameEntity
{
    public Image healthBar;

    // Update is called once per frame
    void Update() {
        healthBar.transform.localScale = new Vector3(health / maxHealth, 1f, 1f);
        if(health == 0) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("YouDied");
        }
    }
}
