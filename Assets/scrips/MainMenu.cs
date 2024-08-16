using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameInfo gameInfo;
    public void playAddition()
    {
        gameInfo.arithmeticSymbol = "+";
        gameInfo.difficulty = 1;
        SceneManager.LoadScene("Game");
    }
    public void playSubtraction()
    {
        gameInfo.arithmeticSymbol = "-";
        gameInfo.difficulty = 1;
        SceneManager.LoadScene("Game");
    }
    public void playMultipikation()
    {
        gameInfo.arithmeticSymbol = "*";
        gameInfo.difficulty = 1;
        SceneManager.LoadScene("Game");
    }
    public void playDevision()
    {
        gameInfo.arithmeticSymbol = "/";
        gameInfo.difficulty = 1;
        SceneManager.LoadScene("Game");
    }
}
