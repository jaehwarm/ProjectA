using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleTest : MonoBehaviour
{
    public void EnterTitleScene(int id)
    {
        TitleScene scene = FindFirstObjectByType<TitleScene>();
        scene.EnterScene();

        TitleView view = FindFirstObjectByType<TitleView>();
        view.ApplyDesignTitle(id);

        Destroy(this.gameObject);
    }
}
