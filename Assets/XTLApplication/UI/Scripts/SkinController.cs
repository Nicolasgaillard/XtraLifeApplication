using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinController : MonoBehaviour
{
    public Material _avatarMaterial;

    public void SetSkin(Image img)
    {
        _avatarMaterial.color = img.color;
        FindObjectOfType<GameManager>().SaveUserColor(img.color);
        FindObjectOfType<UIController>().ActivePanel = UIController.UIPanel.None;
    }
}
