using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoginScreen : MonoBehaviour
{
    [SerializeField] private GameObject loginScreen = null;
    [SerializeField] private Graphic[] loginElements = null;

    [SerializeField] private float exitDuration = 0.1f;

    private float[] colorAlphas;

    private void Start()
    {
        colorAlphas = new float[loginElements.Length];
        for (int i = 0; i < loginElements.Length; i++)
        {
            colorAlphas[i] = loginElements[i].color.a;
        }
    }
    public void Login(bool login)
    {
        if (login)
        {
            foreach(Graphic g in loginElements)
            {
                g.DOColor(new Color(g.color.r, g.color.g, g.color.b, 0f), exitDuration);
            }
            StartCoroutine(DisableLogin());
        }
        else
        {
            loginScreen.SetActive(true);
            for (int i = 0; i < loginElements.Length; i++)
            {
                loginElements[i].DOColor(new Color(loginElements[i].color.r, loginElements[i].color.g, loginElements[i].color.b, colorAlphas[i]), exitDuration);
            }
        }
    }
    private IEnumerator DisableLogin()
    {
        yield return new WaitForSeconds(exitDuration);
        loginScreen.SetActive(false);
    }
}
