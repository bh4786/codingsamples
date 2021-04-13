using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoginScreen : MonoBehaviour
{
    [SerializeField] private GameObject loginScreen = null; //ui for login screen
    [SerializeField] private Graphic[] loginElements = null; //all graphic elements of login ui

    [SerializeField] private float exitDuration = 0.1f; //time it takes to exit login screen

    private float[] colorAlphas;

    private void Start()
    {
        colorAlphas = new float[loginElements.Length]; //set length of color alphas array to length of login elements

        for (int i = 0; i < loginElements.Length; i++) //set each color alpha to alphas of login elements
        {
            colorAlphas[i] = loginElements[i].color.a;
        }
    }
    public void Login(bool login)
    {
        if (login)
        {
            foreach(Graphic g in loginElements) //for each graphic element, tween alpha to zero
            {
                g.DOColor(new Color(g.color.r, g.color.g, g.color.b, 0f), exitDuration);
            }
            StartCoroutine(DisableLogin());
        }
        else
        {
            loginScreen.SetActive(true); //enable login screen
            for (int i = 0; i < loginElements.Length; i++) //for each login element, tween alpha to original alpha
            {
                loginElements[i].DOColor(new Color(loginElements[i].color.r, loginElements[i].color.g, loginElements[i].color.b, colorAlphas[i]), exitDuration);
            }
        }
    }
    private IEnumerator DisableLogin()
    {
        yield return new WaitForSeconds(exitDuration);
        loginScreen.SetActive(false); //disable login ui
    }
}
