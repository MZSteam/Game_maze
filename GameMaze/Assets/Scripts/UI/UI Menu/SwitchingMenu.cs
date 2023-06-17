using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchingMenu : MonoBehaviour
{
    public GameObject MenuStart;
    public GameObject MenuDifficulty;
    void Start()
    {
        MenuStart.SetActive(true);
        MenuDifficulty.SetActive(false);
    }
    public void MenuChangeDifficulty()
    {
        MenuStart.SetActive(false);
        MenuDifficulty.SetActive(true);
    }
    public void MenuChangeStart()
    {
        MenuStart.SetActive(true);
        MenuDifficulty.SetActive(false);
    }
}
