
using UnityEngine;
[CreateAssetMenu(fileName = "new GameInfo", menuName = "GameInfo")]
public class GameInfo : ScriptableObject
{
    public string arithmeticSymbol;
    public int difficulty; 

    private void OnEnable()
    {
        arithmeticSymbol = null;
        difficulty = 0;
    }
}
