using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaController : MonoBehaviour
{
    public delegate void Bananas();
    public static Bananas BananasUpdated;

    [SerializeField] TMPro.TextMeshProUGUI _txtBananasCounter = null;
    [SerializeField] GameObject _BananaEffect = null;

    private void Awake()
    {
        GlobalVariables._curBananas = PlayerPrefs.GetInt(GlobalVariables._bananasID, 0);
    }

    private void OnEnable()
    {
        BananasUpdated += UpdateText;
        BananasUpdated += SaveBananas;

        BananasUpdated?.Invoke();
    }

    private void OnDisable()
    {
        BananasUpdated -= UpdateText;
        BananasUpdated += SaveBananas;
    }

    void UpdateText()
    {
        _txtBananasCounter.text = GlobalVariables._curBananas.ToString();
    }

    void SaveBananas()
    {
        PlayerPrefs.SetInt(GlobalVariables._bananasID, GlobalVariables._curBananas);
        PlayerPrefs.Save();
    }

    public void EatBanana()
    {
        if( GlobalVariables._curBananas > 0 )
        {
            GlobalVariables._curBananas--;
            BananasUpdated?.Invoke();

            _BananaEffect.SetActive(true);
        }
    }
}