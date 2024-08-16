using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   /// <summary>
   /// Problem grade es werden nur 4 Feldenn selected und firstNNum ist an der richtigen stelle
   /// solution ist an der stelle von seconnd Num und 
   /// secondNum wird nicht weitergegeben
   /// </summary>
   public GameInfo gameInfo;
   public GameObject cam;
   private float areaHeight;
   private float areaWidth;

   private readonly int gameWidth = 5;
   private readonly int gameHeight = 10;

   public Tile tilePreFab;
   public Tile[] tiles;
   public Tile[][] equatiosAsTiles;
   public int[][] equations;
   private int equationCount = 0;
   public int amountOfEquations = 1; //provisorisch --> zahl wird später durch die schwierigkeit gewählt  
   private string arithmeticSymbol = "+"; //provisorisch
   private int maxRange = 10;
   private int minRange = 0;
   private bool gridStatus = false;
   private int notSolved = 0;

   public void solvedEquation()
   {
      notSolved --;
      if(notSolved == 0)
      {
         Debug.Log("YOU WON");
         gameInfo.difficulty ++;
         SceneManager.LoadScene("Game");
      }
   }
   private void Awake()
   {
      setUpDifficulty();
      setUpGame();
   }

   private void setUpGame()
   {
      areaHeight = cam.GetComponent<Camera>().pixelHeight;
      areaWidth = cam.GetComponent<Camera>().pixelWidth;

      tiles = new Tile[gameWidth*gameHeight];
      equatiosAsTiles = new Tile[amountOfEquations][];
      equations = new int[amountOfEquations][];
      GenerateGrid();
      setFirstEquation();
      for(int i = 1; i <= amountOfEquations-1; i++)
      {
         setEquation();
      }

      removeTiles(); //those tiles need to be solved 
   }

   private void setUpDifficulty()
   {
      if(gameInfo.difficulty < 4)
      {
         amountOfEquations = gameInfo.difficulty + 1;
      }
      else
      {
         amountOfEquations = 4;
         maxRange = 20;
         minRange = 0;
      }

      arithmeticSymbol = gameInfo.arithmeticSymbol;

   }

   private void removeTiles()
   {
      List<Tile>[] equationNumbers = new List<Tile>[amountOfEquations];
      for(int i = 0; i<amountOfEquations; i++)
      {
         List<Tile> numberTiles = new List<Tile>();
         for(int j = 0;j<5;j++)
         {
            if(equatiosAsTiles[i][j].getStoredValuePos() == "firstNum" || equatiosAsTiles[i][j].getStoredValuePos() == "secondNum" || equatiosAsTiles[i][j].getStoredValuePos() == "solution")
            {
               numberTiles.Add(equatiosAsTiles[i][j]);
            }
         }
         equationNumbers[i] = numberTiles;
      }

      for(int i = 0; i<equationNumbers.Length; i++)
      {
         int randomIndex = Random.Range(0, equationNumbers[i].Count);
         Tile randomTile = equationNumbers[i][randomIndex];

         randomTile.setToBeSolved(true);
         notSolved++;

         for(int j = i+1; j<equationNumbers.Length; j++)
         {
            if(equationNumbers[j].Contains(randomTile))
            {
               equationNumbers[j].Remove(randomTile);
            }
         }

      }
   }
   private void GenerateGrid() //generiert das Raster
   {
      for (int x = 0; x < gameWidth; x++)
      {
         for (int y = 0; y < gameHeight; y++)
         {
            int index = x * gameHeight + y;
            tiles[index] = Instantiate(tilePreFab, new Vector2(x,y),Quaternion.identity);
            tiles[index].name= "Tile: " +x+y;
            tiles[index].setIndex(index);
            tiles[index].setGameHight(gameHeight);
            tiles[index].setGameWidth(gameWidth);
            tiles[index].setGameManager(this);

            //bool isOffset = (x % 2 == 0 && y % 2 != 0) || (y % 2 == 0 && x % 2 != 0);
            tiles[index].GetComponent<Tile>().Initialise();
         }
      }

      Vector3 center = calculateOrthoSize().center;
      Debug.Log("center" + center);
      float size = calculateOrthoSize().size;

      cam.transform.position = center;//new Vector3((float)width/2 -0.5f,(float)height/2 - 0.5f,-10);
      cam.GetComponent<Camera>().orthographicSize = size;//width + 0.5f;
      gridStatus = true;
   }
 /**
   erstellt eine Gleichung basierend auf dem Arithmetiischen Symbol.
   die Zahlen werden zwischen der angegebenen Spanne ausgewäählt.
   die Funktion gibt die erste Zahl, die zweite Zahl und die Lösungg einzelnt wieder.

 **/
   private (Vector3 center, float size) calculateOrthoSize()
   {
      Bounds bounds = new Bounds();
      for(int i=0; i<tiles.Length; i++)
      {
         bounds.Encapsulate(tiles[i].colBounds());
      }
      
      float vertical = bounds.size.y;
      float horizontal = bounds.size.x * areaHeight / areaWidth;

      float size = Mathf.Max(horizontal, vertical)*0.5f;
      Vector3 center = bounds.center + new Vector3(0,0,-10);
      Debug.Log("height"+areaHeight);
      Debug.Log("width"+areaWidth);

      return(center,size);
   }
   public void createEquation(string arithmeticSymbol, int minRange, int maxRage, out int firstNum, out int secondNum, out int solution, int value = default, string valuePos = default) //erstellt eine Gleichung basierent auf dem Rechensybol und der Spannweite an Zahlen. Gibt die drei Zahlen der Gleichung wieder
   {
      int[] equation = new int[3];
      firstNum = 0;
      secondNum = 0;
      solution = 0;
      if(valuePos == default) //-> ValuePos marks where in the equation we are. first num, seccond num, solution
      {
        if(arithmeticSymbol == "+")
         {
            firstNum = Random.Range(minRange,maxRage);
            secondNum = Random.Range(minRange,maxRage);
            solution = firstNum + secondNum;
         }
         else if(arithmeticSymbol == "-")
         {
            firstNum = Random.Range(minRange,maxRage);
            secondNum = Random.Range(minRange,firstNum);
            solution = firstNum - secondNum;
         }
         else if(arithmeticSymbol == "*")
         {
            firstNum = Random.Range(minRange,maxRage);
            secondNum = Random.Range(minRange,maxRage);
            solution = firstNum * secondNum;    
         }
         else if(arithmeticSymbol == "/")
         {
            firstNum = Random.Range(minRange,maxRage);
            secondNum = Random.Range(minRange,firstNum);
            solution = firstNum / secondNum;
         }
         else if(arithmeticSymbol == "?")
         {
            Debug.Log("In Progress");
         } 
      }
      else
      {
         Debug.Log("value"+value+"valuePos"+valuePos+"range"+maxRage);
         if(arithmeticSymbol == "+")
         {
            if(valuePos == "firstNum")
            {
               firstNum = value;
               secondNum = Random.Range(minRange,maxRage);
               solution = firstNum + secondNum;
            }
            else if (valuePos == "secondNum")
            {
               firstNum = Random.Range(minRange,maxRage);
               secondNum = value;
               solution = firstNum + secondNum;
            }else if (valuePos == "solution")
            {
               solution = value;
               firstNum = Random.Range(minRange,maxRage);
               secondNum = solution - firstNum;
            }
            
         }
         else if(arithmeticSymbol == "-")
         {
            if(valuePos == "firstNum")
            {
               firstNum = value;
               secondNum = Random.Range(minRange,firstNum);
               solution = firstNum - secondNum;
            }
            else if (valuePos == "secondNum")
            {
               firstNum = Random.Range(minRange,maxRage);
               secondNum = value;
               solution = firstNum - secondNum;
            }else if (valuePos == "solution")
            {
               solution = value;
               secondNum = Random.Range(minRange,maxRage);
               firstNum = solution + secondNum;
            }
            
         }
         else if(arithmeticSymbol == "*")
         {
            if(valuePos == "firstNum")
            {
               firstNum = value;
               secondNum = Random.Range(minRange,maxRage);
               solution = firstNum + secondNum;
            }
            else if (valuePos == "secondNum")
            {
               firstNum = Random.Range(minRange,maxRage);
               secondNum = value;
               solution = firstNum + secondNum;
            }else if (valuePos == "solution")
            {
               solution = value;
               firstNum = Random.Range(minRange,maxRage);
               secondNum = solution / firstNum;
            }    
         }
         else if(arithmeticSymbol == "/")
         {
            if(valuePos == "firstNum")
            {
               firstNum = value;
               secondNum = Random.Range(minRange,firstNum);
               solution = firstNum - secondNum;
            }
            else if (valuePos == "secondNum")
            {
               firstNum = Random.Range(minRange,maxRage);
               secondNum = value;
               solution = firstNum - secondNum;
            }else if (valuePos == "solution")
            {
               solution = value;
               secondNum = Random.Range(minRange,maxRage);
               firstNum = solution * secondNum;
            }
         }
         else if(arithmeticSymbol == "?")
         {
            Debug.Log("In Progress");
         } 
      }
      Debug.LogWarning("create methode1: "+firstNum+"+"+secondNum+"="+solution);
      equation[0] = firstNum;
      equation[1] = secondNum;
      equation[2] = solution;
      for (int i = 0; i < equations.Length; i++)
      {
         if(equations[i] == null)
         {
            equations[i] = equation;
            Debug.LogWarning("create methode2: "+equations[i][1]);
            break;
         }
      }
      equationCount++;
      Debug.Log("count++"+equationCount);
   }

   private Tile getMiddleTile() //Gewährt zugriff auf das mittlerste Feld im Raster
   {
      int middle = tiles.Length/2;;
      Debug.Log(middle);
      return tiles[middle];
   }

   private Tile getTileAbove(Tile curTile, int nTile) //Gewährt zugriff auf das Feld über dem Angegebenen
   {
      int above = curTile.getIndex() + nTile;
      if(above >= gameWidth*gameHeight)
      {
         return null;
      }
      return tiles[above];
   }

   private Tile getTileBelow(Tile curTile, int nTile) //Gewährt zugriff auf das Feld unter dem Angegebenen
   {
      int below = curTile.getIndex() - nTile;
      if(below < 0)
      {
         return null;
      }
      return tiles[below];
   }

   private Tile getTileRight(Tile curTile, int nTile)
   {
      int right = curTile.getXValue() +nTile;
      int pos = right * gameHeight + curTile.getYValue();
      if(right > gameWidth-1)
      {
         return null;
      }
      return tiles[pos];
   }
      private Tile getTileLeft(Tile curTile, int nTile)
   {
      int left = curTile.getXValue() -nTile;
      int pos = left * gameHeight + curTile.getYValue();
      if(left < 0)
      {
         return null;
      }
      return tiles[pos];
   }

   public void setFirstEquation() //plaziert die erste Gleichung in die Mitte des Rasters 
   {
      createEquation(arithmeticSymbol, minRange, maxRange, out int firstNum, out int secondNum, out int solution);//test values
      Tile[] firstEquation = new Tile[5];
      firstEquation[2] = getMiddleTile(); //Mittlerer Tile der Gleichung 
      firstEquation[2].setStoredValue(secondNum);
      firstEquation[2].setStoredValuePos("secondNum");

      firstEquation[1] = getTileAbove(getMiddleTile(),1); //Tile eins über dem Mittleren
      firstEquation[1].setStoredValuePos(arithmeticSymbol);

      firstEquation[0] = getTileAbove(getMiddleTile(),2); //Tile zwei über dem Mittleren
      firstEquation[0].setStoredValue(firstNum);
      firstEquation[0].setStoredValuePos("firstNum");

      firstEquation[3] = getTileBelow(getMiddleTile(),1); //Tile eins unter dem Mittleren
      firstEquation[3].setStoredValuePos("=");

      firstEquation[4] = getTileBelow(getMiddleTile(),2); //Tile zwei unter dem Mittlerem
      firstEquation[4].setStoredValue(solution);
      firstEquation[4].setStoredValuePos("solution");
        
      equatiosAsTiles[0] = firstEquation;
      for (int i = 0; i < equatiosAsTiles[0].Length; i++)
      {
         Debug.Log("set Equation: "+equatiosAsTiles[0][i].getStoredValuePos());
         equatiosAsTiles[0][i].selectTile(true);
         equatiosAsTiles[0][i].setCollumOrRow("collum");
      }
   }

   private void setEquation() //plaziert eine Gleichung an eine andere
   {
      int randomEquationIndex = Random.Range(0,equationCount); //random Equation
      List<Tile[]> equationAsTilesList = new List<Tile[]>(); //List for passing to chooseConnectingTile
      for(int i = 0; i <equationCount;i++)
      {
         equationAsTilesList.Add(equatiosAsTiles[i]);
      }

      (Tile connectTile, List<string> possibleNewPositions) returnValues= chooseConnectingTile(equationAsTilesList, randomEquationIndex);

      Tile connectTile = returnValues.connectTile;// could retrun null
      List<string> possibleNewPositions = returnValues.possibleNewPositions;
      
      if(connectTile != null && possibleNewPositions.Count > 0)
      {
         string newValuePos = possibleNewPositions[Random.Range(0,possibleNewPositions.Count)];

         Tile[] equation = new Tile[5];
         if(connectTile.getCollumOrRow() == "collum") //save the Space
         {
            if(newValuePos == "firstNum") 
            {
               equation[0] = connectTile;
               for(int i=1;i<=4;i++)
               {
                  equation[i] = getTileRight(connectTile,i);
               }
            }
            else if (newValuePos == "secondNum")
            {
               equation[2] = connectTile;
               equation[1] = getTileLeft(connectTile,1);
               equation[0] = getTileLeft(connectTile,2);
               equation[3] = getTileRight(connectTile,1);
               equation[4] = getTileRight(connectTile,2);
            }
            else if(newValuePos == "solution")
            {
               equation[4] = connectTile;
               equation[3] = getTileLeft(connectTile,1);
               equation[2] = getTileLeft(connectTile,2);
               equation[1] = getTileLeft(connectTile,3);
               equation[0] = getTileLeft(connectTile,4);
            }
         
         }
         else if(connectTile.getCollumOrRow()=="row")
         {
            if(newValuePos == "firstNum") //save the Space
            {
               equation[0] = connectTile;
               for(int i=1;i<=4;i++)
               {
                  equation[i] = getTileBelow(connectTile,i);
               }
            }
            else if (newValuePos == "secondNum")
            {
               equation[2] = connectTile;
               equation[1] = getTileAbove(connectTile,1);
               equation[0] = getTileAbove(connectTile,2);
               equation[3] = getTileBelow(connectTile,1);
               equation[4] = getTileBelow(connectTile,2);

            }
            else if(newValuePos == "solution")
            {
               equation[4] = connectTile;
               equation[3] = getTileAbove(connectTile,1);
               equation[2] = getTileAbove(connectTile,2);
               equation[1] = getTileAbove(connectTile,3);
               equation[0] = getTileAbove(connectTile,4);
            }     
         }
      
         createEquation(arithmeticSymbol,minRange,maxRange,out int firstNum, out int secondNum,out int solution, connectTile.getStoredValue(), newValuePos);
         equation[0].setStoredValue(firstNum);
         equation[0].setStoredValuePos("firstNum");
         equation[1].setStoredValuePos(arithmeticSymbol);
         equation[2].setStoredValue(secondNum);
         equation[2].setStoredValuePos("secondNum");
         equation[3].setStoredValuePos("=");
         equation[4].setStoredValue(solution);
         equation[4].setStoredValuePos("solution");
         Debug.Log("count"+equationCount);
         equatiosAsTiles[equationCount-1]=equation;
         string connectTileOrientation = connectTile.getCollumOrRow();
         for (int i = 0; i < equatiosAsTiles[equationCount-1].Length; i++)
         {
            equatiosAsTiles[equationCount-1][i].selectTile(true);
            if(equatiosAsTiles[equationCount-1][i]!=connectTile)
            {
               if(connectTileOrientation == "collum")
               {
                  equatiosAsTiles[equationCount-1][i].setCollumOrRow("row");
               }
               else
               {
                  equatiosAsTiles[equationCount-1][i].setCollumOrRow("collum");
               }
            }
            else
            {
               equatiosAsTiles[equationCount-1][i].setCollumOrRow("both");
            }

            
         }
      }
      else
      {
         Debug.Log("No More Equations possible");
      }
      
   }

   private (Tile selectedTile,List<string> possibleNewPositions) chooseConnectingTile(List<Tile[]> possibleEquations, int equationIndex) //algorithm needs update for more efficency 
   {

      List<Tile> selectedEquation = new List<Tile>();
      for(int i = 0; i <possibleEquations[equationIndex].Length; i++)
      {
         if(possibleEquations[equationIndex][i].getStoredValuePos() == "firstNum" || possibleEquations[equationIndex][i].getStoredValuePos() == "secondNum" || possibleEquations[equationIndex][i].getStoredValuePos() == "solution")
         {
            selectedEquation.Add(possibleEquations[equationIndex][i]);
         }
      }

      int randomTileIndex = Random.Range(0,selectedEquation.Count);
      (Tile selectedTile, List<string> possibleNewPostions) returnValue = (null,null);

      if(selectedEquation[randomTileIndex].getCollumOrRow() == "both") // gets rid of tiles that have already beenn selected 
      {
         selectedEquation.RemoveAt(randomTileIndex);
         randomTileIndex = Random.Range(0,selectedEquation.Count);
      }
      if(selectedEquation.Count <= 0) //if every Number Tile in the selected equation has been selected before then a new one has to bee selected 
      {
         possibleEquations.RemoveAt(equationIndex);
         if(possibleEquations.Count > 0)
            {
               chooseConnectingTile(possibleEquations, Random.Range(0,possibleEquations.Count));
            }
            else
            {
               return (null,null);
            }
      }

         returnValue = GetPossibleNewPos(selectedEquation, randomTileIndex);
         Tile selectedTile =  returnValue.selectedTile;
         List<string> newPositions = returnValue.possibleNewPostions;

         if(newPositions == null && selectedTile == null) //No fitting Tile was found in that Eqation
         {
            possibleEquations.RemoveAt(equationIndex);
            if(possibleEquations.Count > 0)
            {
               chooseConnectingTile(possibleEquations, Random.Range(0,possibleEquations.Count));
            }
            else
            {
               return (null,null);
            }
         }
      return (returnValue.selectedTile,returnValue.possibleNewPostions);
      
   }

   private (Tile selectedTile, List<string> possibleNewPostions) GetPossibleNewPos(List<Tile> selectedEquation, int index)
   {
      Tile selectedTile = selectedEquation[index];
      
      List<string> possiblePositions = new List<string>();


      if(selectedTile.getCollumOrRow()=="collum")
      {
         int freeTilesRight = 0;
         int freeTilesLeft = 0;
         for(int i=1;i<=4;i++)
         {
            if(getTileRight(selectedTile,i) != null && !getTileRight(selectedTile,i).checkSelected())
            {
               freeTilesRight++;
            }
         }
         for(int i=1;i<=4;i++)
         {
            if(getTileLeft(selectedTile,i) != null && !getTileLeft(selectedTile,i).checkSelected())
            {
               freeTilesLeft++;
            }
           
         }

         if(freeTilesRight>=4) //firstNum possible
         {
            possiblePositions.Add("firstNum"); 
         }
         if(freeTilesLeft>=4) //secondNum possible
         {
            possiblePositions.Add("solution");
         }
         if(freeTilesRight>=2 && freeTilesLeft>=2) //solution possible
         {
            possiblePositions.Add("secondNum");
         }
         if(possiblePositions.Count<1) //no possition possible
         {
            selectedEquation.RemoveAt(index);
            if(selectedEquation.Count > 0) //at least one Number are still in the Equation
            {
               GetPossibleNewPos(selectedEquation,Random.Range(0,selectedEquation.Count));
            }
            else
            {
               return (null,null);
            }
            
         }
      }
      else if(selectedTile.getCollumOrRow()=="row")
      {
         int freeTilesAbove = 0;
         int freeTilesBelow = 0;
         for(int i=1;i<=4;i++)
         {
            if(getTileAbove(selectedTile,i) != null && !getTileAbove(selectedTile,i).checkSelected())
            {
               freeTilesAbove++;
            }
         }
         for(int i=1;i<=4;i++)
         {
            if(getTileBelow(selectedTile,i) != null && !getTileBelow(selectedTile,i).checkSelected())
            {
               freeTilesBelow++;
            }
         }
         if(freeTilesAbove>=4) //firstNum possible
         {
            possiblePositions.Add("firstNum");  
         }
         if(freeTilesBelow>=4) //solution possible
         {
            possiblePositions.Add("solution");
         }
         if(freeTilesAbove>=2 && freeTilesBelow>=2) //secondNum
         {
            possiblePositions.Add("secondNum");
         }
         if(possiblePositions.Count<1) //no possition possible
         {
            selectedEquation.RemoveAt(index);
            if(selectedEquation.Count > 0) //arithmeticSymbol and equalSign + at least one Number are still in the Equation
            {
               GetPossibleNewPos(selectedEquation,Random.Range(0,selectedEquation.Count));
            }
            else
            {
               return (null,null);
            }
            
         }
      }
      return (selectedTile,possiblePositions);
   }

   private int RandomTileInt()
   {
      int randomTileNum = Random.Range(1,4);
      int tileInt = 0;
      if(randomTileNum == 1)
      {
        tileInt = 0;
      }
      else if(randomTileNum == 2)
      {
         tileInt = 2;
      }
      else if(randomTileNum == 3)
      {
         tileInt = 4;
      }
      return tileInt;
   }

   private string chooseNewValuePos()
   {
      string[] possibleValuePositions = new string[3];
      possibleValuePositions[0]="firstNum";
      possibleValuePositions[1]="secondNum";
      possibleValuePositions[2]="solution";
      int newPos = Random.Range(0,2);
      return possibleValuePositions[newPos];
   }

   public bool getGridStatus()
   {
      return gridStatus;
   }

   
}
